#if UNITY_IOS
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor.Build;
using ConsentManagementProvider;

public static class CMPPostProcessBuild
{
    // EDM4U generates the Podfile at 40 and installs it at 50. Keep compatibility
    // settings in CocoaPods' supported hook so a later pod install retains them.
    [PostProcessBuild(45)]
    public static void ConfigurePodsBeforeInstall(BuildTarget buildTarget, string buildPath)
    {
        if (buildTarget != BuildTarget.iOS) return;
        string podfilePath = Path.Combine(buildPath, "Podfile");
        if (!File.Exists(podfilePath))
            throw new BuildFailedException("CMP requires EDM4U to generate a Podfile before installing iOS dependencies. Enable EDM4U Podfile generation and make a fresh Unity iOS export.");
        var project = new PBXProject();
        project.ReadFromFile(PBXProject.GetPBXProjectPath(buildPath));
        string floor = project.GetBuildPropertyForAnyConfig(project.GetUnityMainTargetGuid(), "IPHONEOS_DEPLOYMENT_TARGET");
        File.WriteAllText(podfilePath, ConfigureCmpPodfile(File.ReadAllText(podfilePath), floor));
    }

    static string ConfigureCmpPodfile(string podfile, string deploymentFloor)
    {
        if (string.IsNullOrEmpty(deploymentFloor) || !Regex.IsMatch(deploymentFloor, @"^\d+\.\d+(\.\d+)?$"))
            throw new BuildFailedException("CMP could not read a numeric iOS deployment target from the exported Unity app. Set the Unity iOS minimum version and make a fresh export.");
        const string begin = "# BEGIN Sourcepoint CMP deployment target";
        const string end = "# END Sourcepoint CMP deployment target";
        if (podfile.Contains(begin) || podfile.Contains(end))
        {
            var owned = Regex.Matches(podfile, Regex.Escape(begin) + @"[\s\S]*?" + Regex.Escape(end) + @"\r?\n?");
            if (owned.Count != 1 || Regex.Matches(podfile, Regex.Escape(begin)).Count != 1 || Regex.Matches(podfile, Regex.Escape(end)).Count != 1)
                throw new BuildFailedException("CMP found an incomplete or duplicate generated Podfile hook. Make a fresh Unity iOS export.");
            podfile = podfile.Remove(owned[0].Index, owned[0].Length);
        }
        // Reject customer hooks rather than registering a second CocoaPods hook
        // (which would silently replace one hook). Comment-only mentions are safe.
        string activeLines = Regex.Replace(podfile, @"(?m)^\s*#.*$", "");
        if (Regex.IsMatch(activeLines, @"\bpost_install\b"))
            throw new BuildFailedException("CMP cannot safely combine a custom Podfile post_install hook. Use a fresh EDM4U-generated Podfile, or integrate the generated Sourcepoint deployment-target logic into your single custom post_install hook and manage CocoaPods installation yourself.");
        return podfile.TrimEnd('\r', '\n') + "\n\n" + begin + "\n" +
            "post_install do |installer|\n" +
            "  cmp_floor = Gem::Version.new('" + deploymentFloor + "')\n" +
            "  installer.pods_project.targets.each do |target|\n" +
            "    next unless ['ConsentViewController', 'SPMobileCore', 'ConsentViewController-ConsentViewController'].include?(target.name)\n" +
            "    target.build_configurations.each do |config|\n" +
            "      current = Gem::Version.new(config.build_settings['IPHONEOS_DEPLOYMENT_TARGET'] || '0')\n" +
            "      config.build_settings['IPHONEOS_DEPLOYMENT_TARGET'] = cmp_floor.to_s if current < cmp_floor\n" +
            "    end\n" +
            "  end\n" +
            "end\n" + end + "\n";
    }

