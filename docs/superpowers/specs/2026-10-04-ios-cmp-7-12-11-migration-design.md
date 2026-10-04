# CMP Unity SDK iOS CMP 7.12.11 Migration

## Intent

Update the iOS dependency distributed by `CMP-Unity-SDK` from
`ConsentViewController 7.7.1` to `7.12.11`, while keeping the package easy to
consume from Unity and making the native-version pair explicit and verifiable.

This is a maintenance migration. It must not change the public Unity/C# API,
require client code or Podfile changes, or use new iOS native APIs as a reason
to expand scope.

The existing shared C# contract is explicitly in scope for preservation:
`CMP` exposes the platform-neutral public API and each native implementation
continues to implement `ISpSdk`. The migration validates that this interface,
its public facade behavior, callbacks, and consent JSON models stay aligned
between Android and iOS. It does not create a second platform-specific public
API.

## Release Pair and Version Policy

Android and iOS have independently versioned release streams. "Synchronized"
means that the SDK publishes and tests one intentional pair, not that both
numbers must match:

| Component | Required version | Source of truth |
| --- | --- | --- |
| Android CMP | `7.15.14` | `SourcepointDependencies.xml` |
| iOS ConsentViewController | `7.12.11` | `SourcepointDependencies.xml` |
| iOS transitive SPMobileCore | `0.1.17` | generated `Podfile.lock` release-gate assertion |

The native version declaration remains solely in
`Assets/ConsentManagementProvider/Editor/SourcepointDependencies.xml`. The
tested tuple and its evidence are documented rather than duplicated in another
dependency manifest.

Upstream iOS CMP `7.12.11` is the latest release at the time of this design.
Its release note reports a fix for a user's message-status reporting. Its pod
spec pins `SPMobileCore 0.1.17` and retains iOS deployment target `10.0`.

## Client Contract

For a Unity customer importing the package, the change is intentionally
transparent:

- Existing scenes, C# calls, callbacks, consent models, and package
  configuration continue unchanged.
- A normal fresh iOS export followed by CocoaPods resolution selects the new
  native libraries. No hand edit to a customer's Podfile is supported or
  required.
- Existing CocoaPods caches or an already-exported Xcode workspace may retain
  the old lockfile. The documented upgrade action is a fresh Unity export and
  normal `pod install`; it is not an SDK-specific workaround.
- A customer with an explicit, incompatible pin for `ConsentViewController` or
  `SPMobileCore` in a custom Podfile gets a normal CocoaPods conflict. The SDK
  must not mask that conflict by modifying installed Pods.

These are client-facing requirements, not implementation notes. The root
`README.md` must publish them in an iOS upgrade section: the supported Unity
iOS target, the fresh-export/normal-`pod install` upgrade path, the fact that
no C# or Podfile edits are required, and the expected CocoaPods behavior for a
customer-maintained incompatible pin. The release must not rely on this design
document as the only place a package consumer can learn those constraints.

The update does not raise the package's existing Unity iOS deployment target
of `13.0` (`ProjectSettings/ProjectSettings.asset`). The upstream pod still
declares iOS `10.0`, so it introduces no stricter floor. The supported
Xcode/CocoaPods toolchain is established by the release gate rather than
assumed from the development machine.

## Current iOS Integration and Problem

The package owns `SwiftBridge.swift` and `Unity.mm`, which adapt the Swift
native CMP to Unity's C ABI and C# `DllImport` declarations. The public C# API
already routes all iOS calls through this bridge.

`CMPPostProcessBuild` currently removes `SwiftBridge.swift` from Unity's
export, shell-moves it into the installed `ConsentViewController` pod, and
edits `Pods.xcodeproj` to compile it in the pod target. This is an unsupported
ownership boundary:

- It assumes a particular pod directory layout and target names.
- It mutates generated dependency output that CocoaPods owns.
- Its shell command does not provide a structured failure contract.
- A pod upgrade can break a customer export even when the native public API is
  otherwise compatible.

## Options Considered

### Keep the current Pod mutation

It has historical compatibility and needs the fewest immediate source edits.
It is rejected because the hidden dependency on `Pods` internals is exactly the
kind of upgrade fragility this migration must remove.

### Compile the bridge in a Unity-owned exported target (selected)

