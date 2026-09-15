#!/bin/sh

set -eu

results_dir=
test_name=
previous=

for argument in "$@"; do
    if [ "$previous" = "--results-directory" ]; then
        results_dir=$argument
    fi
    case "$argument" in
        Name=*) test_name=${argument#Name=} ;;
    esac
    previous=$argument
done

[ -n "$results_dir" ] || {
    echo "Fixture expected --results-directory." >&2
    exit 2
}

mkdir -p "$results_dir"
printf '%s\n' "$*" >>"$FIXTURE_STATE_DIR/calls.txt"

write_result() {
    result_name=$1
    outcome=$2
    printf '<TestRun><Results><UnitTestResult testName="%s" outcome="%s" /></Results></TestRun>\n' \
        "$result_name" "$outcome" >"$results_dir/results.trx"
    printf '%s: %s\n' "$result_name" "$outcome"
}

if [ -z "$test_name" ]; then
    case "${FIXTURE_INITIAL_MODE:-failed-tests}" in
        missing-trx)
            printf '%s\n' 'Fixture infrastructure failure before TRX creation.'
            exit 2
            ;;
        no-failed-record)
            write_result SetupProbe Passed
            exit 2
            ;;
        failed-tests) ;;
        *)
            echo "Unexpected fixture initial mode: $FIXTURE_INITIAL_MODE" >&2
            exit 2
            ;;
    esac
    if [ "${FIXTURE_INCLUDE_STABLE:-0}" = 1 ]; then
        {
            printf '%s\n' '<TestRun><Results>'
            printf '%s\n' '<UnitTestResult testName="FlakyTest" outcome="Failed" />'
            printf '%s\n' '<UnitTestResult testName="StableTest" outcome="Failed" />'
            printf '%s\n' '</Results></TestRun>'
        } >"$results_dir/results.trx"
    else
        write_result FlakyTest Failed
    fi
    exit 1
fi

attempt_file="$FIXTURE_STATE_DIR/$test_name-attempts"
attempt=0
if [ -f "$attempt_file" ]; then
    attempt=$(sed -n '1p' "$attempt_file")
fi
attempt=$((attempt + 1))
printf '%s\n' "$attempt" >"$attempt_file"

case "$test_name" in
    FlakyTest)
        if [ "$attempt" -eq 1 ]; then
            write_result "$test_name" Failed
            exit 1
        fi
        write_result "$test_name" Passed
        ;;
    StableTest)
        write_result "$test_name" Failed
        exit 1
        ;;
    *)
        echo "Unexpected filtered test: $test_name" >&2
        exit 2
        ;;
esac
