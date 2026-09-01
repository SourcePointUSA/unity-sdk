namespace UnityAppiumTests;

internal static class TestRunParameterResolver
{
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
