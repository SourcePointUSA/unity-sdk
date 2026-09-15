using System;
using System.IO;
using AltTester.AltTesterUnitySDK.Editor;
using UnityEditor;
#if !UNITY_2017
using UnityEditor.Build.Reporting;
#endif
using UnityEngine;

public static class AndroidUiTestBuild
{
    private const string OutputArgument = "-cmpUiTestApk";

    public static void Build()
    {
        var outputPath = ParseOutputPath(Environment.GetCommandLineArgs());
#if UNITY_2017
        throw new NotSupportedException("CMP UI test APK batch builds require Unity versions that return BuildReport.");
#else
        var previousBuildTarget = EditorUserBuildSettings.activeBuildTarget;
        var previousBuildTargetGroup = BuildPipeline.GetBuildTargetGroup(previousBuildTarget);
        var persistedConfiguration = LoadExistingConfiguration();
        var configuration = CreateInMemoryConfigurationCopy(persistedConfiguration);

        try
        {
            AltTesterEditorWindow.EditorConfiguration = configuration;

            if (!EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android))
            {
                throw new InvalidOperationException("Unable to switch the active build target to Android.");
            }

            configuration.platform = AltPlatform.Android;
            configuration.BuildLocationPath = Path.GetDirectoryName(outputPath);

            var report = AltBuilder.BuildGame(BuildTarget.Android, BuildTargetGroup.Android, outputPath: outputPath, useCurrentEditorConfiguration: true);
            LogBuildReport(outputPath, report);
            EnsureBuildSucceeded(outputPath, report);
        }
        finally
        {
            AltTesterEditorWindow.EditorConfiguration = persistedConfiguration;
            UnityEngine.Object.DestroyImmediate(configuration);

            if (previousBuildTarget != BuildTarget.NoTarget && previousBuildTargetGroup != BuildTargetGroup.Unknown)
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(previousBuildTargetGroup, previousBuildTarget);
            }
        }
#endif
    }

    public static string ParseOutputPath(string[] arguments)
    {
        for (var index = 0; index < arguments.Length; index++)
        {
            if (arguments[index] != OutputArgument)
            {
                continue;
            }

            if (index + 1 >= arguments.Length || string.IsNullOrWhiteSpace(arguments[index + 1]))
            {
                throw new ArgumentException($"{OutputArgument} requires an absolute .apk output path.");
            }

            var outputPath = arguments[index + 1];
            if (!Path.IsPathRooted(outputPath))
            {
                throw new ArgumentException($"{OutputArgument} must be an absolute output path.");
            }

            if (!string.Equals(Path.GetExtension(outputPath), ".apk", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException($"{OutputArgument} must name a .apk output file.");
            }

            return outputPath;
        }

        throw new ArgumentException($"Missing required {OutputArgument} <absolute-path-to.apk> argument.");
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
        Debug.Log($"CMP UI test APK: {outputPath}");
        Debug.Log($"CMP UI test build report: result={report.summary.result}, errors={report.summary.totalErrors}, warnings={report.summary.totalWarnings}, output={report.summary.outputPath}");
    }

    private static void EnsureBuildSucceeded(string outputPath, BuildReport report)
    {
        if (report.summary.result != BuildResult.Succeeded || report.summary.totalErrors != 0)
        {
            throw new InvalidOperationException($"CMP UI test APK build failed: result={report.summary.result}, errors={report.summary.totalErrors}.");
        }

        if (!File.Exists(outputPath) || new FileInfo(outputPath).Length == 0)
        {
            throw new InvalidOperationException($"CMP UI test APK build did not produce a non-empty APK at {outputPath}.");
        }
    }
#endif
}
