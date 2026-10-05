#!/bin/sh

set -eu

script_dir=$(CDPATH= cd -- "$(dirname -- "$0")" && pwd)
project_root=$(CDPATH= cd -- "$script_dir/.." && pwd)
UNITY_PATH=${UNITY_PATH:-/Applications/Unity/Installs/6000.5.10f1/Unity.app/Contents/MacOS/Unity}
UNITY_IOS_PLAYER_PATH=${UNITY_IOS_PLAYER_PATH:-/Applications/Unity/Installs/6000.5.10f1/PlaybackEngines/iOSSupport}
IOS_SIMULATOR_UDID=${IOS_SIMULATOR_UDID:-}
CMP_IOS_SIMULATOR_NAME=${CMP_IOS_SIMULATOR_NAME:-iPhone 14}
POD_BIN=${POD_BIN:-pod}
XCODEBUILD_BIN=${XCODEBUILD_BIN:-xcodebuild}
XCRUN_BIN=${XCRUN_BIN:-xcrun}
APPIUM_BIN=${APPIUM_BIN:-appium}
DOTNET_BIN=${DOTNET_BIN:-dotnet}
PYTHON_BIN=${PYTHON_BIN:-python3}
ALTTESTER_DESKTOP_PATH=${ALTTESTER_DESKTOP_PATH:-/Applications/AltTesterDesktop.app}
ALTTESTER_PORT=${ALTTESTER_PORT:-13000}
CMP_UI_ARTIFACTS_DIR=${CMP_UI_ARTIFACTS_DIR:-$script_dir/artifacts}
artifact_dir=
appium_pid=
simulator_started=0
simulator_state=
simulator_runtime=
simulator_name=
preflight_completed=0

usage() {
    cat <<'EOF'
Usage: UI-TESTS/run-ios-ui-tests.sh [--preflight]

Exports a fresh AltTester-instrumented iOS simulator app and runs the entire
.NET UI suite once. A failed initial suite is returned without retries.

Environment overrides:
  UNITY_PATH                 Unity executable (default: Unity 6000.5.10f1).
  UNITY_IOS_PLAYER_PATH      Unity PlaybackEngines/iOSSupport directory.
  IOS_SIMULATOR_UDID         Explicit available simulator UDID (takes precedence).
  CMP_IOS_SIMULATOR_NAME     Exactly one available name match (default: iPhone 14).
  POD_BIN                    CocoaPods command/path (default: pod).
  XCODEBUILD_BIN             Xcode command/path (default: xcodebuild).
  XCRUN_BIN                  xcrun command/path (default: xcrun).
  APPIUM_BIN                 Appium command/path (default: appium).
  DOTNET_BIN                 .NET 8 SDK command/path (default: dotnet).
  PYTHON_BIN                 Python 3 command/path (default: python3).
  ALTTESTER_DESKTOP_PATH     Desktop .app (default: /Applications/AltTesterDesktop.app).
  ALTTESTER_PORT             Local AltTester readiness port (default: 13000).
  CMP_UI_ARTIFACTS_DIR       Parent directory for timestamped logs/results.

--preflight validates prerequisites and simulator selection only; it does not
boot a simulator or launch Unity, Appium, AltTester Desktop, or tests.
An existing Appium server at 127.0.0.1:4723 must already enable altunity.
EOF
}

fail() { echo "ERROR: $*" >&2; exit 1; }
require_command() { command -v "$1" >/dev/null 2>&1 || fail "$2 is missing: $1"; }

