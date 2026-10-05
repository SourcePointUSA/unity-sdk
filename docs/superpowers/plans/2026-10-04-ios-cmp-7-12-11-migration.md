# iOS CMP 7.12.11 Migration Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Ship a verified Android `7.15.14` + iOS `ConsentViewController 7.12.11` release pair without changing the Unity consumer API or requiring customer workarounds.

**Architecture:** The iOS pod remains declared through EDM4U, while the Unity package keeps ownership of its Swift/C++ bridge and never modifies CocoaPods-owned projects. A project-local iOS runner exports a fresh app, resolves pods, checks the exact dependency graph, builds it for a simulator, and runs the existing UI suite once with retained diagnostics.

**Tech Stack:** Unity `6000.5.10f1`, EDM4U/CocoaPods, Xcode/`xcrun simctl`, iOS Simulator, AltTester `2.1.0`, Appium `xcuitest` + `altunity`, .NET 8/NUnit.

**Spec:** `docs/superpowers/specs/2026-10-04-ios-cmp-7-12-11-migration-design.md`

## Global Constraints

- Preserve `ISpSdk`, `ICmpSdk`, `CMP`, all existing C# public signatures, callbacks, and consent JSON models; platform implementations must continue to satisfy the shared contract.
- Publish and test the native pair Android CMP `7.15.14`, iOS `ConsentViewController 7.12.11`, and resolved iOS `SPMobileCore 0.1.17`.
- Do not raise the existing Unity iOS deployment target `13.0`, add new Global CMP/Preferences APIs, or change Android dependencies/implementation.
- `SwiftBridge.swift` stays Unity-package-owned. No production code may move it through a shell command, write inside `Pods/`, or edit `Pods.xcodeproj`.
- A customer needs no C# change, manual Podfile edit, or SDK-specific CocoaPods workaround. Record every client-facing constraint in root `README.md`.
- A custom customer Podfile with an incompatible native pin must fail through ordinary CocoaPods resolution; do not work around or hide the conflict.
- The iOS gate uses a fresh export and one complete test execution. It may not use a hard-coded simulator UDID, `osascript`, fixed startup sleeps, or silent retries.
- Keep per-run apps, DerivedData, logs, lockfiles, and TRX files below ignored `UI-TESTS/artifacts/`; do not commit them.
- If a gate fails, retain the first failing command and its artifact folder, use systematic debugging, and leave the final iOS pin unchanged until it passes.

## Review Focus

- A clean customer export must resolve exactly `ConsentViewController 7.12.11` and `SPMobileCore 0.1.17`; Task 3 pins this from `Podfile.lock`.
- The exported bridge must be in a Unity-owned target and never refer to `Pods/ConsentViewController/.../SwiftBridge.swift`; Task 3 fixture and fresh-export assertions cover it.
- A simulator can be selected dynamically and reaches `Booted` without a UI app/window or fixed wait; Task 2 exercises the runner's `simctl` readiness path with fixtures.
- Each pre-existing consent UI workflow must run from the fresh `.app`, not the committed historical app; Task 4 runs and records the complete .NET suite once.
- Android's shared-side compatibility adapter must remain valid while iOS changes; Task 4 runs the existing `SpConsentsJsonBridge` regression and 7.15 compile checks unchanged.

---

### Task 1: Lock the shared C# contract and add a failure-aware iOS export entry point

**Files:**
- Create: `Assets/Tests/Editor/IosSharedContractTests.cs`
- Create: `Assets/Tests/Editor/IosUiTestBuild.cs`
- Create: `Assets/Tests/Editor/IosUiTestBuildTests.cs`

**Interfaces:**
- Consumes: `ISpSdk`, `ICmpSdk`, `CMP`, `ConsentWrapperIOS`, `ConsentWrapperAndroid`, `ConsentWrapperUnityEditor`, and AltTester `AltBuilder.BuildGame(...)`.
- Produces: `IosUiTestBuild.Build()` as Unity batch-mode target and `IosUiTestBuild.ParseOutputPath(string[] arguments) -> string` for `-cmpUiTestXcodeProject <absolute-directory>`.

