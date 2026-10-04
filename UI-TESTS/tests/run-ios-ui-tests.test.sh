#!/bin/sh
set -eu

# External tools are replaced at the command boundary. A wrong build destination,
# stale app, missing wait, simulator launch during preflight, or suite retry fails.
case "$(basename "$0")" in
    unity|dotnet|appium|curl|nc|open|osascript|sleep)
        tool=$(basename "$0")
        printf '%s %s\n' "$tool" "$*" >>"$FIXTURE_CALLS"
        case "$tool:$*" in
            dotnet:--list-sdks) printf '8.0.414 [fixture]\n' ;;
            dotnet:*--list-tests*) printf 'The following Tests are available:\n    TestOne\n    TestTwo\n' ;;
            dotnet:test*)
                results=
                while [ "$#" -gt 0 ]; do
                    if [ "$1" = --results-directory ]; then shift; results=$1; fi
                    shift
                done
                mkdir -p "$results"
                printf '<TestRun/>\n' >"$results/results.trx"
                exit "${FIXTURE_TEST_EXIT:-0}"
                ;;
            unity:*)
                while [ "$#" -gt 0 ]; do
                    if [ "$1" = -cmpUiTestXcodeProject ]; then shift; mkdir -p "$1"; fi
                    shift
                done
                ;;
            appium:driver*) printf '{"xcuitest":{}}\n' ;;
            appium:plugin*) printf 'altunity\n' >&2 ;;
            appium:--*)
                printf '%s\n' "$$" >"$FIXTURE_APPIUM_PID"
                touch "$FIXTURE_APPIUM_READY"
                exec python3 -c 'import time; time.sleep(120)'
                ;;
            curl:*)
                if [ "${FIXTURE_START_APPIUM:-0}" = 1 ] && [ ! -f "$FIXTURE_APPIUM_READY" ]; then exit 7; fi
                printf '{"value":{"ready":true}}\n'
                ;;
            osascript:*|sleep:*) echo 'Forbidden simulator readiness shortcut' >&2; exit 99 ;;
        esac
        exit 0
        ;;
esac

test_dir=$(CDPATH= cd -- "$(dirname -- "$0")" && pwd)
runner="$test_dir/../run-ios-ui-tests.sh"
[ -f "$runner" ] || { echo 'FAIL: iOS runner is absent' >&2; exit 1; }
fixture_root=$(mktemp -d)
trap 'rm -rf "$fixture_root"' EXIT
mkdir -p "$fixture_root/bin" "$fixture_root/ios-player" "$fixture_root/alt-tester"
for tool in unity dotnet appium curl nc open osascript sleep; do
    cp "$0" "$fixture_root/bin/$tool"
done
cp "$test_dir/fixtures/simctl-with-device.sh" "$fixture_root/bin/xcrun"
cp "$test_dir/fixtures/xcodebuild-records-command.sh" "$fixture_root/bin/xcodebuild"
cp "$test_dir/fixtures/pod-records-command.sh" "$fixture_root/bin/pod"
chmod +x "$fixture_root/bin/"*
export PATH="$fixture_root/bin:$PATH"
export UNITY_PATH="$fixture_root/bin/unity" UNITY_IOS_PLAYER_PATH="$fixture_root/ios-player"
export ALTTESTER_DESKTOP_PATH="$fixture_root/alt-tester" DOTNET_BIN=dotnet
export XCRUN_BIN=xcrun POD_BIN=pod XCODEBUILD_BIN=xcodebuild APPIUM_BIN=appium
export CMP_UI_ARTIFACTS_DIR="$fixture_root/artifacts" FIXTURE_CALLS="$fixture_root/calls"
export FIXTURE_APPIUM_PID="$fixture_root/appium.pid" FIXTURE_APPIUM_READY="$fixture_root/appium.ready"
export IOS_SIMULATOR_UDID= CMP_IOS_SIMULATOR_NAME='iPhone 14'

sh "$runner" --help >"$fixture_root/help"
for name in UNITY_PATH UNITY_IOS_PLAYER_PATH IOS_SIMULATOR_UDID CMP_IOS_SIMULATOR_NAME POD_BIN XCODEBUILD_BIN APPIUM_BIN ALTTESTER_DESKTOP_PATH CMP_UI_ARTIFACTS_DIR; do
    grep -F "$name" "$fixture_root/help" >/dev/null
done
sh "$runner" --preflight >"$fixture_root/preflight" 2>&1
! grep -E '^(unity |appium --|xcrun simctl (boot|install|launch))' "$FIXTURE_CALLS"

# Missing build support must also fail a full invocation promptly: EXIT handlers
# run while a failing function's redirections are still active on macOS /bin/sh.
UNITY_IOS_PLAYER_PATH="$fixture_root/missing-ios-player" python3 - "$runner" <<'PY'
import os, signal, subprocess, sys
process = subprocess.Popen(["sh", sys.argv[1]], stdout=subprocess.PIPE, stderr=subprocess.PIPE, start_new_session=True)
try:
    stdout, stderr = process.communicate(timeout=3)
except subprocess.TimeoutExpired:
    os.killpg(process.pid, signal.SIGKILL)
    process.communicate()
    sys.exit("FAIL: preflight cleanup did not terminate")
assert process.returncode == 1, (process.returncode, stdout, stderr)
assert stderr.count(b"Unity iOS support is missing") == 1, stderr
PY
export CMP_UI_ARTIFACTS_DIR="$fixture_root/success-artifacts"