resolve_simulator() {
    devices_json=$("$XCRUN_BIN" simctl list devices available -j) || fail "Unable to list available iOS simulators."
    if [ -n "$artifact_dir" ]; then
        printf '%s\n' "$devices_json" >"$artifact_dir/simulators.json"
    fi
    selection=$(printf '%s\n' "$devices_json" | "$PYTHON_BIN" -c '
import json, sys
udid, name = sys.argv[1:]
devices = [(runtime, device) for runtime, group in json.load(sys.stdin)["devices"].items()
           for device in group if device.get("isAvailable", False)]
matches = [(runtime, device) for runtime, device in devices
           if (device["udid"] == udid if udid else device["name"] == name)]
if not matches:
    sys.exit("ERROR: No available simulator matches " + ("UDID " + udid if udid else "name " + name))
if len(matches) != 1:
    sys.exit("ERROR: Multiple available simulators match; set IOS_SIMULATOR_UDID explicitly.")
runtime, device = matches[0]
print(device["udid"] + "\n" + device["state"] + "\n" + runtime + "\n" + device["name"])
' "$IOS_SIMULATOR_UDID" "$CMP_IOS_SIMULATOR_NAME") || exit 1
    IOS_SIMULATOR_UDID=$(printf '%s\n' "$selection" | sed -n '1p')
    simulator_state=$(printf '%s\n' "$selection" | sed -n '2p')
    simulator_runtime=$(printf '%s\n' "$selection" | sed -n '3p')
    simulator_name=$(printf '%s\n' "$selection" | sed -n '4p')
}

preflight() {
    [ -x "$UNITY_PATH" ] || fail "Unity 6000.5.10f1 is missing or not executable: $UNITY_PATH"
    [ -d "$UNITY_IOS_PLAYER_PATH" ] || fail "Unity iOS support is missing: $UNITY_IOS_PLAYER_PATH. Install iOS Build Support for this Unity editor."
    [ -d "$ALTTESTER_DESKTOP_PATH" ] || fail "AltTester Desktop is missing: $ALTTESTER_DESKTOP_PATH"
    require_command "$PYTHON_BIN" 'Python 3'
    require_command "$XCRUN_BIN" xcrun
    resolve_simulator
    require_command "$POD_BIN" CocoaPods
    "$POD_BIN" --version
    require_command "$XCODEBUILD_BIN" Xcode
    "$XCODEBUILD_BIN" -version
    require_command "$DOTNET_BIN" '.NET 8'
    "$DOTNET_BIN" --list-sdks | grep -E '^[[:space:]]*8\.' >/dev/null || fail '.NET 8 SDK is missing.'
    require_command "$APPIUM_BIN" Appium
    "$APPIUM_BIN" driver list --installed --json | grep -q 'xcuitest' || fail 'Install Appium xcuitest: appium driver install xcuitest'
    "$APPIUM_BIN" plugin list --installed 2>&1 | grep -q 'altunity' || fail 'Install Appium altunity: appium plugin install --source=npm appium-altunity-plugin'
    require_command curl curl
    require_command nc nc
    require_command open 'macOS open'
    echo "Preflight passed: $simulator_name ($IOS_SIMULATOR_UDID), $simulator_runtime, $simulator_state"
}

cleanup() {
    status=$?
    trap - EXIT INT TERM
    if [ -n "$artifact_dir" ]; then
        if [ "$status" -ne 0 ] && [ "$preflight_completed" = 0 ] && [ -f "$artifact_dir/preflight.log" ]; then
            head -n 20 "$artifact_dir/preflight.log" >&3
        fi
        printf 'EXIT_STATUS=%s\n' "$status" >>"$artifact_dir/effective-test-config.txt"
        printf '%s\n' "$status" >"$artifact_dir/exit-status.txt"
    fi
    if [ -n "$appium_pid" ]; then
        kill "$appium_pid" 2>/dev/null || true
        wait "$appium_pid" 2>/dev/null || true
    fi
    if [ "$simulator_started" = 1 ]; then
        "$XCRUN_BIN" simctl shutdown "$IOS_SIMULATOR_UDID" >>"$artifact_dir/simulator.log" 2>&1 || true
    fi
    exit "$status"
}

appium_is_ready() {
    curl -fsS --max-time 2 http://127.0.0.1:4723/status 2>/dev/null |
        "$PYTHON_BIN" -c 'import json,sys; sys.exit(0 if json.load(sys.stdin).get("value",{}).get("ready") is True else 1)' 2>/dev/null
}

# Simulator readiness always uses simctl bootstatus -b. Service polling is
# separately bounded, including the duration of each external probe.
wait_for_service() {
    "$PYTHON_BIN" - "$@" <<'PY'
import json, subprocess, sys, time
kind, *command = sys.argv[1:]
deadline = time.monotonic() + 120
while time.monotonic() < deadline:
    try:
        result = subprocess.run(command, capture_output=True, timeout=3)
        if result.returncode == 0 and (kind != "Appium" or json.loads(result.stdout).get("value", {}).get("ready") is True):
            sys.exit(0)
    except (subprocess.TimeoutExpired, ValueError):
        pass
    time.sleep(1)
sys.exit("ERROR: Timed out waiting for " + kind)
PY
}

run_tests() {
    # Preserve original stderr for EXIT cleanup, even if preflight exits while
    # its log redirection is active. Never read a log into its own output file.
    exec 3>&2
    mkdir -p "$CMP_UI_ARTIFACTS_DIR"
    CMP_UI_ARTIFACTS_DIR=$(CDPATH= cd -- "$CMP_UI_ARTIFACTS_DIR" && pwd)
    artifact_dir=$(mktemp -d "$CMP_UI_ARTIFACTS_DIR/$(date +%Y%m%d-%H%M%S)-ios.XXXXXX")
    trap cleanup EXIT
    trap 'exit 130' INT
    trap 'exit 143' TERM
    echo "Artifacts: $artifact_dir"
    {
        printf 'UNITY_PATH=%s\n' "$UNITY_PATH"
        printf 'UNITY_IOS_PLAYER_PATH=%s\n' "$UNITY_IOS_PLAYER_PATH"
        printf 'CMP_IOS_SIMULATOR_NAME=%s\n' "$CMP_IOS_SIMULATOR_NAME"
        printf 'IOS_SIMULATOR_UDID=%s\n' "$IOS_SIMULATOR_UDID"
        printf 'POD_BIN=%s\n' "$POD_BIN"
        printf 'XCODEBUILD_BIN=%s\n' "$XCODEBUILD_BIN"
        printf 'XCRUN_BIN=%s\n' "$XCRUN_BIN"
        printf 'APPIUM_BIN=%s\n' "$APPIUM_BIN"
        printf 'DOTNET_BIN=%s\n' "$DOTNET_BIN"
        printf 'PYTHON_BIN=%s\n' "$PYTHON_BIN"
        printf 'ALTTESTER_DESKTOP_PATH=%s\n' "$ALTTESTER_DESKTOP_PATH"
        printf 'ALTTESTER_PORT=%s\n' "$ALTTESTER_PORT"
        printf 'DISCOVERED_TEST_COUNT=not-run\n'
        printf 'SUITE_EXIT_STATUS=not-run\n'
    } >"$artifact_dir/effective-test-config.txt"
    preflight >"$artifact_dir/preflight.log" 2>&1
    preflight_completed=1
    cat "$artifact_dir/preflight.log"
    {
        printf 'IOS_SIMULATOR_UDID=%s\n' "$IOS_SIMULATOR_UDID"
        printf 'IOS_SIMULATOR_RUNTIME=%s\n' "$simulator_runtime"
        printf 'IOS_SIMULATOR_NAME=%s\n' "$simulator_name"
        printf 'IOS_SIMULATOR_INITIAL_STATE=%s\n' "$simulator_state"
    } >>"$artifact_dir/effective-test-config.txt"
    cp "$script_dir/ios.runsettings" "$artifact_dir/ios.runsettings"
    "$DOTNET_BIN" restore "$script_dir/UI-TESTS.csproj" >"$artifact_dir/dotnet-restore.log" 2>&1
    "$DOTNET_BIN" test "$script_dir/UI-TESTS.csproj" --no-restore --list-tests >"$artifact_dir/test-discovery.txt" 2>&1
    test_count=$(awk '
        /^The following Tests are available:$/ { discovered = 1; next }
        discovered && /^    [^[:space:]]/ { count += 1 }
        END { print count + 0 }
    ' "$artifact_dir/test-discovery.txt")
    [ "$test_count" -gt 0 ] || fail "No UI tests discovered; see $artifact_dir/test-discovery.txt"
    printf 'DISCOVERED_TEST_COUNT=%s\n' "$test_count" >>"$artifact_dir/effective-test-config.txt"
    echo "Discovered $test_count UI tests."

    "$UNITY_PATH" -buildTarget iOS -batchmode -quit -nographics -projectPath "$project_root" \
        -executeMethod IosUiTestBuild.Build -cmpUiTestXcodeProject "$artifact_dir/Unity-iPhone" \
        -logFile "$artifact_dir/unity.log" >"$artifact_dir/unity-console.log" 2>&1
    [ -d "$artifact_dir/Unity-iPhone" ] || fail 'Unity completed without a fresh Xcode export.'
    (cd "$artifact_dir/Unity-iPhone" && "$POD_BIN" install) >"$artifact_dir/pod-install.log" 2>&1
    [ -s "$artifact_dir/Unity-iPhone/Podfile.lock" ] || fail 'pod install produced no Podfile.lock.'
    cp "$artifact_dir/Unity-iPhone/Podfile.lock" "$artifact_dir/Podfile.lock"
    sh "$script_dir/assert-ios-cmp-export.sh" "$artifact_dir/Unity-iPhone" 7.12.11 0.1.17 >"$artifact_dir/export-assertion.log" 2>&1 || {
        cat "$artifact_dir/export-assertion.log" >&2
        fail "iOS CMP export assertion failed; see $artifact_dir/export-assertion.log"
    }
    cat "$artifact_dir/export-assertion.log"
    [ -d "$artifact_dir/Unity-iPhone/Unity-iPhone.xcworkspace" ] || fail 'pod install produced no Unity-iPhone.xcworkspace.'
    if [ "$simulator_state" != Booted ]; then
        "$XCRUN_BIN" simctl boot "$IOS_SIMULATOR_UDID" >"$artifact_dir/simulator.log" 2>&1
        simulator_started=1
    fi
    "$XCRUN_BIN" simctl bootstatus "$IOS_SIMULATOR_UDID" -b >>"$artifact_dir/simulator.log" 2>&1
    (cd "$artifact_dir/Unity-iPhone" && "$XCODEBUILD_BIN" \
        -workspace Unity-iPhone.xcworkspace -scheme Unity-iPhone -sdk iphonesimulator \
        -destination "platform=iOS Simulator,id=$IOS_SIMULATOR_UDID" \
        -derivedDataPath "$artifact_dir/DerivedData" CODE_SIGNING_ALLOWED=NO) >"$artifact_dir/xcodebuild.log" 2>&1
    app_path=
    for candidate in "$artifact_dir"/DerivedData/Build/Products/*-iphonesimulator/ConsentMessagePlugin.app; do
        [ -d "$candidate" ] || continue
        [ -z "$app_path" ] || fail 'xcodebuild produced multiple simulator apps; inspect DerivedData.'
        app_path=$candidate
    done
    [ -n "$app_path" ] || fail "xcodebuild produced no fresh ConsentMessagePlugin.app under $artifact_dir/DerivedData/Build/Products."
    bundle_id=$("$PYTHON_BIN" - "$app_path/Info.plist" <<'PY'
import plistlib, sys
with open(sys.argv[1], "rb") as source:
    print(plistlib.load(source)["CFBundleIdentifier"])
PY
    )
    [ -n "$bundle_id" ] || fail 'The fresh app has no CFBundleIdentifier.'
    printf 'APPIUM_APP=%s\nAPP_BUNDLE_ID=%s\n' "$app_path" "$bundle_id" >>"$artifact_dir/effective-test-config.txt"

    if appium_is_ready; then
        printf 'Reusing external Appium at 127.0.0.1:4723; it must already enable altunity.\n' >"$artifact_dir/appium.log"
    else
        "$APPIUM_BIN" --address 127.0.0.1 --port 4723 --use-plugins=altunity >"$artifact_dir/appium.log" 2>&1 &
        appium_pid=$!
        wait_for_service Appium curl -fsS --max-time 2 http://127.0.0.1:4723/status
    fi
    open "$ALTTESTER_DESKTOP_PATH" >"$artifact_dir/alttester-desktop.log" 2>&1
    "$XCRUN_BIN" simctl install "$IOS_SIMULATOR_UDID" "$app_path" >"$artifact_dir/simctl-install.log" 2>&1
    "$XCRUN_BIN" simctl launch "$IOS_SIMULATOR_UDID" "$bundle_id" >"$artifact_dir/simctl-launch.log" 2>&1
    wait_for_service AltTester nc -z -w 2 127.0.0.1 "$ALTTESTER_PORT"
    suite_status=0
    "$DOTNET_BIN" test "$script_dir/UI-TESTS.csproj" --no-restore \
        --settings "$artifact_dir/ios.runsettings" --logger 'trx;LogFileName=results.trx' \
        --results-directory "$artifact_dir" -- \
        "TestRunParameters.Parameter(name=\"deviceName\",value=\"$simulator_name\")" \
        "TestRunParameters.Parameter(name=\"appium:udid\",value=\"$IOS_SIMULATOR_UDID\")" \
        "TestRunParameters.Parameter(name=\"appium:app\",value=\"$app_path\")" \
        >"$artifact_dir/dotnet-test.log" 2>&1 || suite_status=$?
    printf 'SUITE_EXIT_STATUS=%s\n' "$suite_status" >>"$artifact_dir/effective-test-config.txt"
    cat "$artifact_dir/dotnet-test.log"
    return "$suite_status"
}

[ "$#" -le 1 ] || { usage >&2; exit 2; }
case "${1:-}" in
    --help|-h) usage ;;
    --preflight) preflight ;;
    '') run_tests ;;
    *) usage >&2; exit 2 ;;
esac
