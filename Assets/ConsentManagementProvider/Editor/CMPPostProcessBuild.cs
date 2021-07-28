using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor.iOS.Xcode.Extensions;
using Debug = UnityEngine.Debug;

public static class CMPPostProcessBuild
{
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string buildPath)
    {
        // const string defaultLocationInProj = "Plugins/iOS";
        // const string coreFrameworkName = "ConsentViewController.xcframework";
        if (buildTarget == BuildTarget.iOS)
        {
            PBXProject pbxProject = new PBXProject();
            string projPath = buildPath + "/Unity-Iphone.xcodeproj/project.pbxproj";
            pbxProject.ReadFromFile(projPath);
            string unityMainTargetGuid = pbxProject.GetUnityMainTargetGuid();
            
            ConfigureFrameworks(pbxProject, unityMainTargetGuid);
            TieBridgingHeader(pbxProject, unityMainTargetGuid);
            EnableCppModules(pbxProject);
            EnableObjectiveCExceptions(pbxProject);
            //LinkBinaryWithLibraries(pbxProject, unityMainGUID);
            // EnableSwift(pbxProject, targetGuid);

            pbxProject.WriteToFile(projPath);
        }
    }

    static void ConfigureFrameworks(PBXProject pbxProject, string targetGuid)
    {
        pbxProject.AddBuildProperty(targetGuid, "LD_RUNPATH_SEARCH_PATHS", "@executable_path/Frameworks $(PROJECT_DIR)/lib/$(CONFIGURATION) $(inherited)");
        pbxProject.AddBuildProperty(targetGuid, "FRAMERWORK_SEARCH_PATHS",
            "$(inherited) $(PROJECT_DIR) $(PROJECT_DIR)/Frameworks");
        pbxProject.AddBuildProperty(targetGuid, "DYLIB_INSTALL_NAME_BASE", "@rpath");
        pbxProject.AddBuildProperty(targetGuid, "LD_DYLIB_INSTALL_NAME",
            "@executable_path/../Frameworks/$(EXECUTABLE_PATH)");
    }
    
    static void TieBridgingHeader(PBXProject pbxProject, string targetGuid)
    {
        pbxProject.SetBuildProperty(targetGuid, "ENABLE_BITCODE", "NO");
        pbxProject.SetBuildProperty(targetGuid, "SWIFT_OBJC_BRIDGING_HEADER", "Libraries/Plugins/iOS/Source/UnityPlugin-Bridging-Header.h");
        pbxProject.SetBuildProperty(targetGuid, "SWIFT_OBJC_INTERFACE_HEADER_NAME", "UnityController.h");
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