    [PostProcessBuild(800)]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string buildPath)
    {
        if (buildTarget == BuildTarget.iOS)
        {
            PBXProject pbxProject = new PBXProject();
            string projPath = PBXProject.GetPBXProjectPath(buildPath);

            pbxProject.ReadFromFile(projPath);
            ConfigureUnityBridge(pbxProject, buildPath);
            pbxProject.WriteToFile(projPath);

            string plistPath = buildPath + "/Info.plist";
            AddParameterToInfoPlist(plistPath);
        }
    }

    static void ConfigureUnityBridge(PBXProject project, string buildPath)
    {
        string mainTarget = project.GetUnityMainTargetGuid();
        string frameworkTarget = project.GetUnityFrameworkTargetGuid();
        if (string.IsNullOrEmpty(mainTarget) || string.IsNullOrEmpty(frameworkTarget))
        {
            throw new BuildFailedException("CMP requires Unity-iPhone and UnityFramework targets in the iOS export.");
        }

        string bridgePath = FindUnityBridgePath(buildPath);
        string bridgeGuid = project.FindFileGuidByRealPath(bridgePath);
        if (string.IsNullOrEmpty(bridgeGuid))
        {
            bridgeGuid = project.FindFileGuidByProjectPath(bridgePath);
        }
        if (string.IsNullOrEmpty(bridgeGuid))
        {
            bridgeGuid = project.AddFile(bridgePath, bridgePath, PBXSourceTree.Source);
        }
        project.RemoveFileFromBuild(mainTarget, bridgeGuid);
        project.AddFileToBuild(frameworkTarget, bridgeGuid);

        foreach (string target in new[] { mainTarget, frameworkTarget })
        {
            project.SetBuildProperty(target, "CLANG_ENABLE_MODULES", "YES");
            project.SetBuildProperty(target, "GCC_ENABLE_OBJC_EXCEPTIONS", "YES");
            project.SetBuildProperty(target, "ENABLE_BITCODE", "NO");
            project.AddBuildProperty(target, "LIBRARY_SEARCH_PATHS", "$(inherited)");
            project.AddBuildProperty(target, "LIBRARY_SEARCH_PATHS", "\"$(TOOLCHAIN_DIR)/usr/lib/swift/$(PLATFORM_NAME)\"");
            project.AddBuildProperty(target, "LIBRARY_SEARCH_PATHS", "\"/usr/lib/swift\"");
            project.AddBuildProperty(target, "LD_RUNPATH_SEARCH_PATHS", "$(inherited)");
            project.AddBuildProperty(target, "LD_RUNPATH_SEARCH_PATHS", "/usr/lib/swift");
        }

        project.SetBuildProperty(frameworkTarget, "DEFINES_MODULE", "YES");
        project.SetBuildProperty(frameworkTarget, "SWIFT_VERSION", "5.0");
        project.SetBuildProperty(frameworkTarget, "SWIFT_OBJC_INTERFACE_HEADER_NAME", "UnityFramework-Swift.h");
        project.SetBuildProperty(frameworkTarget, "SWIFT_INSTALL_OBJC_HEADER", "YES");
        project.SetBuildProperty(frameworkTarget, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "NO");
        project.SetBuildProperty(mainTarget, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "YES");
    }

    static string FindUnityBridgePath(string buildPath)
    {
        string[] candidates = {
            "Libraries/ConsentManagementProvider/Plugins/iOS/Source/SwiftBridge.swift",
            "Libraries/com.sourcepoint.unitycmp/Assets/ConsentManagementProvider/Plugins/iOS/Source/SwiftBridge.swift"
        };
        string found = null;
        foreach (string candidate in candidates)
        {
            if (!File.Exists(Path.Combine(buildPath, candidate))) continue;
            if (found != null)
            {
                throw new BuildFailedException("Multiple exported SwiftBridge.swift sources found: " + found + " and " + candidate);
            }
            found = candidate;
        }
        if (found == null)
        {
            throw new BuildFailedException("CMP requires exported SwiftBridge.swift in the asset or UPM layout: " + string.Join(", ", candidates));
        }
        return found;
    }

    static void AddParameterToInfoPlist(string plistPath)
    {
        PlistDocument plist = new PlistDocument();
        plist.ReadFromString(File.ReadAllText(plistPath));
        PlistElementDict rootDict = plist.root;
            
        var buildKey = "NSUserTrackingUsageDescription";
        rootDict.SetString(buildKey,"This identifier will be used to deliver personalized ads to you.");
        buildKey = "SPLogLevel";
        var buildValue="prod";
        if (CmpDebugUtil.IsLogging)
        {
            buildValue="debug";
        }
        rootDict.SetString(buildKey,buildValue);

        File.WriteAllText(plistPath, plist.WriteToString());
    }
    
}
#endif