- [ ] **Step 1: Write failing contract tests in `IosSharedContractTests.cs`**

  Assert that `CMP` implements `ICmpSdk`, each of the iOS, Android, and Editor wrappers implements `ISpSdk`, and the complete public method set inherited by `ICmpSdk` equals the methods in `ISpSdk` plus only the four existing `Use*` properties. The test must be reflection-only: do not invoke a native library.

- [ ] **Step 2: Write failing `IosUiTestBuildTests` for the command-line boundary**

  Cover an accepted absolute directory such as `/tmp/cmp-ios-export`; missing value, a relative path, and a file-like `.app` path must each throw an `ArgumentException` naming `-cmpUiTestXcodeProject`. Add an isolation test proving the in-memory AltTester configuration copy can switch to `AltPlatform.iOS` without changing the saved configuration.

- [ ] **Step 3: Run the new Unity EditMode tests and verify they fail for missing iOS builder**

  Run: Unity batch-mode EditMode command targeting `IosSharedContractTests` and `IosUiTestBuildTests`.

  Expected: the shared-contract assertions pass against the current wrappers; builder tests fail because `IosUiTestBuild` does not exist.

- [ ] **Step 4: Implement `IosUiTestBuild`**

  Mirror the proven Android builder's configuration-copy/`try`-`finally` discipline, but launch the batch editor with `-buildTarget iOS` and validate that `EditorUserBuildSettings.activeBuildTarget` is already iOS. Set `AltPlatform.iOS` and pass the requested absolute export directory and `BuildTarget.iOS` / `BuildTargetGroup.iOS` to `AltBuilder.BuildGame`. Require `BuildReport.summary.result == Succeeded`, zero errors, and a non-empty `<export>/Unity-iPhone.xcodeproj/project.pbxproj`; log the report and export path. Restore the persisted AltTester configuration even after an exception. Do not call `SwitchActiveBuildTarget` or restore Unity targets during batch execution.

- [ ] **Step 5: Run the focused Unity EditMode tests and source checks**

  Run: the Task 1 Unity batch-mode tests and `git diff --check -- Assets/Tests/Editor`.

  Expected: PASS; no persisted AltTester settings change.

- [ ] **Step 6: Commit Task 1**

  ```bash
  git add Assets/Tests/Editor/IosSharedContractTests.cs Assets/Tests/Editor/IosUiTestBuild.cs Assets/Tests/Editor/IosUiTestBuildTests.cs
  git commit -m "test(ios): add shared contract and export builder"
  ```

### Task 2: Establish a reproducible iOS baseline runner

**Files:**
- Create: `UI-TESTS/run-ios-ui-tests.sh`
- Create: `UI-TESTS/tests/run-ios-ui-tests.test.sh`
- Create: `UI-TESTS/tests/fixtures/simctl-with-device.sh`
- Create: `UI-TESTS/tests/fixtures/simctl-without-device.sh`
- Create: `UI-TESTS/tests/fixtures/xcodebuild-records-command.sh`
- Create: `UI-TESTS/tests/fixtures/pod-records-command.sh`
- Modify: `UI-TESTS/ios.runsettings`
- Modify: `UI-TESTS/README.md`

**Interfaces:**
- Consumes: `IosUiTestBuild.Build`, `xcrun simctl`, `pod`, `xcodebuild`, Appium, AltTester Desktop, and `UI-TESTS/UI-TESTS.csproj`.
- Produces: `run-ios-ui-tests.sh [--preflight]`, one timestamped `UI-TESTS/artifacts/<run>/` folder, and a fresh simulator `.app` path passed to the test suite.

- [ ] **Step 1: Write shell fixture tests for the runner contract**

  `run-ios-ui-tests.test.sh` must check `--help` lists `UNITY_PATH`, `UNITY_IOS_PLAYER_PATH`, `IOS_SIMULATOR_UDID`, `CMP_IOS_SIMULATOR_NAME`, `POD_BIN`, `XCODEBUILD_BIN`, `APPIUM_BIN`, `ALTTESTER_DESKTOP_PATH`, and `CMP_UI_ARTIFACTS_DIR`. With stub commands, prove that a matching simulator passes preflight, a missing simulator fails before Unity/Appium launch, and the runner uses `simctl bootstatus -b`, not `osascript` or `sleep`.

