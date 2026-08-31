using System;
using System.Reflection;
using AltTester.AltTesterUnitySDK.Editor;
using NUnit.Framework;
using UnityEngine;

public class AndroidUiTestBuildTests
{
    [Test]
    public void ParseOutputPathReturnsRequiredAbsoluteApkPath()
    {
        const string outputPath = "/tmp/cmp-ui-test.apk";

        Assert.That(ParseOutputPath("-cmpUiTestApk", outputPath), Is.EqualTo(outputPath));
    }

    [TestCase()]
    [TestCase("-batchmode")]
    [TestCase("-cmpUiTestApk")]
    public void ParseOutputPathRejectsMissingRequiredOutputPath(params string[] arguments)
    {
        var exception = Assert.Throws<ArgumentException>(() => ParseOutputPath(arguments));

        Assert.That(exception.Message, Does.Contain("-cmpUiTestApk"));
    }

    [Test]
    public void ParseOutputPathRejectsRelativeOutputPath()
    {
        var exception = Assert.Throws<ArgumentException>(() => ParseOutputPath("-cmpUiTestApk", "build/cmp-ui-test.apk"));

        Assert.That(exception.Message, Does.Contain("absolute"));
    }

    [Test]
    public void ParseOutputPathRejectsNonApkOutputPath()
    {
        var exception = Assert.Throws<ArgumentException>(() => ParseOutputPath("-cmpUiTestApk", "/tmp/cmp-ui-test.aab"));

        Assert.That(exception.Message, Does.Contain(".apk"));
    }

    [Test]
    public void CreateInMemoryConfigurationCopyLeavesSourceConfigurationUntouched()
    {
        var source = ScriptableObject.CreateInstance<AltEditorConfiguration>();
        source.platform = AltPlatform.Standalone;
        source.BuildLocationPath = "/tmp/original-output";
        source.Scenes.Add(new AltMyScenes(true, "Assets/OriginalScene.unity", 3));

        AltEditorConfiguration copy = null;
        try
        {
            copy = CreateInMemoryConfigurationCopy(source);
            copy.platform = AltPlatform.Android;
            copy.BuildLocationPath = "/tmp/batch-output";
            copy.Scenes[0].ToBeBuilt = false;

            Assert.That(source.platform, Is.EqualTo(AltPlatform.Standalone));
            Assert.That(source.BuildLocationPath, Is.EqualTo("/tmp/original-output"));
            Assert.That(source.Scenes[0].ToBeBuilt, Is.True);
        }
        finally
        {
            if (copy != null)
            {
                UnityEngine.Object.DestroyImmediate(copy);
            }
            UnityEngine.Object.DestroyImmediate(source);
        }
    }

    private static string ParseOutputPath(params string[] arguments)
    {
        var type = typeof(AndroidUiTestBuildTests).Assembly.GetType("AndroidUiTestBuild");
        Assert.That(type, Is.Not.Null, "AndroidUiTestBuild must provide the batch build argument parser.");

        var parser = type.GetMethod("ParseOutputPath", BindingFlags.Public | BindingFlags.Static);
        Assert.That(parser, Is.Not.Null, "AndroidUiTestBuild must expose ParseOutputPath for pure argument validation.");

        try
        {
            return (string)parser.Invoke(null, new object[] { arguments });
        }
        catch (TargetInvocationException exception) when (exception.InnerException != null)
        {
            throw exception.InnerException;
        }
    }

    private static AltEditorConfiguration CreateInMemoryConfigurationCopy(AltEditorConfiguration source)
    {
        var type = typeof(AndroidUiTestBuildTests).Assembly.GetType("AndroidUiTestBuild");
        Assert.That(type, Is.Not.Null, "AndroidUiTestBuild must provide the batch build configuration copy.");

        var copyMethod = type.GetMethod("CreateInMemoryConfigurationCopy", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.That(copyMethod, Is.Not.Null, "AndroidUiTestBuild must isolate batch configuration from the persisted asset.");

        return (AltEditorConfiguration)copyMethod.Invoke(null, new object[] { source });
    }
}
