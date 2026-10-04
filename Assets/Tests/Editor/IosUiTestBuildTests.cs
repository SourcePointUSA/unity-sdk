using System;
using System.Reflection;
#if !UNITY_2017
using AltTester.AltTesterUnitySDK.Editor;
using UnityEditor;
using UnityEngine;
#endif
using NUnit.Framework;

public class IosUiTestBuildTests
{
    [Test]
    public void ParseOutputPathReturnsRequiredAbsoluteExportDirectory()
    {
        Assert.That(ParseOutputPath("-cmpUiTestXcodeProject", "/tmp/cmp-ios-export"),
            Is.EqualTo("/tmp/cmp-ios-export"));
    }

    [TestCase()]
    [TestCase("-batchmode")]
    [TestCase("-cmpUiTestXcodeProject")]
    [TestCase("-cmpUiTestXcodeProject", "")]
    [TestCase("-cmpUiTestXcodeProject", " ")]
    [TestCase("-cmpUiTestXcodeProject", "-batchmode")]
    [TestCase("-cmpUiTestXcodeProject", "build/cmp-ios-export")]
    [TestCase("-cmpUiTestXcodeProject", "/tmp/cmp-ios-export.app")]
    [TestCase("-cmpUiTestXcodeProject", "/tmp/cmp-ios-export.APP/")]
    public void ParseOutputPathRejectsMissingRelativeOrAppOutput(params string[] arguments)
    {
        var exception = Assert.Throws<ArgumentException>(() => ParseOutputPath(arguments));

        Assert.That(exception.Message, Does.Contain("-cmpUiTestXcodeProject"));
    }

#if !UNITY_2017
    [Test]
    public void InMemoryIosConfigurationDoesNotChangeTheSavedAltTesterConfiguration()
    {
        var source = AssetDatabase.LoadAssetAtPath<AltEditorConfiguration>(
            "Assets/Plugins/AltTester/AltTesterEditorSettings.asset");
        Assert.That(source, Is.Not.Null, "The saved AltTester configuration must be available.");
        var savedConfiguration = EditorJsonUtility.ToJson(source);

        AltEditorConfiguration copy = null;
        try
        {
            var builder = BuilderType();
            var copyMethod = builder.GetMethod("CreateInMemoryConfigurationCopy", BindingFlags.NonPublic | BindingFlags.Static);
            Assert.That(copyMethod, Is.Not.Null, "The export builder must isolate configuration from the persisted asset.");
            copy = (AltEditorConfiguration)copyMethod.Invoke(null, new object[] { source });
            Assert.That(copy, Is.Not.SameAs(source));

            copy.platform = AltPlatform.iOS;
            copy.BuildLocationPath = "/tmp/cmp-ios-export";
            if (copy.Scenes.Count > 0)
            {
                copy.Scenes[0].ToBeBuilt = !copy.Scenes[0].ToBeBuilt;
            }

            Assert.That(copy.platform, Is.EqualTo(AltPlatform.iOS));
            Assert.That(EditorJsonUtility.ToJson(source), Is.EqualTo(savedConfiguration));
        }
        finally
        {
            if (copy != null)
            {
                UnityEngine.Object.DestroyImmediate(copy);
            }
        }
    }
#endif

    private static Type BuilderType()
    {
        var type = typeof(IosUiTestBuildTests).Assembly.GetType("IosUiTestBuild");
        Assert.That(type, Is.Not.Null, "IosUiTestBuild must provide the batch export builder.");
        return type;
    }

    private static string ParseOutputPath(params string[] arguments)
    {
        var parser = BuilderType().GetMethod("ParseOutputPath", BindingFlags.Public | BindingFlags.Static);
        Assert.That(parser, Is.Not.Null, "IosUiTestBuild must expose ParseOutputPath for pure argument validation.");
        try
        {
            return (string)parser.Invoke(null, new object[] { arguments });
        }
        catch (TargetInvocationException exception) when (exception.InnerException != null)
        {
            throw exception.InnerException;
        }
    }
}
