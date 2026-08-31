using System;
using System.IO;
using AltTester.AltTesterUnitySDK.Editor;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class AndroidUiTestBuild
{
    private const string OutputArgument = "-cmpUiTestApk";

    public static void Build()
    {
        var outputPath = ParseOutputPath(Environment.GetCommandLineArgs());
        var previousBuildTarget = EditorUserBuildSettings.activeBuildTarget;
        var previousBuildTargetGroup = BuildPipeline.GetBuildTargetGroup(previousBuildTarget);

        AltTesterEditorWindow.InitEditorConfiguration();
        var configuration = AltTesterEditorWindow.EditorConfiguration;
        if (configuration == null)
        {
            throw new InvalidOperationException("Unable to load the AltTester editor configuration.");
        }

        var previousPlatform = configuration.platform;
        var previousBuildLocationPath = configuration.BuildLocationPath;

        try
        {
            if (!EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android))
            {
                throw new InvalidOperationException("Unable to switch the active build target to Android.");
            }

            configuration.platform = AltPlatform.Android;
            configuration.BuildLocationPath = Path.GetDirectoryName(outputPath);

            var report = AltBuilder.BuildGame(BuildTarget.Android, BuildTargetGroup.Android, outputPath: outputPath);
            LogBuildReport(outputPath, report);
            EnsureBuildSucceeded(outputPath, report);
        }
        finally
        {
            configuration.platform = previousPlatform;
            configuration.BuildLocationPath = previousBuildLocationPath;

            if (previousBuildTarget != BuildTarget.NoTarget && previousBuildTargetGroup != BuildTargetGroup.Unknown)
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(previousBuildTargetGroup, previousBuildTarget);
            }
        }
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
}
