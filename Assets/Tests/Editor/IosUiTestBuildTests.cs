using System;
using System.IO;
using System.Reflection;
#if !UNITY_2017
using AltTester.AltTesterUnitySDK.Editor;
using UnityEditor;
using UnityEngine;
#endif
using NUnit.Framework;

public class IosUiTestBuildTests
{
#if !UNITY_2017
    [TestCase(iOSSdkVersion.DeviceSDK, AppleMobileArchitectureSimulator.X86_64, false)]
    [TestCase(iOSSdkVersion.DeviceSDK, AppleMobileArchitectureSimulator.ARM64, false)]
    [TestCase(iOSSdkVersion.DeviceSDK, AppleMobileArchitectureSimulator.Universal, false)]
    [TestCase(iOSSdkVersion.SimulatorSDK, AppleMobileArchitectureSimulator.X86_64, false)]
    [TestCase(iOSSdkVersion.SimulatorSDK, AppleMobileArchitectureSimulator.ARM64, false)]
    [TestCase(iOSSdkVersion.SimulatorSDK, AppleMobileArchitectureSimulator.Universal, false)]
    [TestCase(iOSSdkVersion.DeviceSDK, AppleMobileArchitectureSimulator.X86_64, true)]
    [TestCase(iOSSdkVersion.DeviceSDK, AppleMobileArchitectureSimulator.ARM64, true)]
    [TestCase(iOSSdkVersion.DeviceSDK, AppleMobileArchitectureSimulator.Universal, true)]
    [TestCase(iOSSdkVersion.SimulatorSDK, AppleMobileArchitectureSimulator.X86_64, true)]
    [TestCase(iOSSdkVersion.SimulatorSDK, AppleMobileArchitectureSimulator.ARM64, true)]
    [TestCase(iOSSdkVersion.SimulatorSDK, AppleMobileArchitectureSimulator.Universal, true)]
    public void UiExportUsesArm64SimulatorAndRestoresPlayerSettings(
        iOSSdkVersion originalSdk, AppleMobileArchitectureSimulator originalArchitecture, bool failExport)
    {
        var savedSdk = PlayerSettings.iOS.sdkVersion;
        var savedArchitecture = PlayerSettings.iOS.simulatorSdkArchitecture;
        try
        {
            PlayerSettings.iOS.sdkVersion = originalSdk;
            PlayerSettings.iOS.simulatorSdkArchitecture = originalArchitecture;
            var method = BuilderType().GetMethod("ExportForArm64Simulator", BindingFlags.NonPublic | BindingFlags.Static);
            Assert.That(method, Is.Not.Null, "The UI export must select ARM64 Simulator without changing customer build settings.");
            var invoked = false;
            Action export = () =>
            {
                invoked = true;
                Assert.That(PlayerSettings.iOS.sdkVersion, Is.EqualTo(iOSSdkVersion.SimulatorSDK));
                Assert.That(PlayerSettings.iOS.simulatorSdkArchitecture, Is.EqualTo(AppleMobileArchitectureSimulator.ARM64));
                if (failExport) throw new InvalidOperationException("export failure");
            };
            if (failExport)
            {
                var error = Assert.Throws<TargetInvocationException>(() => method.Invoke(null, new object[] { export }));
                Assert.That(error.InnerException, Is.TypeOf<InvalidOperationException>());
            }
            else method.Invoke(null, new object[] { export });
            Assert.That(invoked, Is.True);
            Assert.That(PlayerSettings.iOS.sdkVersion, Is.EqualTo(originalSdk));
            Assert.That(PlayerSettings.iOS.simulatorSdkArchitecture, Is.EqualTo(originalArchitecture));
        }
        finally
        {
            PlayerSettings.iOS.sdkVersion = savedSdk;
            PlayerSettings.iOS.simulatorSdkArchitecture = savedArchitecture;
        }
    }

    [TestCase(false)]
    [TestCase(true)]
    public void ExportDefersPodInstallAndRestoresResolverPreference(bool originalValue)
    {
        VerifyPodInstallScope(originalValue, false);
    }

    [TestCase(false)]
    [TestCase(true)]
    public void FailedExportRestoresResolverPreference(bool originalValue)
    {
        VerifyPodInstallScope(originalValue, true);
    }

    private static void VerifyPodInstallScope(bool originalValue, bool failExport)
    {
        var savedValue = Google.IOSResolver.SkipPodInstallWhenUsingWorkspaceIntegration;
        try
        {
            Google.IOSResolver.SkipPodInstallWhenUsingWorkspaceIntegration = originalValue;
            var method = BuilderType().GetMethod("ExportWithExternalPodInstall", BindingFlags.NonPublic | BindingFlags.Static);
            Assert.That(method, Is.Not.Null, "The UI export must defer pod install to the runner and restore resolver preferences.");
            var invoked = false;
            Action export = () =>
            {
                invoked = true;
                Assert.That(Google.IOSResolver.SkipPodInstallWhenUsingWorkspaceIntegration, Is.True);
                if (failExport) throw new InvalidOperationException("export failure");
            };
            if (failExport)
            {
                var error = Assert.Throws<TargetInvocationException>(() => method.Invoke(null, new object[] { export }));
                Assert.That(error.InnerException, Is.TypeOf<InvalidOperationException>());
            }
            else method.Invoke(null, new object[] { export });
            Assert.That(invoked, Is.True);
            Assert.That(Google.IOSResolver.SkipPodInstallWhenUsingWorkspaceIntegration, Is.EqualTo(originalValue));
        }
        finally
        {
            Google.IOSResolver.SkipPodInstallWhenUsingWorkspaceIntegration = savedValue;
        }
    }

#endif
    [Test]
    public void ParseOutputPathReturnsRequiredAbsoluteExportDirectory()
    {
        Assert.That(ParseOutputPath("-cmpUiTestXcodeProject", "/tmp/cmp-ios-export"),
            Is.EqualTo("/tmp/cmp-ios-export"));
    }

