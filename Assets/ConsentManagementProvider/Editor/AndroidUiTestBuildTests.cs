using System;
using System.Reflection;
using NUnit.Framework;

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
}
