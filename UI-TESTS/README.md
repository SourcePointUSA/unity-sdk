# Android UI test runner

Run the Android suite from the repository root:

```sh
UI-TESTS/run-android-ui-tests.sh
```

The runner builds a fresh AltTester-instrumented APK, starts or reuses the
`CMP_Unity_API_37` emulator, and runs the whole .NET suite once. It restores
NuGet packages and discovers tests before building, so cached `bin` and `obj`
outputs are never treated as a passing result.

Use `UI-TESTS/run-android-ui-tests.sh --preflight` to validate local tooling
without launching Unity, Appium, or an emulator. The documented defaults are
Unity `6000.5.10f1`, its Android module, `CMP_Unity_API_37`, and
`/Applications/AltTesterDesktop.app`. Override any environment setting shown
by `--help`, for example `ANDROID_SERIAL` to target an existing emulator.

On this macOS setup, use the Homebrew .NET 8 installation explicitly when an
older SDK appears first in `PATH`:

```sh
DOTNET_BIN=/opt/homebrew/opt/dotnet@8/bin/dotnet UI-TESTS/run-android-ui-tests.sh
```

Each run writes a timestamped folder beneath `UI-TESTS/artifacts` containing
the fresh APK, Unity/Appium/logcat logs, test discovery, TRX results, copied
runsettings, and `effective-test-config.txt`. These files are intentionally
ignored by Git and are the first place to inspect a failing run.
