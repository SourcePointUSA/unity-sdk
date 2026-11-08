namespace UnityAppiumTests;

public sealed class IosTimeoutDiagnostics
{
    private readonly string directory;
    private readonly string testName;
    private readonly Func<string> source;
    private readonly Func<byte[]> screenshot;
    private readonly IReadOnlyDictionary<string, Func<string>> status;
    private readonly Action<string> attach;

    public IosTimeoutDiagnostics(string directory, string testName, Func<string> source,
        Func<byte[]> screenshot, IReadOnlyDictionary<string, Func<string>> status, Action<string> attach)
    {
        this.directory = directory;
        this.testName = testName;
        this.source = source;
        this.screenshot = screenshot;
        this.status = status;
        this.attach = attach;
    }

    public void Capture()
    {
        // Each probe is independent; unavailable infrastructure must not replace the timeout.
        try
        {
            Directory.CreateDirectory(directory);
            var safeName = new string(testName.Take(100).Select(c => char.IsLetterOrDigit(c) ? c : '_').ToArray());
            var prefix = Path.Combine(directory, $"ios-timeout-{safeName}-{Guid.NewGuid():N}");
            var report = new List<string>();
            Probe("native source", () => File.WriteAllText(prefix + ".xml", source()), prefix + ".xml");
            Probe("WDA screenshot", () => File.WriteAllBytes(prefix + ".png", screenshot()), prefix + ".png");
            foreach (var field in status)
            {
                try { report.Add($"{field.Key}: {field.Value()}"); }
                catch (Exception error) { report.Add($"{field.Key}: unavailable ({error.GetType().Name}: {error.Message})"); }
            }
            File.WriteAllLines(prefix + ".txt", report);
            TryAttach(prefix + ".txt");

            void Probe(string name, Action collect, string path)
            {
                try { collect(); TryAttach(path); }
                catch (Exception error) { report.Add($"{name}: unavailable ({error.GetType().Name}: {error.Message})"); }
            }

            void TryAttach(string path)
            {
                try { attach(path); }
                catch (Exception error) { report.Add($"attachment: unavailable ({error.GetType().Name}: {error.Message})"); }
            }
        }
        catch (Exception error)
        {
            Console.WriteLine($"iOS timeout diagnostics unavailable: {error.GetType().Name}: {error.Message}");
        }
    }
}

public sealed class IosDiagnosticWait : WebDriverWait
{
    private readonly IosTimeoutDiagnostics diagnostics;
    public IosDiagnosticWait(IWebDriver driver, TimeSpan timeout, IosTimeoutDiagnostics diagnostics)
        : base(driver, timeout) => this.diagnostics = diagnostics;

    public override TResult Until<TResult>(Func<IWebDriver, TResult> condition)
    {
        try { return base.Until(condition); }
        catch (WebDriverTimeoutException)
        {
            diagnostics.Capture();
            throw;
        }
    }
}
