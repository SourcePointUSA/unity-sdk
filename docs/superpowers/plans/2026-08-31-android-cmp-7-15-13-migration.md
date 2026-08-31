# CMP Unity SDK Android CMP 7.15.13 Migration — Implementation Plan

> **Worker subskill prompt:** Execute this plan using `superpowers:subagent-driven-development` task-by-task. Keep work in `/Users/wombatmbp17/Documents/CMP-Unity-SDK` on `2026/first-wave-fixes`; preserve unrelated changes, especially `Assets/Plugins/AltTester/AltTesterEditorSettings.asset`. Use test-driven development for every behavior change, commit each completed task with the stated message, and run the verification commands before reporting success.

## Goal

Deliver a reproducible local Android UI validation pipeline and use it to migrate
the Unity SDK's Android CMP dependency from `7.10.1` through the Ktor 3/mobile-core
checkpoint `7.12.0` to `7.15.13`, without intentional public C# API regression.

## Architecture

`SourcepointDependencies.xml` remains the single dependency declaration. EDM4U
patches its version into the custom Unity Gradle templates. A small batch-mode
Unity entry point configures the already-installed AltTester SDK for a disposable
Android test build and fails on a real Unity `BuildReport` error. One shell runner
owns tool preflight, the fresh APK, AVD/Appium/AltTester lifecycle, ChromeDriver
selection, dependency-report capture, and the full .NET UI suite. The same runner
is the baseline, `7.12.0`, and final-release gate; version-specific graph checks
make the Ktor 3 transition observable.

## Tech Stack

Unity `6000.5.10f1`; installed Android SDK/NDK/OpenJDK; EDM4U `1.2.188` from
OpenUPM; Android Emulator `CMP_Unity_API_37`; AltTester Unity SDK `2.1.0` and
Desktop; .NET 8/NUnit/Appium `2.5.1` with `uiautomator2` and `altunity`; Gradle
dependency reports; POSIX shell.

## Spec Link

[`../specs/2026-08-31-android-cmp-7-15-13-migration-design.md`](../specs/2026-08-31-android-cmp-7-15-13-migration-design.md)

## Global Constraints

- Work only in `/Users/wombatmbp17/Documents/CMP-Unity-SDK`, branch `2026/first-wave-fixes`.
- Preserve the current C# bridge and all public types, method signatures,
  callbacks, and documented behavior; new Android APIs are expressly deferred.
- The dependency source is `Assets/ConsentManagementProvider/Editor/SourcepointDependencies.xml`.
  `Assets/Plugins/Android/*.gradle` and `ProjectSettings/AndroidResolverDependencies.xml`
  are EDM4U-generated/template state and must be updated only by a fresh resolve.
- Never use `UI-TESTS/BUILDS/Android/ConsentMessagePlugin.apk` or the committed
  ChromeDriver 103/113 files as validation input.
- Store transient test output below `UI-TESTS/artifacts/`, do not commit it, and
  clean up only processes spawned by the runner.
- Any failed gate stops the migration. Investigate with
  `superpowers:systematic-debugging`, add a focused regression test where
  appropriate, and do not advance to the next CMP version.

## Planned File Structure

```text
Assets/ConsentManagementProvider/Editor/
  AndroidUiTestBuild.cs                     # new Unity batch-mode APK builder
  AndroidUiTestBuildTests.cs                # new Editor tests for builder contract
Assets/AltTester/Editor/Scripts/AltBuilder.cs # returns BuildReport to caller
UI-TESTS/
  run-android-ui-tests.sh                   # new single supported runner
  assert-android-cmp-graph.sh               # new version-aware Gradle report check
  README.md                                  # new operating guide for humans/agents
  android.runsettings                        # dynamic, runner-overridable defaults
  UITests.cs                                 # no static ChromeDriver / fixed startup sleep
  .gitignore                                 # ignores artifacts and fresh APK output
Assets/ConsentManagementProvider/Editor/SourcepointDependencies.xml
Assets/Plugins/Android/mainTemplate.gradle
Assets/Plugins/Android/settingsTemplate.gradle
Assets/Plugins/Android/gradleTemplate.properties
ProjectSettings/AndroidResolverDependencies.xml
CHANGELOG.md
```

## Tasks

### 1. Make the AltTester Android build callable and failure-aware

**Files:**
- Create: `Assets/ConsentManagementProvider/Editor/AndroidUiTestBuild.cs`
- Create: `Assets/ConsentManagementProvider/Editor/AndroidUiTestBuildTests.cs`
- Modify: `Assets/AltTester/Editor/Scripts/AltBuilder.cs`

