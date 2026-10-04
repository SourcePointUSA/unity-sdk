using System;
using System.IO;
using AltTester.AltTesterUnitySDK.Editor;
using UnityEditor;
#if !UNITY_2017
using UnityEditor.Build.Reporting;
#endif
using UnityEngine;

public static class IosUiTestBuild
{
    private const string OutputArgument = "-cmpUiTestXcodeProject";

    public static void Build()
    {
        var outputPath = ParseOutputPath(Environment.GetCommandLineArgs());
#if UNITY_2017
        throw new NotSupportedException("CMP UI test iOS batch exports require Unity versions that return BuildReport.");
#else
        var previousBuildTarget = EditorUserBuildSettings.activeBuildTarget;
        var previousBuildTargetGroup = BuildPipeline.GetBuildTargetGroup(previousBuildTarget);
        var persistedConfiguration = LoadExistingConfiguration();
        var configuration = CreateInMemoryConfigurationCopy(persistedConfiguration);

        try
        {
            AltTesterEditorWindow.EditorConfiguration = configuration;

            if (!EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS))
            {
                throw new InvalidOperationException("Unable to switch the active build target to iOS.");
            }

            configuration.platform = AltPlatform.iOS;
            configuration.BuildLocationPath = outputPath;

            var report = AltBuilder.BuildGame(BuildTarget.iOS, BuildTargetGroup.iOS, outputPath: outputPath, useCurrentEditorConfiguration: true);
            LogBuildReport(outputPath, report);
            EnsureBuildSucceeded(outputPath, report);
        }
        finally
        {
            AltTesterEditorWindow.EditorConfiguration = persistedConfiguration;
            try
            {
                UnityEngine.Object.DestroyImmediate(configuration);
            }
            finally
            {
                if (previousBuildTarget != BuildTarget.NoTarget && previousBuildTargetGroup != BuildTargetGroup.Unknown &&
                    !EditorUserBuildSettings.SwitchActiveBuildTarget(previousBuildTargetGroup, previousBuildTarget))
                {
                    throw new InvalidOperationException($"Unable to restore the active build target to {previousBuildTarget}.");
                }
            }
        }
#endif
    }

    public static string ParseOutputPath(string[] arguments)
    {
        if (arguments != null)
        {
            for (var index = 0; index < arguments.Length; index++)
            {
                if (arguments[index] != OutputArgument)
                {
                    continue;
                }

                if (index + 1 >= arguments.Length || string.IsNullOrWhiteSpace(arguments[index + 1]))
                {
                    throw new ArgumentException($"{OutputArgument} requires an absolute export directory.");
                }

                var outputPath = arguments[index + 1];
                if (!Path.IsPathRooted(outputPath))
                {
                    throw new ArgumentException($"{OutputArgument} must be an absolute export directory.");
                }

                var directoryPath = outputPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                if (string.Equals(Path.GetExtension(directoryPath), ".app", StringComparison.OrdinalIgnoreCase) || File.Exists(outputPath))
                {
                    throw new ArgumentException($"{OutputArgument} must name an Xcode export directory, not an application or file.");
                }

                return outputPath;
            }
        }

        throw new ArgumentException($"Missing required {OutputArgument} <absolute-export-directory> argument.");
    }

#if !UNITY_2017
    private static AltEditorConfiguration LoadExistingConfiguration()
    {
        var configurationGuids = AssetDatabase.FindAssets("AltTesterEditorSettings");
        if (configurationGuids.Length == 0)
        {
            throw new InvalidOperationException("Unable to find the existing AltTester editor configuration.");
        }

        var configuration = AssetDatabase.LoadAssetAtPath<AltEditorConfiguration>(AssetDatabase.GUIDToAssetPath(configurationGuids[0]));
        if (configuration == null)
        {
            throw new InvalidOperationException("Unable to load the existing AltTester editor configuration.");
        }

        return configuration;
    }

    private static AltEditorConfiguration CreateInMemoryConfigurationCopy(AltEditorConfiguration source)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        return UnityEngine.Object.Instantiate(source);
    }

    private static void LogBuildReport(string outputPath, BuildReport report)
    {
        Debug.Log($"CMP UI test Xcode export: {outputPath}");
        if (report == null)
        {
            throw new InvalidOperationException("CMP UI test iOS export did not return a build report.");
        }

        Debug.Log($"CMP UI test build report: result={report.summary.result}, errors={report.summary.totalErrors}, warnings={report.summary.totalWarnings}, output={report.summary.outputPath}");
    }

    private static void EnsureBuildSucceeded(string outputPath, BuildReport report)
    {
        if (report.summary.result != BuildResult.Succeeded || report.summary.totalErrors != 0)
        {
            throw new InvalidOperationException($"CMP UI test iOS export failed: result={report.summary.result}, errors={report.summary.totalErrors}.");
        }

        var projectPath = Path.Combine(outputPath, "Unity-iPhone.xcodeproj", "project.pbxproj");
        if (!File.Exists(projectPath) || new FileInfo(projectPath).Length == 0)
        {
            throw new InvalidOperationException($"CMP UI test iOS export did not produce a non-empty Xcode project at {projectPath}.");
        }
    }
#endif
}
