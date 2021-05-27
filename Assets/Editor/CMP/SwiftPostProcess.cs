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

public static class SwiftPostProcess
{
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string buildPath)
    {
        if (buildTarget == BuildTarget.iOS)
        {
            string projPath = buildPath + "/Unity-Iphone.xcodeproj/project.pbxproj";
            PBXProject pbxProject = new PBXProject();
            pbxProject.ReadFromFile(projPath);
            var targetGuid = pbxProject.TargetGuidByName(PBXProject.GetUnityTestTargetName());

            ConfigureFrameworks(pbxProject, targetGuid);
            SwiftBridgingHeader(pbxProject, targetGuid);
            EnableSwift(pbxProject, targetGuid);
            EnableCppModules(pbxProject);

            string unityMainGUID = pbxProject.GetUnityMainTargetGuid();
            const string defaultLocationInProj = "Plugins/iOS";
            const string coreFrameworkName = "ConsentViewController.framework";
            // string framework = Path.Combine(defaultLocationInProj, coreFrameworkName);
            // string fileGuid = pbxProject.AddFile(framework, "Frameworks/" + framework, PBXSourceTree.Sdk);
            ///// string fileGuid = pbxProject.AddFile(framework, "Frameworks/" + coreFrameworkName, PBXSourceTree.Sdk);

            // PBXProjectExtensions.AddFileToEmbedFrameworks(pbxProject, unityMainGUID, fileGuid);
            
            
            // PBXProjectExtensions.AddFileToCopyFilesWithSubfolder();
            // pbxProject.AddFile("/Users/wombatmbp17/Documents/Projects/iOS-CMP-Test/Frameworks/Plugins/iOS/Source", )
            
            //LinkBinaryWithLibraries(pbxProject, unityMainGUID);
            
            if(pbxProject.ContainsFramework(unityMainGUID, "ConsentViewController.framework" ))
            {
                Debug.LogWarning("Framewrok exists");
                
                Debug.LogWarning("LinkBinaryWithLibraries -> ...");

                // pbxProject.AddFileToEmbedFrameworks(unityMainGUID, "ConsentViewController.framework");
                Debug.LogWarning("AddFileToEmbedFrameworks -> ...");
            }
            
            pbxProject.WriteToFile(projPath);
        }
    }

    static void LinkBinaryWithLibraries(PBXProject pbxProject, string targetGuid)
    {
        //"true" will add the framework in the "Link Binary With Libraries" section with status "Optional", "false" will be "Required".
        pbxProject.AddFrameworkToProject(targetGuid, "ConsentViewController.framework", false);
        // pbxProject.AddFrameworksBuildPhase(targetGuid, "ConsentViewController.framework", false);
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
    
    static void SwiftBridgingHeader(PBXProject pbxProject, string targetGuid)
    {
        pbxProject.SetBuildProperty(targetGuid, "ENABLE_BITCODE", "NO");
        // pbxProject.SetBuildProperty(targetGuid, "SWIFT_OBJC_BRIDGING_HEADER", "Libraries/Plugins/iOS/Test_framework/Source/UnityPlugin-Bridging-Header.h");
        pbxProject.SetBuildProperty(targetGuid, "SWIFT_OBJC_BRIDGING_HEADER", "Libraries/Plugins/iOS/Source/UnityPlugin-Bridging-Header.h");
        // pbxProject.SetBuildProperty(targetGuid, "SWIFT_OBJC_INTERFACE_HEADER_NAME", "UnityIosPlugin-Swift.h");
        // pbxProject.SetBuildProperty(targetGuid, "SWIFT_OBJC_INTERFACE_HEADER_NAME", "Obc-C_Example-Swift.h");
        pbxProject.SetBuildProperty(targetGuid, "SWIFT_OBJC_INTERFACE_HEADER_NAME", "ConsentViewController-Swift.h");
    }
    
    static void EnableSwift(PBXProject pbxProject, string targetGuid)
    {
        pbxProject.AddBuildProperty(targetGuid, "DEFINES_MODULE", "YES");
        pbxProject.AddBuildProperty(targetGuid, "SWIFT_VERSION", "4.0");
        pbxProject.AddBuildProperty(targetGuid, "COREML_CODEGEN_LANGUAGE", "Swift");
        pbxProject.AddBuildProperty(targetGuid, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "YES");
    }
    
    static void EnableCppModules(PBXProject pbxProject)
    {
        pbxProject.AddBuildProperty(pbxProject.GetUnityMainTargetGuid(), "CLANG_ENABLE_MODULES", "YES");
        pbxProject.AddBuildProperty(pbxProject.GetUnityFrameworkTargetGuid(), "CLANG_ENABLE_MODULES", "YES");
    }

}