- [ ] Write Editor tests first for pure command-line argument parsing: `-cmpUiTestApk <absolute-path>` is required, a relative/missing path is rejected with an actionable exception, and `.apk` is enforced. Keep these helpers independent from a real Android build.
- [ ] Change `AltBuilder.BuildGameFromUI(...)` (or add a compatible overload) to return the actual Unity `BuildReport`; retain the existing UI caller behavior, but never swallow the report or a build exception from the batch-mode caller.
- [ ] Implement `AndroidUiTestBuild.Build()` as the `-executeMethod` target. It parses the output path, switches to `BuildTarget.Android`, loads the existing AltTester configuration, temporarily sets its Android platform and output directory, and invokes the report-returning AltTester build path.
- [ ] In `try/finally`, restore the selected build target and every changed AltTester configuration value. Do not persist or normalize `Assets/Plugins/AltTester/AltTesterEditorSettings.asset`; this protects the user's existing local configuration.
- [ ] Treat `BuildReport.summary.result != Succeeded`, non-zero `totalErrors`, or a missing/non-empty output APK as a thrown failure so Unity batch mode exits non-zero. Log the output APK path and report summary for the shell runner.
- [ ] Run the new EditMode tests from Unity in batch mode and run `git diff --check -- Assets/ConsentManagementProvider/Editor Assets/AltTester/Editor/Scripts`.
- [ ] Commit: `test(android): add batch-mode AltTester APK builder`.

### 2. Replace the legacy per-test launcher with one reproducible runner

**Files:**
- Create: `UI-TESTS/run-android-ui-tests.sh`
- Create: `UI-TESTS/.gitignore`
- Create: `UI-TESTS/README.md`
- Modify: `UI-TESTS/android.runsettings`
- Modify: `UI-TESTS/UITests.cs`

- [ ] Add `run-android-ui-tests.sh` with `set -eu`, `--help`, and overridable settings: `UNITY_PATH`, `UNITY_ANDROID_PLAYER_PATH`, `ANDROID_SDK_ROOT`, `CMP_ANDROID_AVD`, `ANDROID_SERIAL`, `ALTTESTER_DESKTOP_PATH`, `APPIUM_BIN`, and `CMP_UI_ARTIFACTS_DIR`.
- [ ] Default the known macOS setup to Unity `/Applications/Unity/Installs/6000.5.10f1/Unity.app/Contents/MacOS/Unity`, Android module `/Applications/Unity/Installs/6000.5.10f1/PlaybackEngines/AndroidPlayer`, AVD `CMP_Unity_API_37`, and `/Applications/AltTesterDesktop.app`; validate executable paths rather than assuming the SDK/NDK live inside `Unity.app`.
- [ ] Preflight `adb`, the named AVD, .NET SDK 8, Appium, installed `uiautomator2` driver, installed `altunity` plugin, AltTester Desktop, and Unity Android SDK/NDK/OpenJDK directories. Each failure must name the missing command/path and its remediation; no test process starts before all checks pass.
- [ ] Create one timestamped artifact directory containing `unity.log`, `appium.log`, `logcat.txt`, `gradle-dependencies.txt`, `results.trx`, and a copy of the effective test configuration. Register an EXIT trap that copies diagnostics and kills only emulator/Appium/logcat PIDs started by this invocation.
- [ ] Restore then discover the suite with `dotnet restore UI-TESTS/UI-TESTS.csproj` and `dotnet test --no-restore --list-tests`; never use stale `bin`/`obj` as a passing signal. Record the discovered test count before building.
- [ ] Start an existing target emulator or launch `emulator -avd "$CMP_ANDROID_AVD"`; use `adb wait-for-device` plus `sys.boot_completed` polling, not fixed sleeps. Start Appium only when its status endpoint is unavailable and wait for `/status`; open AltTester Desktop and wait for the AltTester connection port only after the APK is launched.
- [ ] Invoke Unity with `-batchmode -quit -nographics -projectPath "$PROJECT_ROOT" -executeMethod Sourcepoint.UnitySdk.Editor.AndroidUiTestBuild.Build -cmpUiTestApk "$ARTIFACT_DIR/ConsentMessagePlugin.apk" -logFile "$ARTIFACT_DIR/unity.log"`; fail if the expected fresh APK is absent.
- [ ] Replace the hard-coded ChromeDriver path. In `UITests.cs`, read an optional executable parameter only when supplied; otherwise set `appium:chromedriverAutodownload=true`. Start Appium with the narrowly-scoped `--allow-insecure chromedriver_autodownload` capability, so its Uiautomator2 driver downloads a Chrome/WebView-compatible driver for the AVD.
- [ ] Remove the unconditional ten-second sleep in `UITests.Setup`; rely on the runner readiness gates and the existing explicit WebDriver waits. Keep a finite Appium session timeout with diagnostic context.
- [ ] Pass the fresh APK, Android serial, AltTester host, and dynamic ChromeDriver setting as VSTest `TestRunParameters` overrides; run the whole assembly once with a TRX logger. Do not preserve `androidStartup.sh`'s silent, per-test retries.
- [ ] Validate script syntax with `sh -n UI-TESTS/run-android-ui-tests.sh`; execute `--help`; execute an intentional unavailable-AVD preflight and assert it fails before Unity/Appium launch.
- [ ] Commit: `test(android): add reproducible UI test runner`.

