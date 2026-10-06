namespace UnityAppiumTests;

internal static class TestRunParameterResolver
{
    public static void ApplyAndroidDeviceSelection(
        OpenQA.Selenium.Appium.AppiumOptions options,
        string? deviceName,
        string? udid)
    {
        options.DeviceName = string.IsNullOrWhiteSpace(deviceName) ? "Android Emulator" : deviceName;
        if (!string.IsNullOrWhiteSpace(udid))
        {
            options.AddAdditionalAppiumOption("appium:udid", udid);
        }
    }

    public static int ResolveIosAltUnityPort(
        string? host,
        string? port)
    {
        if (string.IsNullOrWhiteSpace(host))
        {
            throw new ArgumentException("A non-empty altUnityHost test parameter is required.", nameof(host));
        }

        if (!int.TryParse(port, out var parsedPort) || parsedPort is < 1 or > 65535)
        {
            throw new ArgumentException("A valid altUnityPort test parameter is required.", nameof(port));
        }

        return parsedPort;
    }

    public static void ApplyIosHeadlessSimulatorMode(
        OpenQA.Selenium.Appium.AppiumOptions options)
    {
        options.AddAdditionalAppiumOption("appium:isHeadless", true);
        options.AddAdditionalAppiumOption("appium:waitForQuiescence", false);
        options.AddAdditionalAppiumOption("appium:waitForIdleTimeout", 0);
    }

    public static string ResolveAppPath(string? appPath, string testRoot)
    {
        if (string.IsNullOrWhiteSpace(appPath))
        {
            throw new ArgumentException("A non-empty appium:app test parameter is required.", nameof(appPath));
        }

        if (appPath.StartsWith("/BUILDS/", StringComparison.Ordinal))
        {
            return testRoot + appPath;
        }

        return Path.IsPathRooted(appPath) ? appPath : Path.Combine(testRoot, appPath);
    }

    public static bool UseChromeDriverAutodownload(string? chromeDriverExecutable)
    {
        return string.IsNullOrWhiteSpace(chromeDriverExecutable);
    }
}
