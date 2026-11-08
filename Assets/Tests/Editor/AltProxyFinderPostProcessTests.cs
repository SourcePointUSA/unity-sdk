#if UNITY_IOS
using System;
using System.IO;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.iOS.Xcode;
using UnityEngine;
using UnityEngine.TestTools;

public class AltProxyFinderPostProcessTests
{
    [TestCase(false)]
    [TestCase(true)]
    public void HeaderExportSupportsUnityWithOrWithoutLegacySharedDecls(bool legacyHeaderPresent)
    {
        var root = Path.Combine(Path.GetTempPath(), "cmp-alt-header-" + Guid.NewGuid().ToString("N"));
        try
        {
            Directory.CreateDirectory(Path.Combine(root, "Unity-iPhone.xcodeproj"));
            Directory.CreateDirectory(Path.Combine(root, "UnityFramework"));
            var project = new PBXProject();
            project.ReadFromString(@"{
                archiveVersion = 1; objectVersion = 56;
                objects = {
                    A00000000000000000000010 = {isa = PBXProject; mainGroup = A00000000000000000000011; productRefGroup = A00000000000000000000012; buildConfigurationList = A00000000000000000000013; targets = (); };
                    A00000000000000000000011 = {isa = PBXGroup; children = (A00000000000000000000012,); sourceTree = ""<group>""; };
                    A00000000000000000000012 = {isa = PBXGroup; children = (); name = Products; sourceTree = ""<group>""; };
                    A00000000000000000000013 = {isa = XCConfigurationList; buildConfigurations = (A00000000000000000000014,); defaultConfigurationName = Release; };
                    A00000000000000000000014 = {isa = XCBuildConfiguration; buildSettings = {}; name = Release; };
                }; rootObject = A00000000000000000000010;
            }");
            project.AddTarget("Unity-iPhone", "app", "com.apple.product-type.application");
            var framework = project.AddTarget("UnityFramework", "framework", "com.apple.product-type.framework");
            project.AddHeadersBuildPhase(framework);
            var headers = legacyHeaderPresent
                ? new[] { "UnityInterface.h", "UnityForwardDecls.h", "UnityRendering.h", "UnitySharedDecls.h" }
                : new[] { "UnityInterface.h", "UnityForwardDecls.h", "UnityRendering.h" };
            foreach (var header in headers)
            {
                var file = project.AddFile("Classes/Unity/" + header, "Classes/Unity/" + header, PBXSourceTree.Source);
                project.AddFileToBuild(framework, file);
            }
            var projectPath = Path.Combine(root, "Unity-iPhone.xcodeproj/project.pbxproj");
            project.WriteToFile(projectPath);

            LogAssert.Expect(LogType.Log, "OnPostProcessBuild: iOS");
            LogAssert.Expect(LogType.Log, "OnPostProcessBuild: Complete");
            AltProxyFinderPostProcess.OnPostProcessBuild(BuildTarget.iOS, root);

            LogAssert.NoUnexpectedReceived();
            project.ReadFromFile(projectPath);
            var serialized = project.WriteToString();
            foreach (var header in headers)
                Assert.That(serialized, Does.Contain(header + " in Headers"), "Export must retain public bridge header " + header);
            Assert.That(serialized, Does.Contain("Public"), "Export must expose the bridge headers publicly.");
        }
        finally
        {
            if (Directory.Exists(root)) Directory.Delete(root, true);
        }
    }
}
#endif
