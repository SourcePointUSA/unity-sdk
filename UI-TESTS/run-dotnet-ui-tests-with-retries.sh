#!/bin/sh

set -eu

[ "$#" -eq 5 ] || {
    echo "Usage: $0 TEST_PROJECT RUNSETTINGS APK ARTIFACT_DIR EFFECTIVE_CONFIG" >&2
    exit 2
}

test_project=$1
runsettings=$2
apk_path=$3
artifact_dir=$4
effective_config=$5

DOTNET_BIN=${DOTNET_BIN:-dotnet}
ADB_BIN=${ADB_BIN:-adb}
ANDROID_SERIAL=${ANDROID_SERIAL:-}
CMP_UI_APPIUM_LOG=${CMP_UI_APPIUM_LOG:-}

attempts_dir="$artifact_dir/test-attempts"
initial_dir="$attempts_dir/initial"
summary_file="$artifact_dir/retry-summary.txt"
failed_tests_file="$artifact_dir/initial-failed-tests.txt"

mkdir -p "$initial_dir"
: >"$summary_file"

record() {
    printf '%s\n' "$*"
    printf '%s\n' "$*" >>"$summary_file"
}

copy_attempt_context() {
    context_dir=$1
    cp "$effective_config" "$context_dir/effective-test-config.txt"
    if [ -n "$CMP_UI_APPIUM_LOG" ] && [ -f "$CMP_UI_APPIUM_LOG" ]; then
        cp "$CMP_UI_APPIUM_LOG" "$context_dir/appium.log"
    else
        printf '%s\n' 'Appium log unavailable; an external Appium server may be in use.' \
            >"$context_dir/appium.log"
    fi
}

capture_attempt_logcat() {
    log_dir=$1
    if [ -n "$ANDROID_SERIAL" ] && command -v "$ADB_BIN" >/dev/null 2>&1; then
        "$ADB_BIN" -s "$ANDROID_SERIAL" logcat -d -v threadtime \
            >"$log_dir/logcat-final.txt" 2>&1 || true
    else
        printf '%s\n' 'Android logcat unavailable.' >"$log_dir/logcat-final.txt"
    fi
}

clear_attempt_logcat() {
    if [ -n "$ANDROID_SERIAL" ] && command -v "$ADB_BIN" >/dev/null 2>&1; then
        "$ADB_BIN" -s "$ANDROID_SERIAL" logcat -c >/dev/null 2>&1 || true
    fi
}

run_attempt() {
    run_dir=$1
    filter_name=$2
    mkdir -p "$run_dir"
    copy_attempt_context "$run_dir"
    clear_attempt_logcat

    if [ -n "$filter_name" ]; then
        if "$DOTNET_BIN" test "$test_project" --no-restore --settings "$runsettings" \
            --filter "Name=$filter_name" \
            --logger "trx;LogFileName=results.trx" --results-directory "$run_dir" -- \
            "TestRunParameters.Parameter(name=\"deviceName\",value=\"$ANDROID_SERIAL\")" \
            "TestRunParameters.Parameter(name=\"appium:app\",value=\"$apk_path\")" \
            >"$run_dir/dotnet-test.log" 2>&1; then
            attempt_status=0
        else
            attempt_status=$?
        fi
    else
        if "$DOTNET_BIN" test "$test_project" --no-restore --settings "$runsettings" \
            --logger "trx;LogFileName=results.trx" --results-directory "$run_dir" -- \
            "TestRunParameters.Parameter(name=\"deviceName\",value=\"$ANDROID_SERIAL\")" \
            "TestRunParameters.Parameter(name=\"appium:app\",value=\"$apk_path\")" \
            >"$run_dir/dotnet-test.log" 2>&1; then
            attempt_status=0
        else
            attempt_status=$?
        fi
    fi

    cat "$run_dir/dotnet-test.log"
    capture_attempt_logcat "$run_dir"
    copy_attempt_context "$run_dir"
    return "$attempt_status"
}

trx_outcomes() {
    trx_file=$1
    sed 's/></>\
</g' "$trx_file" | sed -n '/<UnitTestResult /p'
}

retry_result_passed() {
    retry_trx=$1
    [ -f "$retry_trx" ] || return 1
    outcomes=$(trx_outcomes "$retry_trx")
    printf '%s\n' "$outcomes" | grep 'outcome="Passed"' >/dev/null || return 1
    if printf '%s\n' "$outcomes" | grep 'outcome="Failed"' >/dev/null; then
        return 1
    fi
}

record "Initial full-suite attempt: $initial_dir"
if run_attempt "$initial_dir" ''; then
    initial_status=0
else
    initial_status=$?
fi

if [ -f "$initial_dir/results.trx" ]; then
    cp "$initial_dir/results.trx" "$artifact_dir/results.trx"
else
    record "Initial result: failed (no TRX produced; exit $initial_status)"
    record 'Stable failures: unclassified-initial-run'
    exit "${initial_status:-1}"
fi

if [ "$initial_status" -eq 0 ]; then
    record "Initial result: passed ($initial_dir/results.trx)"
    record 'Stable failures: none'
    exit 0
fi

trx_outcomes "$initial_dir/results.trx" | sed -n \
    '/outcome="Failed"/s/.*testName="\([^"]*\)".*/\1/p' | awk '!seen[$0]++' \
    >"$failed_tests_file"

if [ ! -s "$failed_tests_file" ]; then
    record "Initial result: failed without a failed test record (exit $initial_status)"
    record 'Stable failures: unclassified-initial-run'
    exit "$initial_status"
fi

stable_failures=
while IFS= read -r test_name; do
    [ -n "$test_name" ] || continue
    safe_test_name=$(printf '%s' "$test_name" | tr -c 'A-Za-z0-9._-' '_')
    record "$test_name initial: failed ($initial_dir/results.trx)"
    retry=1
    passed=0
    while [ "$retry" -le 3 ]; do
        retry_dir="$attempts_dir/$safe_test_name/retry-$retry"
        record "$test_name retry-$retry: running ($retry_dir)"
        if run_attempt "$retry_dir" "$test_name" && \
            retry_result_passed "$retry_dir/results.trx"; then
            record "$test_name retry-$retry: passed ($retry_dir/results.trx)"
            passed=1
            break
        fi
        record "$test_name retry-$retry: failed ($retry_dir/results.trx)"
        retry=$((retry + 1))
    done

    if [ "$passed" -eq 1 ]; then
        record "$test_name classification: flaky-pass"
    else
        record "$test_name classification: stable-failure"
        if [ -z "$stable_failures" ]; then
            stable_failures=$test_name
        else
            stable_failures="$stable_failures,$test_name"
        fi
    fi
done <"$failed_tests_file"

if [ -n "$stable_failures" ]; then
    record "Stable failures: $(printf '%s' "$stable_failures" | tr ',' ' ')"
    exit 1
fi

record 'Stable failures: none'
exit 0
