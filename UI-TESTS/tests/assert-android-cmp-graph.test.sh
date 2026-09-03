#!/bin/sh

set -eu

test_dir=$(CDPATH= cd -- "$(dirname -- "$0")" && pwd)
assertion_script=$(CDPATH= cd -- "$test_dir/.." && pwd)/assert-android-cmp-graph.sh
fixtures_dir=$test_dir/fixtures/gradle-reports
output_file=$(mktemp)
trap 'rm -f "$output_file"' EXIT

assert_passes() {
    report=$1
    version=$2
    if ! sh "$assertion_script" "$fixtures_dir/$report" "$version" >"$output_file" 2>&1; then
        cat "$output_file" >&2
        exit 1
    fi
}

assert_fails_with() {
    report=$1
    version=$2
    expected=$3
    if sh "$assertion_script" "$fixtures_dir/$report" "$version" >"$output_file" 2>&1; then
        echo "Expected $report to fail for CMP $version." >&2
        exit 1
    fi
    grep -F "$expected" "$output_file" >/dev/null
}

assert_passes good-7.12.0.txt 7.12.0
assert_passes good-7.15.13.txt 7.15.13
assert_fails_with selected-cmp-core-mismatch.txt 7.12.0 'com.sourcepoint.cmplibrary:cmplibrary:7.12.0 -> 7.15.13'
assert_fails_with selected-core-mismatch.txt 7.12.0 'com.sourcepoint:core:0.1.4 -> 0.1.16'
assert_fails_with ktor-2-duplicate.txt 7.12.0 'io.ktor:ktor-client-core:2.3.9'
assert_fails_with ktor-3.1-selected.txt 7.12.0 'io.ktor:ktor-client-core:3.0.3 -> 3.1.3'
assert_fails_with missing-mobile-core.txt 7.12.0 'com.sourcepoint:core:0.1.4'
assert_fails_with legacy-local-jar.txt 7.15.13 'Assets/Plugins/Android/ktor-client-core-3.1.3.jar'
assert_fails_with good-7.12.0.txt 7.10.1 'Unsupported CMP version: 7.10.1'
