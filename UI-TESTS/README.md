# Android and iOS UI test runners

Both release runners build a fresh AltTester-instrumented app and retain their
configuration, discovery output, build logs, and test results in timestamped
artifact folders. Android instructions and checkpoint evidence follow below;
the iOS runner is documented at the end of this guide.

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

## Stage 2 CMP dependency-graph gates

Use these three gates during the Android CMP migration:

```sh
sh UI-TESTS/tests/assert-android-cmp-graph.test.sh
```

```sh
# After EDM4U resolves com.sourcepoint.cmplibrary:cmplibrary:7.12.0
UI-TESTS/run-android-ui-tests.sh
```

```sh
# After EDM4U resolves com.sourcepoint.cmplibrary:cmplibrary:7.15.14
UI-TESTS/run-android-ui-tests.sh
```

After every fresh Unity build, the runner writes
`gradle-dependencies.txt` in that run's timestamped `UI-TESTS/artifacts/<run>`
folder. It is the authoritative file for investigating Gradle version conflicts;
rerun its assertion directly when diagnosing a retained artifact:

```sh
UI-TESTS/assert-android-cmp-graph.sh UI-TESTS/artifacts/<run>/gradle-dependencies.txt 7.12.0
```

## 7.10.1 baseline acceptance

Stage 1 was accepted on 2026-09-03 using Unity `6000.5.10f1` and the
`CMP_Unity_API_37` AVD. The full command used was:

```sh
EMULATOR_BIN=/Users/wombatmbp17/Library/Android/sdk/emulator/emulator UI-TESTS/run-android-ui-tests.sh
```

It discovered 22 tests and produced a fresh APK in
`UI-TESTS/artifacts/20260903-144811`. The suite result was 21/22 passed;
`OpenPmLayersTest` timed out only while opening USNAT PM. An immediate isolated
retry passed 1/1 (`/private/tmp/open-pm-retry/results.trx`), so the remaining
failure was accepted as a sequential flaky result for the baseline checkpoint.

## 7.12.0 checkpoint acceptance

Stage 2 was accepted on 2026-09-08 with Android CMP `7.12.0`,
`mobile-core:0.1.4`, Ktor `3.0.0`, and
`kotlinx-coroutines-android:1.9.0`. The full command was:

```sh
EMULATOR_BIN=/Users/wombatmbp17/Library/Android/sdk/emulator/emulator \
ANDROID_SDK_ROOT=/Users/wombatmbp17/Library/Android/sdk \
DOTNET_BIN=/opt/homebrew/opt/dotnet@8/bin/dotnet \
UI-TESTS/run-android-ui-tests.sh
```

It discovered and passed all 22 Android UI tests on `CMP_Unity_API_37` during
the initial suite run; no retries or stable failures were recorded. Retain the
timestamped run folder under `UI-TESTS/artifacts/` with its APK, TRX,
`retry-summary.txt`, and `gradle-dependencies.txt` as the local checkpoint
evidence.

## 7.15.14 release gate

The final Android CMP `7.15.14` gate uses the same complete command:

```sh
EMULATOR_BIN=/Users/wombatmbp17/Library/Android/sdk/emulator/emulator \
ANDROID_SDK_ROOT=/Users/wombatmbp17/Library/Android/sdk \
DOTNET_BIN=/opt/homebrew/opt/dotnet@8/bin/dotnet \
UI-TESTS/run-android-ui-tests.sh
```

The dependency assertion requires `mobile-core:0.1.17` and Ktor `3.x` after a
fresh Unity Android build. The actual UI test result must be captured in its
timestamped run directory before a release is accepted.

Each local release-gate run is retained under the ignored
`UI-TESTS/artifacts/<YYYYMMDD-HHMMSS>/` convention. Keep the APK,
`gradle-dependencies.txt`, `retry-summary.txt`, and the initial/retry TRX files
together when handing off results; the accepted local run is
`UI-TESTS/artifacts/20260908-124608/`.

## iOS UI test runner

Run from the repository root on macOS:

```sh
UI-TESTS/run-ios-ui-tests.sh --preflight
UI-TESTS/run-ios-ui-tests.sh
```

Install Unity `6000.5.10f1` with **iOS Build Support**, Xcode with command-line
tools and an available iOS simulator runtime, CocoaPods, Python 3, .NET 8,
Appium with its `xcuitest` driver and AltTester Desktop.
The default simulator name is `iPhone 14`; name selection must find exactly
one available device across all runtimes. Select a UDID explicitly when names
are duplicated. `--preflight` checks selection and tools without booting or
launching anything.

For example, using the local Homebrew .NET 8 installation:

