#if UNITY_IOS
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;
using UnityEditor.iOS.Xcode.Extensions;

public static class CMPPostProcessBuild
{
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string buildPath)
    {
        if (buildTarget == BuildTarget.iOS)
        {
            PBXProject pbxProject = new PBXProject();
            string projPath = buildPath + "/Unity-Iphone.xcodeproj/project.pbxproj";
            pbxProject.ReadFromFile(projPath);
            string unityProjectGuid = pbxProject.ProjectGuid();
            string unityMainTargetGuid = pbxProject.GetUnityMainTargetGuid();

            pbxProject.AddBuildProperty(unityMainTargetGuid, "LIBRARY_SEARCH_PATHS", "\"$(TOOLCHAIN_DIR)/usr/lib/swift/$(PLATFORM_NAME)\" \"/usr/lib/swift\"");
            pbxProject.AddBuildProperty(unityProjectGuid, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "YES");

            ConfigureFrameworks(pbxProject, unityProjectGuid);
            TieBridgingHeader(pbxProject, unityMainTargetGuid);
            EnableCppModules(pbxProject);
            EnableObjectiveCExceptions(pbxProject);
            EnableBitcode(pbxProject, unityProjectGuid, false);

            pbxProject.WriteToFile(projPath);
            
            string plistPath = buildPath + "/Info.plist";
            AddParameterToInfoPlist(plistPath);
        }
    }

    static void AddParameterToInfoPlist(string plistPath)
    {
        PlistDocument plist = new PlistDocument();
        plist.ReadFromString(File.ReadAllText(plistPath));
        PlistElementDict rootDict = plist.root;
            
        var buildKey = "NSUserTrackingUsageDescription";
        rootDict.SetString(buildKey,"This identifier will be used to deliver personalized ads to you.");

        File.WriteAllText(plistPath, plist.WriteToString());
    }
    
    static void ConfigureFrameworks(PBXProject pbxProject, string targetGuid)
    {
        pbxProject.AddBuildProperty(targetGuid, "LD_RUNPATH_SEARCH_PATHS", "/usr/lib/swift");
    }
    
    static void TieBridgingHeader(PBXProject pbxProject, string targetGuid)
    {
        pbxProject.SetBuildProperty(targetGuid, "SWIFT_OBJC_BRIDGING_HEADER", "Libraries/Plugins/iOS/Source/UnityPlugin-Bridging-Header.h");
        pbxProject.SetBuildProperty(targetGuid, "SWIFT_OBJC_INTERFACE_HEADER_NAME", "UnityController.h");
    }

    private static void EnableBitcode(PBXProject pbxProject, string targetGuid, bool enabled)
    {
        pbxProject.SetBuildProperty(targetGuid, "ENABLE_BITCODE", enabled ? "YES" : "NO");
    }

    static void EnableCppModules(PBXProject pbxProject)
    {
        pbxProject.AddBuildProperty(pbxProject.GetUnityMainTargetGuid(), "CLANG_ENABLE_MODULES", "YES");
        pbxProject.AddBuildProperty(pbxProject.GetUnityFrameworkTargetGuid(), "CLANG_ENABLE_MODULES", "YES");
    }

    static void EnableObjectiveCExceptions(PBXProject pbxProject)
    {
        pbxProject.SetBuildProperty (pbxProject.GetUnityMainTargetGuid(), "GCC_ENABLE_OBJC_EXCEPTIONS", "YES");
        pbxProject.SetBuildProperty (pbxProject.GetUnityFrameworkTargetGuid(), "GCC_ENABLE_OBJC_EXCEPTIONS", "YES");
    }
    
    static void LinkBinaryWithLibraries(PBXProject pbxProject, string targetGuid, string frameworkName)
    {
        //"true" will add the framework in the "Link Binary With Libraries" section with status "Optional", "false" will be "Required".
        pbxProject.AddFrameworkToProject(targetGuid, frameworkName, false);
    }
    
    static void EnableSwift(PBXProject pbxProject, string targetGuid)
    {
        pbxProject.AddBuildProperty(targetGuid, "DEFINES_MODULE", "YES");
        pbxProject.AddBuildProperty(targetGuid, "SWIFT_VERSION", "4.0");
        pbxProject.AddBuildProperty(targetGuid, "COREML_CODEGEN_LANGUAGE", "Swift");
        pbxProject.AddBuildProperty(targetGuid, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "YES");
    }
}
#endif
