# CMP Unity SDK Android CMP 7.15.13 Migration

## Goal

Update the Android dependency bundled by `CMP-Unity-SDK` from Android CMP
`7.10.1` to `7.15.13`, moving through `7.12.0` first. Preserve the existing
public Unity/C# API and make Android build and UI validation repeatable for
people and future agents.

`7.12.0` is the migration checkpoint because it introduces `mobile-core`
(`com.sourcepoint:core:0.1.4`) and its Ktor 3.0.0 graph. `7.15.13` is the
release target and includes `mobile-core:0.1.16` thread-safety improvements.

## Scope

### Must have

1. Rebuild the Android UI-test infrastructure as one reproducible,
   project-local command.
2. Establish a green `7.10.1` UI/build baseline.
3. Upgrade to `7.12.0`, resolve dependency/build/runtime gaps caused by the
   `mobile-core` and Ktor 3 transition, and pass the same gates.
4. Upgrade to `7.15.13` and pass the same gates without changing the current
   public Unity/C# API.

### Out of scope

Exposing Android APIs added after `7.12.0` is a separate feature. In
particular, Global CMP, Preferences, `dateCreated`, message inactivity, and
new Back/accessibility behavior must not block this migration. They require a
separate design that considers a matching future iOS API.

## Test infrastructure

### Supported local test target

- Unity: `6000.5.10f1` with its installed Android Build Support, SDK, NDK, and
  OpenJDK.
- Emulator: `CMP_Unity_API_37` (Google Play ARM 64 v8a system image).
- Desktop server: `/Applications/AltTesterDesktop.app`.
- Test project: `UI-TESTS/UI-TESTS.csproj` (`net8.0`, Appium and AltTester
  Driver).

The current suite has Android happy-flow coverage for the first layer,
Accept/Reject All, all three Privacy Managers, Save & Exit, Clear Data,
Auth ID, programmatic reject, custom consent, and message language.

### Deliverables

Stage 1 creates both:

- `UI-TESTS/run-android-ui-tests.sh`: the single supported command.
- `UI-TESTS/README.md`: an agent-facing and human-facing playbook. It states
  prerequisites, configuration variables, the command, failure triage, and
  the success criterion. It is intentionally project-local rather than a
  global Codex skill, because it depends on this Unity project, its AVD and
  its AltTester/Appium wiring.

The command accepts explicit environment configuration such as `UNITY_PATH`,
`CMP_ANDROID_AVD`, and `ALTTESTER_DESKTOP_PATH`; defaults may reflect the
known local setup but must be overridable.

### Command behavior

The command must:

1. Fail fast with actionable messages when Unity Android tools, the named AVD,
   AltTester Desktop, .NET 8, Appium, its `uiautomator2` driver, or its
   `altunity` plugin are unavailable.
2. Restore the UI-test project from `UI-TESTS.csproj`, then build/discover the
   test suite. Existing `obj` and `bin` artifacts are stale (2024) compared
   with the project dependency declarations (2025), so they cannot be used as
   a validity signal.
3. Produce a fresh AltTester-instrumented Android APK from Unity 6.5; never
   use the checked-in July 2025 APK as a test result.
4. Start or wait for the configured emulator, AltTester Desktop, and Appium
   using condition-based readiness checks rather than fixed sleep durations.
5. Install and exercise the fresh APK with the entire existing Android UI
   suite through `dotnet test`.
6. Resolve the ChromeDriver matching the Chrome/WebView version on the AVD.
   The checked-in ChromeDriver 103/113 binaries must not be treated as a
   universal dependency.
7. Always save Unity build logs, Appium logs, `adb logcat`, and test `.trx`
   results below `UI-TESTS/artifacts/`; return nonzero if any required step or
   test fails, and clean up processes it started.

## Dependency migration

### Dependency source of truth

`Assets/ConsentManagementProvider/Editor/SourcepointDependencies.xml` is the
declarative Android dependency source. External Dependency Manager resolves it
into the Android Resolver blocks in `Assets/Plugins/Android/mainTemplate.gradle`,
`Assets/Plugins/Android/settingsTemplate.gradle`,
`Assets/Plugins/Android/gradleTemplate.properties`, and its recorded state in
`ProjectSettings/AndroidResolverDependencies.xml`. Those generated/template
outputs must describe exactly the graph for the selected CMP version.

After each upgrade, resolve dependencies afresh and inspect the generated
output. No old `7.10.1` AAR or manually retained Ktor 2 JAR may coexist with
the resolved Ktor 3 graph. Do not upgrade External Dependency Manager unless
the existing resolver demonstrably blocks the migration.

### Stage 2: 7.12.0 checkpoint

1. Run and preserve the green `7.10.1` baseline using the new command.
2. Change Android CMP to `7.12.0` and regenerate resolver artifacts.
3. Confirm the graph contains the Ktor 3/mobile-core dependencies expected by
   `mobile-core:0.1.4`, with no Ktor 2 duplicates.
4. Build the Unity Android APK and run the complete Android UI suite.
5. Treat any failure here as a transition-specific product gap; fix it with a
   focused regression test before moving on.

### Stage 3: 7.15.13 target

1. Change Android CMP to `7.15.13` and regenerate resolver artifacts.
2. Verify that its `mobile-core:0.1.16` graph resolves cleanly.
3. Build a fresh APK and repeat the full UI command unchanged.
4. Review the C# bridge and sample app diff. Existing public C# types,
   signatures, callbacks, and documented behavior must not be intentionally
   removed or changed in this stage.
5. Update release documentation/changelog with the Android CMP version and
   the Ktor 3 compatibility boundary for Unity consumers.

## Verification gates

The required sequence is:

1. **Baseline gate:** fresh `7.10.1` Android build and all Android UI tests
   pass; artifacts are retained.
2. **Migration gate:** fresh `7.12.0` Android build, dependency sanity check,
   and the same UI suite pass.
3. **Release gate:** fresh `7.15.13` Android build, dependency sanity check,
   C# API compatibility review, and the same UI suite pass.

The runner's exit status and saved artifacts are the success signal for CI,
agents, and later sessions. A failure must report the failed readiness check,
build phase, or test name and point to its artifacts; it must not silently
retry an indeterminate state.

## Risks and decisions

- A consuming Unity application may already use Ktor 2. Gradle can select Ktor
  3 for the whole dependency graph or encounter incompatible constraints. This
  release intentionally adopts Ktor 3; supporting Ktor 2 in parallel is out
  of scope.
- Google Play emulator images can update Chrome, so static ChromeDriver pins
  are brittle. Runtime driver matching is mandatory.
- UI tests interact with remote CMP campaigns and native WebViews. The runner
  records diagnostics, while flaky test behavior is repaired only after a
  reproducible root-cause investigation.
- User-owned Unity 6 upgrade changes already present in the worktree remain
  outside this feature unless an explicit overlap is necessary.