```sh
IOS_SIMULATOR_UDID=<available-simulator-udid> \
DOTNET_BIN=/opt/homebrew/opt/dotnet@8/bin/dotnet \
UI-TESTS/run-ios-ui-tests.sh
```

All iOS environment overrides are listed here and by `--help`:

| Variable | Default / purpose |
| --- | --- |
| `UNITY_PATH` | `/Applications/Unity/Installs/6000.5.10f1/Unity.app/Contents/MacOS/Unity` |
| `UNITY_LICENSING_IPC` | Empty; optionally names an existing licensing IPC channel for local editor/CI troubleshooting |
| `UNITY_IOS_PLAYER_PATH` | `/Applications/Unity/Installs/6000.5.10f1/PlaybackEngines/iOSSupport` |
| `IOS_SIMULATOR_UDID` | Empty; an explicit available UDID takes precedence over name selection |
| `CMP_IOS_SIMULATOR_NAME` | `iPhone 14`; exactly one available match required |
| `POD_BIN` | `pod` |
| `XCODEBUILD_BIN` | `xcodebuild` |
| `XCRUN_BIN` | `xcrun` |
| `APPIUM_BIN` | `appium` |
| `DOTNET_BIN` | `dotnet`; a .NET 8 SDK must be installed |
| `PYTHON_BIN` | `python3`; parses simulator JSON, plist data, and bounds readiness polling |
| `ALTTESTER_DESKTOP_PATH` | `/Applications/AltTesterDesktop.app` |
| `ALTTESTER_PORT` | `13000`; local readiness port |
| `CMP_UI_ARTIFACTS_DIR` | `UI-TESTS/artifacts`; accepts an absolute or relative parent path |

The runner restores and discovers the entire .NET project, invokes
`IosUiTestBuild.Build`, runs `pod install` in that fresh export, and builds
`Unity-iPhone.xcworkspace` for the selected simulator with signing disabled.
Unity starts with `-buildTarget iOS` so the batch editor imports for iOS before
executing the export method. The builder validates that target and restores the
persisted AltTester configuration after the build; it does not switch or restore
Unity targets during batch execution.
The export temporarily disables EDM4U's workspace-integrated pod install and
restores that preference in `finally`; the runner performs one explicit install
using `POD_BIN`. This keeps the selected CocoaPods environment effective even
when EDM4U discovers another installation (for example, RVM).
Unity's default licensing discovery remains enabled unless `UNITY_LICENSING_IPC`
is supplied. Use that override only when the local editor/CI environment needs
an already-running licensing channel; channel names are machine-specific.
Before building, it checks the actual resolved lockfile for
`ConsentViewController 7.12.11`, its exact `SPMobileCore (= 0.1.17)` dependency,
and installed `SPMobileCore 0.1.17`. It follows the Xcode target/source entries
to require `SwiftBridge.swift` in `UnityFramework`, and rejects a bridge beneath
`Pods/ConsentViewController`. The bridge remains under the Unity export’s
`Libraries` directory; `Unity.mm` imports `UnityFramework-Swift.h`, generated
by that owning target. The postprocess does not alter the Pods project.
Asset imports use
`Libraries/ConsentManagementProvider/Plugins/iOS/Source/SwiftBridge.swift`;
UPM imports use
`Libraries/com.sourcepoint.unitycmp/Assets/ConsentManagementProvider/Plugins/iOS/Source/SwiftBridge.swift`.
Exactly one existing source is required; its PBX file reference is reused when
available. A missing or duplicate bridge fails with the candidate paths named.
It boots the device only when needed and waits using `simctl bootstatus -b`.
It starts ordinary Appium, or reuses a ready local server at `127.0.0.1:4723`.
Only the installed `xcuitest` driver is checked. AltTester communication uses
the direct AltDriver connection configured by `altUnityHost` and `altUnityPort`,
not an Appium plugin.

After opening AltTester Desktop, the runner installs and launches the fresh
app, waits for the local AltTester port, and runs the entire .NET project once.
`ios.runsettings` retains iOS/xcuitest and the AltTester host; explicit
`appium:udid` and absolute `appium:app` command overrides select the fresh
`DerivedData/Build/Products/<configuration>-iphonesimulator/ConsentMessagePlugin.app`.
It does not use the historical `UI-TESTS/BUILDS` app. **iOS never silently
retries tests**; the initial full-suite exit status is the runner's status.
Cleanup stops only the Appium process and simulator booted by this invocation.
An already running simulator or external Appium server remains available.

Each full invocation creates `UI-TESTS/artifacts/<timestamp>-ios.<suffix>/`.
Logs and results appear only for stages reached:

```text
effective-test-config.txt, exit-status.txt, preflight.log, simulators.json
dotnet-restore.log, test-discovery.txt, ios.runsettings
Unity-iPhone/                 fresh Xcode export and original Podfile.lock
Podfile.lock                  copy of actual resolved CocoaPods lockfile
unity.log, unity-console.log, pod-install.log, xcodebuild.log
export-assertion.log          resolved versions and bridge ownership gate
DerivedData/                 fresh simulator app and Xcode outputs
appium.log, alttester-desktop.log, simulator.log
simctl-install.log, simctl-launch.log, dotnet-test.log, results.trx
```

`effective-test-config.txt` records runtime/UDID, discovered count, fresh app,
initial suite status, and invocation status. Later entries replace earlier
`not-run` or unresolved entries. A preflight failure retains its error and
exit status even though no test results or build output exist.

Verify the runner's external command contract without Unity or a simulator:

```sh
sh UI-TESTS/tests/run-ios-ui-tests.test.sh
sh UI-TESTS/tests/assert-ios-cmp-export.test.sh
sh UI-TESTS/tests/ios-bridge-logging.test.sh
sh UI-TESTS/tests/ios-clearall-lifecycle.test.sh
sh -n UI-TESTS/run-ios-ui-tests.sh
```

Rerun the export assertion against a retained fresh export when diagnosing an
upgrade or target-ownership failure:

```sh
UI-TESTS/assert-ios-cmp-export.sh UI-TESTS/artifacts/<run>/Unity-iPhone 7.12.11 0.1.17
```

`sh UI-TESTS/tests/ios-postprocess.test.sh` additionally compiles the iOS
postprocess with Unity’s bundled native Mono/compiler and PBX managed library.
It checks configuration behavior for both asset and UPM exports and validates
each serialized project, without claiming to export or compile a simulator app.
It requires the installed Unity managed libraries and an imported
Unity project (`Library/ScriptAssemblies/Assembly-CSharp.dll`). Override
`UNITY_CONTENTS_PATH` when Unity is installed elsewhere.

The logging check uses Xcode’s Swift compiler and Foundation to compile the
bridge’s actual logger declaration and print consumer without pod internals.
Unity-owned diagnostics read the app’s `SPLogLevel`: `debug` enables logging,
while production, missing, and invalid values disable it.

## iOS ClearAll lifecycle regression

`SPConsentManager.clearAllData()` clears persistent storage but does not reset
an already-live manager's in-memory coordinator. Therefore the Unity iOS bridge
rebuilds its manager from the last validated configuration immediately after
`ClearAllData()`. A subsequent `LoadMessage()` then observes the cleared
repository instead of reusing accepted consent state. This is iOS-only; it does
not change the public C# API or Android behavior.

Run `sh UI-TESTS/tests/ios-clearall-lifecycle.test.sh` to guard the bridge
invariant, then use a fresh simulator app to execute `ClearAllButtonTest`.

If Xcode cannot generate `UnityFramework-Swift.h`, import the CMP module, or
link the bridge, retain the artifact directory and diagnose the owning target’s
Swift/module settings. Do not move the bridge into Pods as a fallback. A passing
fixture gate requires a subsequent fresh export, Xcode build, launch, and initial
full UI suite to establish release acceptance.

## iOS 7.7.1 baseline attempt

On 2026-10-04, while `SourcepointDependencies.xml` still selected
`ConsentViewController` `7.7.1`, the command was:

```sh
UI-TESTS/run-ios-ui-tests.sh
```

The final attempt exited **1** at preflight: Unity `6000.5.10f1` lacks
`PlaybackEngines/iOSSupport` (only AndroidPlayer and MacStandaloneSupport
are installed). Evidence is retained in
`UI-TESTS/artifacts/20261004-215618-ios.2z4fyi/`. Simulator runtime and UDID
were not resolved; discovered count and suite status are `not-run`.
No fresh export, `Podfile.lock`, simulator `.app`, or TRX was produced. This is
an environment-blocked baseline attempt, not a green iOS baseline. Install
the matching Unity iOS module and rerun this command to obtain the initial
full-suite acceptance evidence.

## iOS 7.12.11 upgraded gate attempt

On 2026-10-04, after selecting `ConsentViewController 7.12.11` and moving
bridge configuration to `UnityFramework`, `UI-TESTS/run-ios-ui-tests.sh`
exited **1** at the same missing `PlaybackEngines/iOSSupport` preflight check.
Evidence is retained in `UI-TESTS/artifacts/20261004-221617-ios.K30yaK/`.
The export assertion fixtures, real Unity PBX configuration harness, and all
36 repository Unity EditMode tests passed. The actual export, resolved pod
graph, generated Swift header, Xcode link, fresh launch, and full iOS UI suite
remain unverified. Install the matching Unity iOS module and rerun the gate;
this attempt is not release acceptance.