### 3. Add dependency-report and Ktor-version assertions

**Files:**
- Create: `UI-TESTS/assert-android-cmp-graph.sh`
- Modify: `UI-TESTS/run-android-ui-tests.sh`
- Modify: `UI-TESTS/README.md`

- [ ] After each fresh Unity APK build, locate the Unity 6 generated Gradle project beneath `Library/Bee/Android/Prj/*/Gradle`; select the one that contains both `gradlew` and `unityLibrary` and fail explicitly if none is found.
- [ ] Run `./gradlew :unityLibrary:dependencies --configuration releaseRuntimeClasspath` from that generated project and save stdout/stderr to `gradle-dependencies.txt`.
- [ ] Implement `assert-android-cmp-graph.sh <report> <cmp-version>` using fixed-string/regular-expression checks. It must reject legacy `Assets/Plugins/Android` CMP/Kotlin/Ktor AAR/JAR artifacts, require the selected `cmplibrary` coordinate, and require Ktor major version `3` for both migration versions.
- [ ] Make the `7.12.0` contract exact: require `com.sourcepoint:core:0.1.4` and the Ktor 3.0.x entries. Make the `7.15.13` contract exact for `com.sourcepoint:core:0.1.16` and Ktor major 3; print the matching report lines on success and the missing/forbidden coordinate on failure.
- [ ] Add shell fixture tests by feeding minimal good/bad dependency-report text into the assertion script, including a Ktor 2 duplicate and a missing mobile-core case. Run these tests without Unity.
- [ ] Extend the README with the three expected gate invocations and the artifact locations for Gradle conflict diagnosis.
- [ ] Commit: `test(android): assert CMP mobile-core dependency graph`.

### 4. Establish and preserve the 7.10.1 baseline

**Files:**
- Modify only if EDM4U refreshes them: `Assets/Plugins/Android/mainTemplate.gradle`, `Assets/Plugins/Android/settingsTemplate.gradle`, `Assets/Plugins/Android/gradleTemplate.properties`, `ProjectSettings/AndroidResolverDependencies.xml`
- Modify: `UI-TESTS/README.md`

- [ ] Confirm `Assets/ConsentManagementProvider/Editor/SourcepointDependencies.xml` still declares exactly `com.sourcepoint.cmplibrary:cmplibrary:7.10.1`.
- [ ] In Unity, use **Assets → External Dependency Manager → Android Resolver → Force Resolve** once; inspect the resolver blocks and `ProjectSettings/AndroidResolverDependencies.xml` to ensure they record `7.10.1` and no static CMP/Kotlin/Ktor libraries return to `Assets/Plugins/Android`.
- [ ] Run `UI-TESTS/run-android-ui-tests.sh` against the API 37 AVD. Require an APK built in this invocation, a saved Gradle report, and all discovered NUnit tests passing; retain artifacts locally but do not commit them.
- [ ] Record the date, Unity version, AVD API/image, discovered-test count, and the exact runner command in `UI-TESTS/README.md` as the baseline evidence. Do not claim a green baseline until the command's exit status is zero.
- [ ] Commit only intentional resolver-template/README changes: `test(android): record 7.10.1 UI baseline`.

### 5. Validate the mobile-core/Ktor 3 transition at 7.12.0

**Files:**
- Modify: `Assets/ConsentManagementProvider/Editor/SourcepointDependencies.xml`
- Modify only through EDM4U: `Assets/Plugins/Android/mainTemplate.gradle`, `Assets/Plugins/Android/settingsTemplate.gradle`, `Assets/Plugins/Android/gradleTemplate.properties`, `ProjectSettings/AndroidResolverDependencies.xml`
- Modify as required by a reproducible failure: `Assets/ConsentManagementProvider/Scripts/wrapper/Android/*.cs`, `UI-TESTS/*.cs`, or the runner/assertion scripts

