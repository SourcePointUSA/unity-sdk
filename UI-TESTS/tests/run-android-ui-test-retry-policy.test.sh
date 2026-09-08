#!/bin/sh

set -eu

test_dir=$(CDPATH= cd -- "$(dirname -- "$0")" && pwd)
policy_runner=$(CDPATH= cd -- "$test_dir/.." && pwd)/run-dotnet-ui-tests-with-retries.sh
fixture_dotnet="$test_dir/fixtures/dotnet-with-retry-results.sh"
fixture_adb="$test_dir/fixtures/command-succeeds.sh"
fixture_root=$(mktemp -d)
trap 'rm -rf "$fixture_root"' EXIT

[ -x "$policy_runner" ] || {
    echo "Retry policy runner is missing or not executable: $policy_runner" >&2
    exit 1
}

run_policy() {
    include_stable=$1
    scenario=$2
    scenario_root="$fixture_root/$scenario"
    mkdir -p "$scenario_root/state" "$scenario_root/artifacts"
    printf '%s\n' 'DEVICE=fixture-emulator' >"$scenario_root/effective-test-config.txt"
    : >"$scenario_root/app.apk"
    : >"$scenario_root/appium.log"

    if FIXTURE_STATE_DIR="$scenario_root/state" \
        FIXTURE_INCLUDE_STABLE="$include_stable" \
        DOTNET_BIN="$fixture_dotnet" \
        ADB_BIN="$fixture_adb" \
        ANDROID_SERIAL=fixture-emulator \
        CMP_UI_APPIUM_LOG="$scenario_root/appium.log" \
        sh "$policy_runner" \
            "$scenario_root/project.csproj" \
            "$scenario_root/android.runsettings" \
            "$scenario_root/app.apk" \
            "$scenario_root/artifacts" \
            "$scenario_root/effective-test-config.txt" \
            >"$scenario_root/policy-output.log" 2>&1; then
        status=0
    else
        status=$?
    fi
    printf '%s\n' "$status"
}

stable_status=$(run_policy 1 stable)
[ "$stable_status" -eq 1 ] || {
    echo "Expected a stable failure to exit 1, got $stable_status." >&2
    exit 1
}

stable_summary="$fixture_root/stable/artifacts/retry-summary.txt"
grep -F 'FlakyTest retry-1: failed' "$stable_summary" >/dev/null
grep -F 'FlakyTest retry-2: passed' "$stable_summary" >/dev/null
grep -F 'FlakyTest classification: flaky-pass' "$stable_summary" >/dev/null
grep -F 'StableTest retry-1: failed' "$stable_summary" >/dev/null
grep -F 'StableTest retry-2: failed' "$stable_summary" >/dev/null
grep -F 'StableTest retry-3: failed' "$stable_summary" >/dev/null
grep -F 'StableTest classification: stable-failure' "$stable_summary" >/dev/null
grep -F 'Stable failures: StableTest' "$stable_summary" >/dev/null

[ "$(grep -c -- '--filter Name=FlakyTest' "$fixture_root/stable/state/calls.txt")" -eq 2 ]
[ "$(grep -c -- '--filter Name=StableTest' "$fixture_root/stable/state/calls.txt")" -eq 3 ]
[ "$(grep -c -F 'TestRunParameters.Parameter(name="deviceName",value="Android Emulator")' "$fixture_root/stable/state/calls.txt")" -eq 6 ]
[ "$(grep -c -F 'TestRunParameters.Parameter(name="appium:udid",value="fixture-emulator")' "$fixture_root/stable/state/calls.txt")" -eq 6 ]

for attempt_dir in \
    "$fixture_root/stable/artifacts/test-attempts/initial" \
    "$fixture_root/stable/artifacts/test-attempts/FlakyTest/retry-1" \
    "$fixture_root/stable/artifacts/test-attempts/FlakyTest/retry-2" \
    "$fixture_root/stable/artifacts/test-attempts/StableTest/retry-1" \
    "$fixture_root/stable/artifacts/test-attempts/StableTest/retry-2" \
    "$fixture_root/stable/artifacts/test-attempts/StableTest/retry-3"; do
    test -f "$attempt_dir/results.trx"
    test -f "$attempt_dir/dotnet-test.log"
    test -f "$attempt_dir/logcat-final.txt"
    test -f "$attempt_dir/appium.log"
    test -f "$attempt_dir/effective-test-config.txt"
done

flaky_status=$(run_policy 0 flaky)
[ "$flaky_status" -eq 0 ] || {
    echo "Expected a flaky-pass-only run to exit 0, got $flaky_status." >&2
    exit 1
}

flaky_summary="$fixture_root/flaky/artifacts/retry-summary.txt"
grep -F 'FlakyTest classification: flaky-pass' "$flaky_summary" >/dev/null
grep -F 'Stable failures: none' "$flaky_summary" >/dev/null
[ "$(grep -c -- '--filter Name=FlakyTest' "$fixture_root/flaky/state/calls.txt")" -eq 2 ]
[ "$(grep -c -F 'TestRunParameters.Parameter(name="deviceName",value="Android Emulator")' "$fixture_root/flaky/state/calls.txt")" -eq 3 ]
[ "$(grep -c -F 'TestRunParameters.Parameter(name="appium:udid",value="fixture-emulator")' "$fixture_root/flaky/state/calls.txt")" -eq 3 ]
