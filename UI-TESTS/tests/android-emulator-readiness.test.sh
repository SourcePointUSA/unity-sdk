#!/bin/sh

set -eu

test_dir=$(CDPATH= cd -- "$(dirname -- "$0")" && pwd)
readiness_script=$(CDPATH= cd -- "$test_dir/.." && pwd)/android-emulator-readiness.sh
fixture_root=$(mktemp -d)
trap 'rm -rf "$fixture_root"' EXIT

mkdir -p "$fixture_root/existing" "$fixture_root/dead"

existing_serial=$(CMP_ANDROID_WAIT_ATTEMPTS=2 CMP_ANDROID_WAIT_INTERVAL=0 \
    sh "$readiness_script" \
        "$test_dir/fixtures/adb-with-existing-target.sh" \
        "$test_dir/fixtures/emulator-exits-immediately.sh" \
        CMP_Unity_API_37 \
        "$fixture_root/existing" \
        existing-emulator \
        "$fixture_root/existing/emulator.pid")
[ "$existing_serial" = existing-emulator ]
[ ! -e "$fixture_root/existing/emulator.pid" ]

start_time=$(date +%s)
if CMP_ANDROID_WAIT_ATTEMPTS=30 CMP_ANDROID_WAIT_INTERVAL=1 \
    sh "$readiness_script" \
        "$test_dir/fixtures/adb-never-sees-emulator.sh" \
        "$test_dir/fixtures/emulator-exits-immediately.sh" \
        CMP_Unity_API_37 \
        "$fixture_root/dead" \
        '' \
        "$fixture_root/dead/emulator.pid" \
        >"$fixture_root/dead/output.log" 2>&1; then
    echo 'Expected a runner-spawned emulator that exits immediately to fail.' >&2
    exit 1
fi
elapsed=$(( $(date +%s) - start_time ))
[ "$elapsed" -lt 5 ] || {
    echo "Expected exited emulator detection before the 30-second deadline; took ${elapsed}s." >&2
    exit 1
}
grep -F 'exited before becoming ready' "$fixture_root/dead/output.log" >/dev/null