cp "$test_dir/fixtures/simctl-without-device.sh" "$fixture_root/bin/xcrun"
if sh "$runner" --preflight >"$fixture_root/missing" 2>&1; then
    echo 'FAIL: missing simulator passed' >&2; exit 1
fi
grep -F 'No available simulator' "$fixture_root/missing" >/dev/null
! grep -E '^(unity |appium --|xcrun simctl (boot|install|launch))' "$FIXTURE_CALLS"
cp "$test_dir/fixtures/simctl-with-device.sh" "$fixture_root/bin/xcrun"
if FIXTURE_AMBIGUOUS=1 sh "$runner" --preflight >"$fixture_root/ambiguous" 2>&1; then
    echo 'FAIL: ambiguous simulator passed' >&2; exit 1
fi
grep -F 'Multiple available simulators' "$fixture_root/ambiguous" >/dev/null
IOS_SIMULATOR_UDID=fixture-udid FIXTURE_AMBIGUOUS=1 sh "$runner" --preflight >/dev/null 2>&1
if IOS_SIMULATOR_UDID=missing-udid sh "$runner" --preflight >"$fixture_root/udid" 2>&1; then
    echo 'FAIL: unavailable explicit UDID passed' >&2; exit 1
fi

sh "$runner" >"$fixture_root/run" 2>&1 || { cat "$fixture_root/run"; exit 1; }
artifact=$(find "$CMP_UI_ARTIFACTS_DIR" -name effective-test-config.txt -exec dirname {} \;)
app="$artifact/DerivedData/Build/Products/Debug-iphonesimulator/ConsentMessagePlugin.app"
grep -F 'xcrun simctl bootstatus fixture-udid -b' "$FIXTURE_CALLS" >/dev/null
grep -F 'xcrun simctl boot fixture-udid' "$FIXTURE_CALLS" >/dev/null
grep -F 'xcrun simctl shutdown fixture-udid' "$FIXTURE_CALLS" >/dev/null
grep -F "pod cwd=$artifact/Unity-iPhone install" "$FIXTURE_CALLS" >/dev/null
grep -F -- "-executeMethod IosUiTestBuild.Build -cmpUiTestXcodeProject $artifact/Unity-iPhone" "$FIXTURE_CALLS" >/dev/null
grep -F -- "-workspace Unity-iPhone.xcworkspace -scheme Unity-iPhone -sdk iphonesimulator -destination platform=iOS Simulator,id=fixture-udid -derivedDataPath $artifact/DerivedData CODE_SIGNING_ALLOWED=NO" "$FIXTURE_CALLS" >/dev/null
grep -F "xcrun simctl install fixture-udid $app" "$FIXTURE_CALLS" >/dev/null
grep -F 'TestRunParameters.Parameter(name="appium:udid",value="fixture-udid")' "$FIXTURE_CALLS" >/dev/null
grep -F "TestRunParameters.Parameter(name=\"appium:app\",value=\"$app\")" "$FIXTURE_CALLS" >/dev/null
grep -F 'DISCOVERED_TEST_COUNT=2' "$artifact/effective-test-config.txt" >/dev/null
[ -f "$artifact/Podfile.lock" ] && [ -f "$artifact/results.trx" ]
[ "$(grep -c '^dotnet test .*--logger trx' "$FIXTURE_CALLS")" -eq 1 ]
! grep -E '^(osascript |sleep )' "$FIXTURE_CALLS"

# A failing initial suite is returned without retry; an already booted device
# stays booted after both suite failure and build failure.
export CMP_UI_ARTIFACTS_DIR="$fixture_root/failing-artifacts" FIXTURE_SIMULATOR_STATE=Booted
if FIXTURE_TEST_EXIT=7 sh "$runner" >"$fixture_root/failing" 2>&1; then
    echo 'FAIL: test failure was swallowed' >&2; exit 1
else
    [ "$?" -eq 7 ]
fi
[ "$(grep -c '^dotnet test .*--logger trx' "$FIXTURE_CALLS")" -eq 2 ]
[ "$(grep -c 'xcrun simctl shutdown fixture-udid' "$FIXTURE_CALLS")" -eq 1 ]
export CMP_UI_ARTIFACTS_DIR="$fixture_root/build-failing-artifacts"
if FIXTURE_XCODE_FAILURE=1 sh "$runner" >"$fixture_root/build-failing" 2>&1; then
    echo 'FAIL: build failure was swallowed' >&2; exit 1
else
    [ "$?" -eq 65 ]
fi
[ "$(grep -c '^dotnet test .*--logger trx' "$FIXTURE_CALLS")" -eq 2 ]
export CMP_UI_ARTIFACTS_DIR="$fixture_root/appium-start-artifacts"
FIXTURE_START_APPIUM=1 FIXTURE_XCODE_CONFIGURATION=Release sh "$runner" >"$fixture_root/appium-start" 2>&1 || { cat "$fixture_root/appium-start"; exit 1; }
grep -F 'appium --address 127.0.0.1 --port 4723 --use-plugins=altunity' "$FIXTURE_CALLS" >/dev/null
! kill -0 "$(sed -n '1p' "$FIXTURE_APPIUM_PID")" 2>/dev/null
grep -F 'Release-iphonesimulator/ConsentMessagePlugin.app' "$FIXTURE_CALLS" >/dev/null
printf 'PASS: iOS runner preflight, build, fresh-app handoff, no retries, and cleanup contracts\n'