- [ ] Change only the Android package coordinate to `com.sourcepoint.cmplibrary:cmplibrary:7.12.0`; keep the iOS pod at `7.7.1` and do not expose new Android API.
- [ ] Force-resolve with EDM4U, review the generated template block and resolver state, then run `UI-TESTS/assert-android-cmp-graph.sh` through the main runner. Require `mobile-core:0.1.4`, Ktor 3.0.x, and absence of Ktor 2/static artifacts before accepting the build.
- [ ] Run the full fresh-build UI command unchanged. If compilation, Gradle resolution, WebView, callbacks, or UI behavior fails, preserve its artifact folder; use systematic debugging to isolate the smallest cause and first add a focused regression test that fails on `7.10.1` or `7.12.0` as appropriate.
- [ ] Implement only the smallest bridge/runtime repair justified by that test. Re-run the focused test, full .NET UI suite, Gradle graph assertion, and Unity EditMode tests before proceeding.
- [ ] Review `git diff origin/develop -- Assets/ConsentManagementProvider/Scripts` to verify that public C# signatures have not changed. Document any implementation-only compatibility adaptation in the README's 7.12.0 checkpoint note.
- [ ] Commit: `build(android): migrate CMP to 7.12.0` (and a separate `fix(android): ...` commit first if a product-gap repair was required).

### 6. Move to 7.15.13 and close the release gate

**Files:**
- Modify: `Assets/ConsentManagementProvider/Editor/SourcepointDependencies.xml`
- Modify only through EDM4U: `Assets/Plugins/Android/mainTemplate.gradle`, `Assets/Plugins/Android/settingsTemplate.gradle`, `Assets/Plugins/Android/gradleTemplate.properties`, `ProjectSettings/AndroidResolverDependencies.xml`
- Modify: `CHANGELOG.md`
- Modify: `UI-TESTS/README.md`

- [ ] Change the same single XML coordinate from `7.12.0` to `7.15.13` and force-resolve. Confirm the resolver block names the target version and no old version appears in the project templates/state.
- [ ] Run the complete runner. Its graph assertion must require `mobile-core:0.1.16` and Ktor major 3; its Unity build and every existing Android UI test must pass from the fresh APK.
- [ ] Compare the public Android C# bridge (`Assets/ConsentManagementProvider/Scripts/wrapper/Android`) and the sample app against the baseline. Any changed public declaration is a regression unless it is an intentional, separately approved optional API feature; do not mix such exposure into this commit.
- [ ] Add a concise `CHANGELOG.md` entry recording Android CMP `7.15.13`, the mobile-core/Ktor 3 compatibility boundary for consuming Unity applications, and that the public Unity API was preserved. Update the README with the final green command and its local artifact convention.
- [ ] Run `git diff --check`, Unity EditMode tests, `sh -n` for both scripts, shell fixtures, and the complete `UI-TESTS/run-android-ui-tests.sh` release gate. Review the final diff for only intended files.
- [ ] Commit: `build(android): update CMP to 7.15.13`.

### 7. Handoff and future API boundary

**Files:**
- Modify: `docs/superpowers/specs/2026-08-31-android-cmp-7-15-13-migration-design.md` only if results reveal a durable constraint
- Create: a new dated spec under `docs/superpowers/specs/` only after this migration is green and a future API change is authorized

- [ ] Summarize baseline, 7.12.0, and 7.15.13 commands with artifact paths and test counts in the task handoff; link only the generated local artifacts needed to diagnose any remaining non-green gate.
- [ ] If `7.15.13` is green, create no bridge methods for Global CMP, Preferences, `dateCreated`, inactivity, Back, or accessibility. Capture them as a separately approved Android+iOS design follow-up instead.
- [ ] If a gate is not green, leave the final version unchanged and hand off the first failing command, failure phase, and artifact directory rather than masking it with retries.
- [ ] Commit documentation only when an observed constraint changes the migration spec: `docs: record Android CMP migration finding`.

## Final Verification Checklist

- [ ] The repository has no intentional public Unity/C# API removal or signature change.
- [ ] `SourcepointDependencies.xml`, EDM4U templates, and resolver state all name `7.15.13`.
- [ ] The saved dependency report proves `mobile-core:0.1.16` and Ktor 3 with no legacy local jars.
- [ ] Unity 6.5 builds a fresh AltTester-instrumented APK and all Android UI tests pass on `CMP_Unity_API_37`.
- [ ] The committed runner, README, and artifact behavior let a fresh agent repeat the success gate using one command.