- [ ] **Step 2: Run the shell test and verify it fails before the runner exists**

  Run: `sh UI-TESTS/tests/run-ios-ui-tests.test.sh`

  Expected: FAIL because `UI-TESTS/run-ios-ui-tests.sh` is absent.

- [ ] **Step 3: Implement `run-ios-ui-tests.sh`**

  Use `set -eu` and the Android runner's fail-fast/artifact/cleanup conventions. Default to Unity `6000.5.10f1`, its `PlaybackEngines/iOSSupport` directory, `/Applications/AltTesterDesktop.app`, `.NET 8`, and `iPhone 14`; allow each to be overridden. Resolve `IOS_SIMULATOR_UDID` explicitly, or resolve `CMP_IOS_SIMULATOR_NAME` only when exactly one available simulator matches; fail on zero or ambiguous matches. Boot with `xcrun simctl boot` when needed and wait with `xcrun simctl bootstatus <udid> -b`.

  In a timestamped artifact directory, restore/discover the .NET tests, start Unity with `-buildTarget iOS` before the batch/export arguments `-executeMethod IosUiTestBuild.Build -cmpUiTestXcodeProject <artifact>/Unity-iPhone`, run `pod install` from that export, and capture each command's log. Build the generated workspace with `xcodebuild -workspace Unity-iPhone.xcworkspace -scheme Unity-iPhone -sdk iphonesimulator -destination platform=iOS\ Simulator,id=<udid> -derivedDataPath <artifact>/DerivedData CODE_SIGNING_ALLOWED=NO`; derive the fresh `ConsentMessagePlugin.app` under that DerivedData path, not from `UI-TESTS/BUILDS`.

  Start or reuse a local Appium server configured with the installed `xcuitest` driver and `altunity` plugin, install/launch the fresh app with `simctl`, wait for the AltTester port, then run the entire .NET project once with runsettings and explicit `appium:udid`/`appium:app` overrides. Emit one TRX and effective configuration file. Cleanup only Appium and simulator processes started by this invocation.

- [ ] **Step 4: Update `ios.runsettings` and UI-test documentation**

  Retain `platformName=iOS`, `automationName=xcuitest`, and the AltTester host, but make the runner's absolute fresh app path and simulator UDID authoritative overrides. Rewrite `UI-TESTS/README.md` as a shared Android/iOS runner guide: retain existing Android sections, add iOS prerequisites, all variables, artifact layout, and the fact that iOS does not silently retry tests.

- [ ] **Step 5: Run fixture verification and a current-version baseline**

  Run: `sh UI-TESTS/tests/run-ios-ui-tests.test.sh`, `sh -n UI-TESTS/run-ios-ui-tests.sh`, and `UI-TESTS/run-ios-ui-tests.sh` while `SourcepointDependencies.xml` still selects iOS `7.7.1`.

  Expected: shell checks pass; the baseline creates a fresh export, records its actual `Podfile.lock`, builds a fresh `.app`, and runs every discovered test once. Record the command, simulator runtime/UDID, discovered count, exit status, and artifact directory in `UI-TESTS/README.md`; do not call it green unless its initial full-suite exit is zero.

- [ ] **Step 6: Commit Task 2**

  ```bash
  git add UI-TESTS/run-ios-ui-tests.sh UI-TESTS/tests/run-ios-ui-tests.test.sh UI-TESTS/tests/fixtures/simctl-with-device.sh UI-TESTS/tests/fixtures/simctl-without-device.sh UI-TESTS/tests/fixtures/xcodebuild-records-command.sh UI-TESTS/tests/fixtures/pod-records-command.sh UI-TESTS/ios.runsettings UI-TESTS/README.md
  git commit -m "test(ios): add reproducible UI release runner"
  ```

### Task 3: Move the Swift bridge to Unity ownership and update the iOS dependency