## Paired release acceptance status

The declared pair is Android CMP `7.15.14` and iOS
`ConsentViewController 7.12.11` with expected resolved `SPMobileCore 0.1.17`.
The iOS migration remains **Unreleased**. There is no final successful iOS
command, accepted simulator identity, or completed iOS UI test count to record
yet. Local fixtures and EditMode tests do not establish native release acceptance.

Task 4 repeated the complete iOS command on 2026-10-05:

```sh
UI-TESTS/run-ios-ui-tests.sh
```

It exited **1** at the missing Unity `iOSSupport` check. The first failing
command's evidence is `UI-TESTS/artifacts/20261005-094809-ios.ejqZdK/`, including
`preflight.log`, `effective-test-config.txt`, and `exit-status.txt`. Runtime/UDID
remain unresolved; test discovery and the full suite are `not-run`. No export,
resolved lockfile, generated Swift header, simulator app, or TRX exists for
this attempt. An independent `xcodebuild -version` reported Xcode `27.0`
(`27A266a`); this identifies the installed tool, not a tested toolchain.

The repeated local checks had these results:

| Check | Observed result |
| --- | --- |
| iOS runner fixtures and export assertion fixtures | Passed |
| iOS runner shell syntax | Passed |
| `IosSharedContractTests;IosUiTestBuildTests` in Unity `6000.5.10f1` | 18/18 passed |
| `sp-consents-json-bridge.test.sh` | Blocked: Gradle cache lacks CMP `7.12.0`, core-android-debug `0.1.4`, Kotlin and serialization inputs |
| `sp-consents-json-bridge-7.15-compile.test.sh` | Blocked: Gradle cache lacks CMP `7.15.13` and Kotlin inputs |
| Fresh iOS export/build/launch/full UI gate | Blocked before export: Unity iOS Build Support missing |

The Android checks retain their existing checkpoint inputs and were not
rewritten to make this migration appear green. Historical Android acceptance
above remains separate from these missing-cache results.

Install the matching Unity iOS module, restore the Android checks' required
cached artifacts, and ensure the selected CocoaPods executable works before
rerunning the documented gates. A separate local `pod --version` check failed
in the current shell because system Ruby and the RVM gem paths were mixed;
an explicit RVM Ruby invocation then encountered an incompatible
`/usr/local/bin/git`. The iOS runner has not reached CocoaPods, so no CocoaPods
version or compatibility result is established. Fix the local Ruby/PATH setup
or select a working `POD_BIN`; this is an operator prerequisite, not a customer
Podfile workaround.

After a green initial full iOS run, record its exact command, Xcode/CocoaPods
versions, simulator runtime/UDID, discovered and passed counts, exit status,
and retained artifact folder here before publishing the pair. Keep the
`<timestamp>-ios.<suffix>/` export, actual `Podfile.lock`, logs, app, and TRX
together. No successful run or release claim may be inferred from an earlier
fixture result or a retry.

### Xcode 27 deployment-target compatibility (2026-10-05)

The resumed Unity 6.5 (`6000.5.10f1`) export resolves the expected pods and
exports the Unity app at iOS `15.0`. The initial Xcode 27 build rejected the
pods' declared iOS `10.0` deployment targets despite the generated Podfile
already declaring `platform :ios, '15.0'`. The package now emits a standard
CocoaPods `post_install` hook before EDM4U installation, aligning only CMP,
its resource bundle, and mobile-core targets to the actual exported app floor.
Higher floors and unrelated dependencies are preserved. A custom pre-existing
hook causes an explicit export failure. Run the behavioral fixture with:

```sh
sh UI-TESTS/tests/ios-podfile.test.sh
```

This fixture executes the generated Ruby and checks the resulting target
settings, idempotence, custom-hook rejection, and exported-floor updates;
it does not replace the real CocoaPods/Xcode/UI gate.

On the retained export `UI-TESTS/artifacts/20261005-215536-ios.WIIbe8/Unity-iPhone`,
ordinary Homebrew CocoaPods `1.16.2` installation with the generated hook
succeeded (exit 0). Inspection using the installed `xcodeproj` library confirms
all configurations of `ConsentViewController`,
`ConsentViewController-ConsentViewController`, and `SPMobileCore` now use
`15.0`. CocoaPods reported the existing app target's explicit
`ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES` override; the app continues to own
Swift runtime embedding. The resumed full build and UI result must be
recorded separately.
