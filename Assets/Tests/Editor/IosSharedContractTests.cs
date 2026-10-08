using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using ConsentManagementProvider;
using NUnit.Framework;
using UnityEngine;

public class IosSharedContractTests
{
    [Test]
    public void IosConsentPodIsNotAddedToAllTargetsAndBridgeRemainsInUnityFramework()
    {
        var dependenciesPath = Path.Combine(Application.dataPath,
            "ConsentManagementProvider/Editor/SourcepointDependencies.xml");
        var dependencies = XDocument.Load(dependenciesPath);
        var consentPod = dependencies.Descendants("iosPod")
            .SingleOrDefault(pod => (string)pod.Attribute("name") == "ConsentViewController");

        Assert.That(consentPod, Is.Not.Null, "The iOS ConsentViewController dependency must remain declared.");
        Assert.That(consentPod.Attribute("addToAllTargets"), Is.Null,
            "ConsentViewController and transitive SPMobileCore must not be linked into Unity-iPhone and UnityFramework.");

        var postProcessPath = Path.Combine(Application.dataPath,
            "ConsentManagementProvider/Editor/CMPPostProcessBuild.cs");
        var postProcess = File.ReadAllText(postProcessPath);
        Assert.That(postProcess, Does.Contain("project.RemoveFileFromBuild(mainTarget, bridgeGuid)"),
            "The native bridge must be removed from the Unity-iPhone target.");
        Assert.That(postProcess, Does.Contain("project.AddFileToBuild(frameworkTarget, bridgeGuid)"),
            "The native bridge must remain compiled by UnityFramework, the sole owner of the CMP pod.");
    }

    [Test]
    public void NativeBridgeImportsSwiftHeadersFromTheirOwningFrameworksInDependencyOrder()
    {
        var bridgeSourcePath = Path.Combine(Application.dataPath,
            "ConsentManagementProvider/Plugins/iOS/Source/Unity.mm");
        var bridgeSource = File.ReadAllText(bridgeSourcePath);

        var consentHeader = "#import <ConsentViewController/ConsentViewController-Swift.h>";
        var unityHeader = "#import <UnityFramework/UnityFramework-Swift.h>";
        var consentHeaderIndex = bridgeSource.IndexOf(consentHeader, StringComparison.Ordinal);
        var unityHeaderIndex = bridgeSource.IndexOf(unityHeader, StringComparison.Ordinal);

        Assert.That(consentHeaderIndex, Is.GreaterThanOrEqualTo(0),
            "The consent Swift interface must be imported from the CocoaPods framework module.");
        Assert.That(unityHeaderIndex, Is.GreaterThan(consentHeaderIndex),
            "The UnityFramework Swift interface must be imported after the consent interface it uses.");
        Assert.That(bridgeSource, Does.Not.Contain("#import \"UnityFramework-Swift.h\""),
            "The generated UnityFramework Swift interface is not available as a local header during Xcode builds.");
    }

    [Test]
    public void SwiftBridgeKeepsConsentDelegateTypesOutOfItsPublicObjcInterface()
    {
        var bridgeSourcePath = Path.Combine(Application.dataPath,
            "ConsentManagementProvider/Plugins/iOS/Source/SwiftBridge.swift");
        var bridgeSource = File.ReadAllText(bridgeSourcePath);

        Assert.That(bridgeSource, Does.Not.Contain("extension SwiftBridge: SPDelegate"),
            "SwiftBridge's generated Objective-C header must not expose the pod-owned SPDelegate protocol.");
        Assert.That(bridgeSource, Does.Contain("private lazy var consentDelegate = ConsentDelegateAdapter(bridge: self)"),
            "SwiftBridge must strongly retain the private adapter because the native SDK retains its delegate weakly.");
        Assert.That(bridgeSource, Does.Contain("delegate: consentDelegate"),
            "The native consent manager must receive the retained private adapter.");
        Assert.That(bridgeSource, Does.Contain("private final class ConsentDelegateAdapter: NSObject, SPDelegate"),
            "Only the private adapter may conform to the pod-owned delegate protocol.");

        foreach (var callback in new[] {
            "bridge?.handleSPUIReady(controller)",
            "bridge?.handleAction(action, from: controller)",
            "bridge?.handleSPUIFinished(controller)",
            "bridge?.handleConsentReady(userData)",
            "bridge?.handleSPFinished(userData)",
            "bridge?.handleError(error)"
        })
        {
            Assert.That(bridgeSource, Does.Contain(callback),
                "The private delegate adapter must forward every native callback to SwiftBridge.");
        }
    }