    [TestCase()]
    [TestCase("-batchmode")]
    [TestCase("-cmpUiTestXcodeProject")]
    [TestCase("-cmpUiTestXcodeProject", "")]
    [TestCase("-cmpUiTestXcodeProject", " ")]
    [TestCase("-cmpUiTestXcodeProject", "-batchmode")]
    [TestCase("-cmpUiTestXcodeProject", "build/cmp-ios-export")]
    [TestCase("-cmpUiTestXcodeProject", "/tmp/cmp-ios-export.app")]
    [TestCase("-cmpUiTestXcodeProject", "/tmp/cmp-ios-export.APP/")]
    [TestCase("-cmpUiTestXcodeProject", "/tmp/cmp-ios-export.app/.")]
    public void ParseOutputPathRejectsMissingRelativeOrAppOutput(params string[] arguments)
    {
        var exception = Assert.Throws<ArgumentException>(() => ParseOutputPath(arguments));

        Assert.That(exception.Message, Does.Contain("-cmpUiTestXcodeProject"));
    }

#if !UNITY_2017
    [Test]
    public void BatchTargetValidationAcceptsAnEditorStartedForIos()
    {
        ValidateBatchTarget(BuildTarget.iOS);
    }

    [TestCase(BuildTarget.Android)]
    [TestCase(BuildTarget.StandaloneOSX)]
    [TestCase(BuildTarget.NoTarget)]
    public void BatchTargetValidationRejectsOtherTargetsBeforeExport(BuildTarget target)
    {
        var exception = Assert.Throws<InvalidOperationException>(() => ValidateBatchTarget(target));
        Assert.That(exception.Message, Does.Contain("-buildTarget iOS"));
    }

    private static void ValidateBatchTarget(BuildTarget target)
    {
        var validator = BuilderType().GetMethod("ValidateBatchTarget", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.That(validator, Is.Not.Null, "Batch export must validate the launch target instead of switching in-process.");
        try { validator.Invoke(null, new object[] { target }); }
        catch (TargetInvocationException error) when (error.InnerException != null) { throw error.InnerException; }
    }

    [Test]
    public void ParseOutputPathRejectsAnExistingFileWithATrailingDirectorySeparator()
    {
        var existingFile = Path.GetFullPath(Path.Combine(Application.dataPath, "../ProjectSettings/ProjectVersion.txt"));
        Assert.That(File.Exists(existingFile), Is.True, "The project version file is the regression fixture.");

        var exception = Assert.Throws<ArgumentException>(() =>
            ParseOutputPath("-cmpUiTestXcodeProject", existingFile + Path.DirectorySeparatorChar));

        Assert.That(exception.Message, Does.Contain("-cmpUiTestXcodeProject"));
    }

    [Test]
    public void InMemoryIosConfigurationDoesNotChangeTheSavedAltTesterConfiguration()
    {
        var source = AssetDatabase.LoadAssetAtPath<AltEditorConfiguration>(
            "Assets/Plugins/AltTester/AltTesterEditorSettings.asset");
        Assert.That(source, Is.Not.Null, "The saved AltTester configuration must be available.");
        var savedConfiguration = EditorJsonUtility.ToJson(source);

        AltEditorConfiguration copy = null;
        try
        {
            var builder = BuilderType();
            var copyMethod = builder.GetMethod("CreateInMemoryConfigurationCopy", BindingFlags.NonPublic | BindingFlags.Static);
            Assert.That(copyMethod, Is.Not.Null, "The export builder must isolate configuration from the persisted asset.");
            copy = (AltEditorConfiguration)copyMethod.Invoke(null, new object[] { source });
            Assert.That(copy, Is.Not.SameAs(source));

            copy.platform = AltPlatform.iOS;
            copy.BuildLocationPath = "/tmp/cmp-ios-export";
            if (copy.Scenes.Count > 0)
            {
                copy.Scenes[0].ToBeBuilt = !copy.Scenes[0].ToBeBuilt;
            }

            Assert.That(copy.platform, Is.EqualTo(AltPlatform.iOS));
            Assert.That(EditorJsonUtility.ToJson(source), Is.EqualTo(savedConfiguration));
        }
        finally
        {
            if (copy != null)
            {
                UnityEngine.Object.DestroyImmediate(copy);
            }
        }
    }
#endif

    private static Type BuilderType()
    {
        var type = typeof(IosUiTestBuildTests).Assembly.GetType("IosUiTestBuild");
        Assert.That(type, Is.Not.Null, "IosUiTestBuild must provide the batch export builder.");
        return type;
    }

    private static string ParseOutputPath(params string[] arguments)
    {
        var parser = BuilderType().GetMethod("ParseOutputPath", BindingFlags.Public | BindingFlags.Static);
        Assert.That(parser, Is.Not.Null, "IosUiTestBuild must expose ParseOutputPath for pure argument validation.");
        try
        {
            return (string)parser.Invoke(null, new object[] { arguments });
        }
        catch (TargetInvocationException exception) when (exception.InnerException != null)
        {
            throw exception.InnerException;
        }
    }
}