`SwiftBridge.swift` remains a Unity package asset. The exported Unity/Xcode
target compiles it, imports `ConsentViewController` normally, and exposes its
generated Swift header to `Unity.mm`. The postprocessor performs only
first-party project configuration required for that target; it never moves a
source file, runs a shell move, or edits `Pods.xcodeproj`.

This gives the package clear ownership of its bridge and allows CocoaPods to
install/upgrade the dependency normally. The trade-off is that the fresh export
must prove correct target membership, Swift-header visibility, linking, and
Swift runtime embedding. A compile failure is a release-gate failure, not a
reason to retain a hidden Pod mutation as a fallback.

### Introduce a cross-platform test-runner framework

This could remove duplication between Android and iOS, but it broadens the
migration and couples two independently working native flows. It is deferred.
The iOS runner should borrow Android's reproducibility rules while staying
project-local and platform-specific.

## Proposed Architecture

### Dependency and bridge boundary

1. Change only the iOS pod declaration from `7.7.1` to `7.12.11`.
2. Compile `SwiftBridge.swift` in the Unity-owned target after export. Update
   `Unity.mm` to import that target's generated Swift header.
3. Simplify `CMPPostProcessBuild` to deterministic, checked first-party Xcode
   project configuration. Remove its `Pods.xcodeproj` mutation and shell file
   move.
4. Preserve all existing C# `DllImport` symbols and Swift bridge method
   behavior. Do not add `globalcmp`, `preferences`, or any other newly exposed
   native capability in this migration.

### iOS release gate

Add one supported iOS command, `UI-TESTS/run-ios-ui-tests.sh`, with explicit,
overridable inputs for the Unity/Xcode export, simulator/device, CocoaPods,
Appium, and AltTester. It replaces `iosStartup.sh` as the release gate.

The runner must:

1. Preflight required commands and simulator availability.
2. Create a fresh Unity iOS export and resolve pods without relying on an old
   `.app`, workspace, or lockfile.
3. Assert the resolved `Podfile.lock` contains exactly
   `ConsentViewController 7.12.11` and `SPMobileCore 0.1.17`.
4. Build the exported project for the selected simulator architecture.
5. Install/launch the fresh application and execute the existing .NET/Appium/
   AltTester UI suite once.
6. Write Unity, CocoaPods, Xcode, Appium, simulator, test-result, and lockfile
   evidence to one timestamped artifact directory.

It uses `xcrun simctl` readiness checks rather than a hard-coded simulator
UDID, `osascript`, fixed delays, or silent retries. A failed step exits
non-zero and names the artifact directory; subsequent version changes stop.

## Validation Scope

The baseline and upgraded gate exercise the current public behavior: first
layer accept/reject, GDPR/CCPA/USNAT privacy managers, save and exit, clear
data, authenticated consent, programmatic rejection, custom GDPR consent, and
message language. The C# public surface is compared before and after the
migration; a signature change is a regression.

In addition, an automated export/project assertion verifies that no production
postprocess path references `Pods.xcodeproj`, moves `SwiftBridge.swift`, or
depends on `Pods/ConsentViewController/.../Classes`. This directly protects the
maintainability objective.

`SpConsentsJsonBridge.java` is out of scope. It is an Android-only adapter
introduced by the Android CMP 7.12 object-callback/model change. Its Java
compile and JSON regression tests remain part of the Android side of the
published pair, but the iOS migration neither removes nor rewrites it.

## Failure Policy

Do not publish the new iOS version if any of the following occurs:

- CocoaPods resolves a different CMP or mobile-core version.
- The Unity-owned Swift bridge cannot compile, link, or expose its generated
  header in a fresh Xcode export.
- The postprocessor still mutates a Pod or requires a client-side workaround.
- A pre-existing iOS UI flow, public C# signature, or Android bridge regression
  test fails.

Keep the prior iOS pin and hand off the first failing command plus its artifact
directory. Do not obscure a failure with retries, manual Pod edits, or a
second implementation path that edits `Pods`.

## Out of Scope

- New Unity API surface for Global CMP, Preferences, or future native features.
- Any Android dependency or implementation change.
- Refactoring the Android and iOS **test shell runners** into one shared
  automation framework. This is unrelated to, and must not weaken, the shared
  C# `ISpSdk`/`CMP` consumer API.
- Customer-specific Podfile conflict workarounds.
