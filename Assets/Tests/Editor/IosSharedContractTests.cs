using System;
using System.IO;
using System.Linq;
using System.Reflection;
using ConsentManagementProvider;
using NUnit.Framework;
using UnityEngine;

public class IosSharedContractTests
{
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