**Files:**
- Create: `UI-TESTS/assert-ios-cmp-export.sh`
- Create: `UI-TESTS/tests/assert-ios-cmp-export.test.sh`
- Create: `UI-TESTS/tests/fixtures/ios-cmp-export/good/Podfile.lock`
- Create: `UI-TESTS/tests/fixtures/ios-cmp-export/good/Unity-iPhone.xcodeproj/project.pbxproj`
- Create: `UI-TESTS/tests/fixtures/ios-cmp-export/wrong-cmp/Podfile.lock`
- Create: `UI-TESTS/tests/fixtures/ios-cmp-export/wrong-core/Podfile.lock`
- Create: `UI-TESTS/tests/fixtures/ios-cmp-export/pod-owned-bridge/Unity-iPhone.xcodeproj/project.pbxproj`
- Modify: `Assets/ConsentManagementProvider/Editor/CMPPostProcessBuild.cs`
- Modify: `Assets/ConsentManagementProvider/Plugins/iOS/Source/Unity.mm`
- Modify: `Assets/ConsentManagementProvider/Editor/SourcepointDependencies.xml`
- Modify: `UI-TESTS/run-ios-ui-tests.sh`
- Modify: `UI-TESTS/README.md`

**Interfaces:**
- Consumes: an exported Xcode root, its `Podfile.lock`, and `Unity-iPhone.xcodeproj/project.pbxproj`.
- Produces: `assert-ios-cmp-export.sh <export-root> 7.12.11 0.1.17`, which exits non-zero with the missing/forbidden value named in its error.

- [ ] **Step 1: Write failing export-assertion fixtures and tests**

  The good fixture must declare `ConsentViewController (7.12.11)`, its exact `SPMobileCore (= 0.1.17)` dependency, an installed `SPMobileCore (0.1.17)`, and `SwiftBridge.swift` in the `UnityFramework` build context. Fixtures must independently fail for a wrong CMP version, wrong mobile-core version, and a bridge path/build-file under `Pods/ConsentViewController`.

- [ ] **Step 2: Run the fixture test and verify it fails before implementation**

  Run: `sh UI-TESTS/tests/assert-ios-cmp-export.test.sh`

  Expected: FAIL because `assert-ios-cmp-export.sh` does not exist.

- [ ] **Step 3: Implement the export assertion**

  Parse `Podfile.lock` with fixed strings/portable shell checks; do not infer versions from the XML. Check that the PBX project exists, contains `SwiftBridge.swift` in the Unity-owned `UnityFramework` target context, and has no path or build file that puts the bridge beneath `Pods/ConsentViewController`. Print matching lockfile/project lines on success. Make the iOS runner call this assertion after `pod install` and before `xcodebuild`.

- [ ] **Step 4: Replace the Pod mutation with Unity-owned bridge configuration**

  First update `Unity.mm` to import `UnityFramework-Swift.h`, the generated header for the target that owns both the Objective-C++ facade and `SwiftBridge.swift`. In `CMPPostProcessBuild`, remove `RemoveBridge`, `AddBridgeToPods`, `ExecuteProcessTerminal`, the shell `mv`, and every `Pods.xcodeproj`/`ConsentViewController/Classes` path assumption. Keep only checked PBX configuration for the Unity target: ensure the Swift source belongs to `UnityFramework`, enable the needed Swift module/runtime settings on the owning Unity targets, preserve required framework/search-path and Info.plist behavior, and write the main PBX project once.

  Do not add a fallback that mutates Pods. If the fresh export cannot produce `UnityFramework-Swift.h` or link the bridge, stop and diagnose the target setup before proceeding.

  Resolve the one existing Swift bridge in either the asset-import layout (`Libraries/ConsentManagementProvider/Plugins/iOS/Source/SwiftBridge.swift`) or UPM layout (`Libraries/com.sourcepoint.unitycmp/Assets/ConsentManagementProvider/Plugins/iOS/Source/SwiftBridge.swift`). Reuse its existing PBX reference. Cover both layouts with the real Unity PBX harness, and fail explicitly when neither source or multiple candidates exist.

- [ ] **Step 5: Update the declared pod and run focused checks**

  Change only `ConsentViewController` in `SourcepointDependencies.xml` from `7.7.1` to `7.12.11`; retain Android CMP `7.15.14` and current iOS target settings. Run `sh UI-TESTS/tests/assert-ios-cmp-export.test.sh`, `sh -n UI-TESTS/run-ios-ui-tests.sh`, and the Task 1 Unity EditMode tests.

  Expected: all local tests pass; no public C# signature changed; no Android source changed.