    [Test]
    public void SwiftBridgeSerializesNativeErrorsWithStableFields()
    {
        var bridgeSourcePath = Path.Combine(Application.dataPath,
            "ConsentManagementProvider/Plugins/iOS/Source/SwiftBridge.swift");
        var bridgeSource = File.ReadAllText(bridgeSourcePath);
        var handleErrorStart = bridgeSource.IndexOf("fileprivate func handleError(_ error: SPError)", StringComparison.Ordinal);
        var delegateStart = bridgeSource.IndexOf("private final class ConsentDelegateAdapter", StringComparison.Ordinal);

        Assert.That(handleErrorStart, Is.GreaterThanOrEqualTo(0));
        Assert.That(delegateStart, Is.GreaterThan(handleErrorStart));

        var handleError = bridgeSource.Substring(handleErrorStart, delegateStart - handleErrorStart);
        Assert.That(handleError, Does.Not.Contain("error.toJSON()"),
            "SPError is an NSError, so NSObject JSON conversion can emit no payload.");
        Assert.That(handleError, Does.Contain("errorPayload(error)"),
            "The callback must use the bridge-owned error serializer.");
        Assert.That(bridgeSource, Does.Contain("\"code\": error.code"));
        Assert.That(bridgeSource, Does.Contain("\"spCode\": error.spCode"));
        Assert.That(bridgeSource, Does.Contain("\"description\": error.description"));
        Assert.That(bridgeSource, Does.Contain("\"failureReason\": error.failureReason"));
        Assert.That(bridgeSource, Does.Contain("\"campaignType\": String(describing: error.campaignType)"));
        Assert.That(bridgeSource, Does.Contain("JSONSerialization.data(withJSONObject: payload"),
            "The bridge must emit a JSON string even though SPError itself is not Codable.");
    }

    [Test]
    public void CmpImplementsTheSharedFacadeContract()
    {
        Assert.That(typeof(ICmpSdk).IsAssignableFrom(typeof(CMP)), Is.True);
    }

    [TestCase("ConsentManagementProvider.iOS.ConsentWrapperIOS")]
    [TestCase("ConsentManagementProvider.Android.ConsentWrapperAndroid")]
    [TestCase("ConsentManagementProvider.UnityEditor.ConsentWrapperUnityEditor")]
    public void PlatformWrapperImplementsTheSharedSdkContract(string typeName)
    {
        var wrapper = typeof(ISpSdk).Assembly.GetType(typeName);

        Assert.That(wrapper, Is.Not.Null, $"Missing platform wrapper {typeName}.");
        Assert.That(typeof(ISpSdk).IsAssignableFrom(wrapper), Is.True);
    }

    [Test]
    public void FacadeAddsOnlyTheExistingFourCampaignPropertiesToTheSdkContract()
    {
        var properties = typeof(ICmpSdk).GetProperties();
        Assert.That(properties.Select(property => property.Name),
            Is.EquivalentTo(new[] { "UseGDPR", "UseCCPA", "UseUSNAT", "UseIOS14" }));
        foreach (var property in properties)
        {
            Assert.That(property.PropertyType, Is.EqualTo(typeof(bool)));
            Assert.That(property.CanRead, Is.True);
            Assert.That(property.CanWrite, Is.False);
        }

        var expectedMethods = PublicInterfaceMethods(typeof(ISpSdk))
            .Concat(new[] { "System.Boolean get_UseGDPR()", "System.Boolean get_UseCCPA()",
                "System.Boolean get_UseUSNAT()", "System.Boolean get_UseIOS14()" });

        Assert.That(PublicInterfaceMethods(typeof(ICmpSdk)), Is.EquivalentTo(expectedMethods));
    }

    private static string[] PublicInterfaceMethods(Type contract)
    {
        // GetMethods on an interface does not include inherited interface methods.
        return contract.GetInterfaces().Concat(new[] { contract })
            .SelectMany(type => type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            .Select(method => $"{method.ReturnType} {method.Name}({string.Join(",", method.GetParameters().Select(parameter => parameter.ParameterType.ToString()))})")
            .Distinct()
            .ToArray();
    }
}
