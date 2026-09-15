namespace UnityAppiumTests;

[TestFixture]
public class TestRunParameterResolverTests
{
    [Test]
    public void ResolveAppPathKeepsAbsoluteApkPath()
    {
        Assert.That(TestRunParameterResolver.ResolveAppPath("/tmp/artifacts/ConsentMessagePlugin.apk", "/repo/UI-TESTS"),
            Is.EqualTo("/tmp/artifacts/ConsentMessagePlugin.apk"));
    }

    [Test]
    public void ResolveAppPathPrefixesLegacyRelativePath()
    {
        Assert.That(TestRunParameterResolver.ResolveAppPath("/BUILDS/Android/ConsentMessagePlugin.apk", "/repo/UI-TESTS"),
            Is.EqualTo("/repo/UI-TESTS/BUILDS/Android/ConsentMessagePlugin.apk"));
    }

    [Test]
    public void ResolveAppPathRejectsMissingPath()
    {
        Assert.That(() => TestRunParameterResolver.ResolveAppPath(null, "/repo/UI-TESTS"),
            Throws.ArgumentException.With.Message.Contains("appium:app"));
    }

    [Test]
    public void UseChromeDriverAutodownloadWhenNoExecutableWasSupplied()
    {
        Assert.That(TestRunParameterResolver.UseChromeDriverAutodownload(string.Empty), Is.True);
    }

    [Test]
    public void DoNotAutodownloadWhenExecutableWasSupplied()
    {
        Assert.That(TestRunParameterResolver.UseChromeDriverAutodownload("/tmp/chromedriver"), Is.False);
    }

    [Test]
    public void ApplyAndroidDeviceSelectionUsesUdidAndHumanReadableDeviceName()
    {
        var options = new OpenQA.Selenium.Appium.AppiumOptions();

        TestRunParameterResolver.ApplyAndroidDeviceSelection(
            options,
            "Android Emulator",
            "emulator-5588");

        var capabilities = options.ToCapabilities();
        Assert.Multiple(() =>
        {
            Assert.That(capabilities.GetCapability("appium:deviceName"), Is.EqualTo("Android Emulator"));
            Assert.That(capabilities.GetCapability("appium:udid"), Is.EqualTo("emulator-5588"));
        });
    }

    [Test]
    public void ApplyAndroidDeviceSelectionOmitsBlankOptionalUdid()
    {
        var options = new OpenQA.Selenium.Appium.AppiumOptions();

        TestRunParameterResolver.ApplyAndroidDeviceSelection(options, "Android Emulator", string.Empty);

        Assert.That(options.ToCapabilities().GetCapability("appium:udid"), Is.Null);
    }
}
