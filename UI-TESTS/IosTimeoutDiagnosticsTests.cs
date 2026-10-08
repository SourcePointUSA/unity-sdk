namespace UnityAppiumTests;

[TestFixture]
public class IosTimeoutDiagnosticsTests
{
    private string directory = null!;

    [SetUp]
    public void SetUp() => directory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

    [TearDown]
    public void TearDown()
    {
        if (Directory.Exists(directory)) Directory.Delete(directory, true);
    }

    [Test]
    public void TimeoutRetainsOriginalExceptionAndWritesIndependentArtifacts()
    {
        var original = new WebDriverTimeoutException("native title missing");
        var attached = new List<string>();
        var diagnostics = new IosTimeoutDiagnostics(directory, "Clear/All", () => "<native />",
            () => new byte[] { 1, 2, 3 }, new Dictionary<string, Func<string>>
            {
                ["SDK"] = () => "SDK:Finished",
                ["consent"] = () => "accepted"
            }, attached.Add);
        var wait = new IosDiagnosticWait(new Castle.DynamicProxy.ProxyGenerator().CreateInterfaceProxyWithoutTarget<IWebDriver>(), TimeSpan.Zero, diagnostics);
        var caught = Assert.Throws<WebDriverTimeoutException>(() => wait.Until<bool>(_ => throw original));
        Assert.That(caught, Is.SameAs(original));
        Assert.That(attached, Has.Count.EqualTo(3));
        Assert.That(File.ReadAllText(attached.Single(p => p.EndsWith(".xml"))), Is.EqualTo("<native />"));
        Assert.That(File.ReadAllBytes(attached.Single(p => p.EndsWith(".png"))), Is.EqualTo(new byte[] { 1, 2, 3 }));
        Assert.That(File.ReadAllText(attached.Single(p => p.EndsWith(".txt"))), Does.Contain("SDK:Finished").And.Contain("accepted"));
        Assert.That(caught!.StackTrace, Does.Contain(nameof(TimeoutRetainsOriginalExceptionAndWritesIndependentArtifacts)));
    }

    [Test]
    public void BrokenSourceAndStatusStillRetainScreenshotAndFailureDetails()
    {
        var diagnostics = new IosTimeoutDiagnostics(directory, "test", () => throw new InvalidOperationException("source unavailable"),
            () => new byte[] { 4 }, new Dictionary<string, Func<string>>
            {
                ["SDK"] = () => throw new InvalidOperationException("AltTester disconnected"),
                ["GDPR"] = () => "default"
            }, _ => throw new InvalidOperationException("attachment unavailable"));
        Assert.DoesNotThrow(diagnostics.Capture);
        Assert.That(Directory.GetFiles(directory, "*.png"), Has.Length.EqualTo(1));
        Assert.That(File.ReadAllText(Directory.GetFiles(directory, "*.txt").Single()),
            Does.Contain("source unavailable").And.Contain("AltTester disconnected").And.Contain("default"));
    }

    [Test]
    public void RepeatedCapturesCannotOverwriteEarlierEvidence()
    {
        var diagnostics = new IosTimeoutDiagnostics(directory, "same test", () => "native", () => new byte[] { 1 },
            new Dictionary<string, Func<string>>(), _ => { });
        diagnostics.Capture();
        diagnostics.Capture();
        Assert.That(Directory.GetFiles(directory), Has.Length.EqualTo(6));
    }

    [Test]
    public void SuccessfulWaitDoesNotCollectDiagnostics()
    {
        var diagnostics = new IosTimeoutDiagnostics(directory, "test", () => throw new Exception(),
            () => throw new Exception(), new Dictionary<string, Func<string>>(), _ => { });
        var wait = new IosDiagnosticWait(new Castle.DynamicProxy.ProxyGenerator().CreateInterfaceProxyWithoutTarget<IWebDriver>(), TimeSpan.Zero, diagnostics);
        Assert.That(wait.Until(_ => true), Is.True);
        Assert.That(Directory.Exists(directory), Is.False);
    }

    [Test]
    public void UnwritableArtifactPathCannotReplaceTimeout()
    {
        Directory.CreateDirectory(directory);
        var blockedPath = Path.Combine(directory, "file");
        File.WriteAllText(blockedPath, "occupied");
        var diagnostics = new IosTimeoutDiagnostics(blockedPath, "test", () => "native", () => new byte[] { 1 },
            new Dictionary<string, Func<string>>(), _ => { });
        var wait = new IosDiagnosticWait(new Castle.DynamicProxy.ProxyGenerator().CreateInterfaceProxyWithoutTarget<IWebDriver>(), TimeSpan.Zero, diagnostics);
        var original = new WebDriverTimeoutException("original");
        Assert.That(Assert.Throws<WebDriverTimeoutException>(() => wait.Until<bool>(_ => throw original)), Is.SameAs(original));
    }
}