- [ ] **Step 6: Run the first upgraded iOS build gate**

  Run: `UI-TESTS/run-ios-ui-tests.sh`

  Expected: `Podfile.lock` selects `ConsentViewController 7.12.11` and `SPMobileCore 0.1.17`; export assertion passes; Xcode builds the Unity-owned bridge; the fresh app launches and the initial full UI suite passes once. Preserve the artifact directory if any phase fails.

- [ ] **Step 7: Commit Task 3**

  ```bash
  git add Assets/ConsentManagementProvider/Editor/CMPPostProcessBuild.cs Assets/ConsentManagementProvider/Plugins/iOS/Source/Unity.mm Assets/ConsentManagementProvider/Editor/SourcepointDependencies.xml UI-TESTS/assert-ios-cmp-export.sh UI-TESTS/tests/assert-ios-cmp-export.test.sh UI-TESTS/tests/fixtures/ios-cmp-export UI-TESTS/run-ios-ui-tests.sh UI-TESTS/README.md
  git commit -m "build(ios): update CMP to 7.12.11"
  ```

### Task 4: Publish consumer requirements and close the paired-platform release gate

**Files:**
- Modify: `README.md`
- Modify: `CHANGELOG.md`
- Modify: `UI-TESTS/README.md`
- Modify: `docs/superpowers/specs/2026-10-04-ios-cmp-7-12-11-migration-design.md` only if the observed export/build result changes a durable constraint

**Interfaces:**
- Consumes: successful Task 3 iOS artifacts and existing Android bridge regression checks.
- Produces: customer-facing upgrade requirements in the root README and recorded evidence for the Android/iOS release pair.

- [ ] **Step 1: Write the root README client-facing iOS upgrade section**

  State the exact tested pair (Android `7.15.14`, iOS `7.12.11`), current Unity iOS deployment target `13.0`, and the normal upgrade action: update the Unity package, make a fresh iOS export, and run ordinary CocoaPods installation. State explicitly that consumers change neither C# calls nor Podfiles. Explain that a customer-maintained incompatible `ConsentViewController`/`SPMobileCore` pin produces an ordinary CocoaPods conflict and is not supported through a hidden workaround. Do not expose internal runner paths as customer requirements.

- [ ] **Step 2: Record the release change and repeatable internal commands**

  Add a concise `CHANGELOG.md` entry for iOS `7.12.11`, `SPMobileCore 0.1.17`, preserved public Unity API, and removal of Pod-project mutation. In `UI-TESTS/README.md`, record the final successful iOS command, Xcode/simulator identity, test count, and artifact convention; keep the Android runner documentation intact.

- [ ] **Step 3: Verify both sides of the published pair**

  Run: `sh UI-TESTS/tests/sp-consents-json-bridge.test.sh`, `sh UI-TESTS/tests/sp-consents-json-bridge-7.15-compile.test.sh`, `sh UI-TESTS/tests/run-ios-ui-tests.test.sh`, `sh UI-TESTS/tests/assert-ios-cmp-export.test.sh`, Task 1 Unity EditMode tests, and `UI-TESTS/run-ios-ui-tests.sh`.

  Expected: Android's existing adapter tests pass unchanged; iOS runner fixtures and export assertions pass; the fresh iOS release gate passes once with a retained artifact folder.

- [ ] **Step 4: Perform final diff and client-contract review**

  Run: `git diff --check`, `git diff -- Assets/ConsentManagementProvider/Scripts/interface/ISpSdk.cs Assets/ConsentManagementProvider/Scripts/facade/ICmpSdk.cs Assets/ConsentManagementProvider/Scripts/facade/CMP.cs`, and `git diff -- Assets/Plugins/Android/SpConsentsJsonBridge.java`.

  Expected: no whitespace errors, no C# contract change, and no Android bridge change. Read the root README as a package consumer: every required tool, version floor, action, and conflict behavior must be explicit there; nothing client-facing may exist only in an internal plan/spec.

- [ ] **Step 5: Commit Task 4**

  ```bash
  git add README.md CHANGELOG.md UI-TESTS/README.md docs/superpowers/specs/2026-10-04-ios-cmp-7-12-11-migration-design.md
  git commit -m "docs: publish iOS CMP 7.12.11 upgrade requirements"
  ```
