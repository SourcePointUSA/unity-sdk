#if UNITY_IOS
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;
using UnityEditor.Build;
using ConsentManagementProvider;

public static class CMPPostProcessBuild
{    
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

        const string bridgePath = "Libraries/ConsentManagementProvider/Plugins/iOS/Source/SwiftBridge.swift";
        if (!File.Exists(Path.Combine(buildPath, bridgePath)))
        {
            throw new BuildFailedException("CMP requires exported SwiftBridge.swift at " + bridgePath);
        }
        string bridgeGuid = project.FindFileGuidByProjectPath(bridgePath);
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
