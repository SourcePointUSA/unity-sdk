#!/bin/sh

set -eu

test_dir=$(CDPATH= cd -- "$(dirname -- "$0")" && pwd)
policy_runner=$(CDPATH= cd -- "$test_dir/.." && pwd)/run-dotnet-ui-tests-with-retries.sh
fixture_dotnet="$test_dir/fixtures/dotnet-with-retry-results.sh"
fixture_adb="$test_dir/fixtures/command-succeeds.sh"
fixture_root=$(mktemp -d)
trap 'rm -rf "$fixture_root"' EXIT

run_inconclusive_case() {
    initial_mode=$1
    expected_reason=$2
    scenario_root="$fixture_root/$initial_mode"
    mkdir -p "$scenario_root/state" "$scenario_root/artifacts"
    printf '%s\n' 'DEVICE=fixture-emulator' >"$scenario_root/effective-test-config.txt"
    : >"$scenario_root/app.apk"
    : >"$scenario_root/appium.log"

    if FIXTURE_STATE_DIR="$scenario_root/state" \
        FIXTURE_INITIAL_MODE="$initial_mode" \
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
        echo "Expected $initial_mode to exit nonzero." >&2
        exit 1
    fi

    summary="$scenario_root/artifacts/retry-summary.txt"
    if ! grep -F 'Initial classification: infrastructure/inconclusive' "$summary" >/dev/null; then
        cat "$summary" >&2
        echo "Expected explicit infrastructure/inconclusive classification." >&2
        exit 1
    fi
    if ! grep -F "Infrastructure/inconclusive reason: $expected_reason" "$summary" >/dev/null; then
        cat "$summary" >&2
        echo "Expected inconclusive reason: $expected_reason" >&2
        exit 1
    fi
    if grep -F 'Stable failures:' "$summary" >/dev/null; then
        echo "Inconclusive initial failure was incorrectly labeled stable." >&2
        exit 1
    fi

    [ "$(wc -l <"$scenario_root/state/calls.txt" | tr -d ' ')" -eq 1 ]
    test ! -d "$scenario_root/artifacts/test-attempts/FlakyTest"
    test -f "$scenario_root/artifacts/test-attempts/initial/dotnet-test.log"
    test -f "$scenario_root/artifacts/test-attempts/initial/logcat-final.txt"
    test -f "$scenario_root/artifacts/test-attempts/initial/appium.log"
    test -f "$scenario_root/artifacts/test-attempts/initial/effective-test-config.txt"
}

run_inconclusive_case missing-trx 'no TRX produced (dotnet test exit 2)'
run_inconclusive_case no-failed-record 'nonzero dotnet test exit 2 with no failed test record'
