#!/bin/sh

set -eu

[ "$#" -eq 6 ] || {
    echo "Usage: $0 ADB EMULATOR AVD ARTIFACT_DIR ANDROID_SERIAL EMULATOR_PID_FILE" >&2
    exit 2
}

adb_bin=$1
emulator_bin=$2
avd=$3
artifact_dir=$4
android_serial=$5
emulator_pid_file=$6
wait_attempts=${CMP_ANDROID_WAIT_ATTEMPTS:-60}
wait_interval=${CMP_ANDROID_WAIT_INTERVAL:-2}
emulator_pid=

fail() {
    echo "ERROR: $*" >&2
    exit 1
}

find_online_emulator() {
    "$adb_bin" devices | awk '/^emulator-[0-9]+[[:space:]]+device$/ { print $1; exit }'
}

spawned_emulator_is_running() {
    [ -z "$emulator_pid" ] || kill -0 "$emulator_pid" 2>/dev/null
}

fail_if_spawned_emulator_exited() {
    [ -z "$emulator_pid" ] && return
    if ! spawned_emulator_is_running; then
        set +e
        wait "$emulator_pid"
        emulator_status=$?
        set -e
        fail "Android emulator process $emulator_pid exited before becoming ready (exit $emulator_status). See $artifact_dir/emulator.log."
    fi
}

timeout_spawned_emulator() {
    if [ -n "$emulator_pid" ] && spawned_emulator_is_running; then
        kill "$emulator_pid" 2>/dev/null || true
        wait "$emulator_pid" 2>/dev/null || true
    fi
}

wait_for_online_serial() {
    attempts=0
    while :; do
        fail_if_spawned_emulator_exited
        online_serial=$(find_online_emulator)
        if [ -n "$online_serial" ]; then
            android_serial=$online_serial
            return
        fi
        attempts=$((attempts + 1))
        if [ "$attempts" -ge "$wait_attempts" ]; then
            timeout_spawned_emulator
            fail "Timed out waiting for Android emulator device. See $artifact_dir."
        fi
        sleep "$wait_interval"
    done
}

boot_is_complete() {
    [ "$("$adb_bin" -s "$android_serial" shell getprop sys.boot_completed 2>/dev/null | tr -d '\r')" = 1 ]
}

wait_for_boot_completion() {
    attempts=0
    until boot_is_complete; do
        fail_if_spawned_emulator_exited
        attempts=$((attempts + 1))
        if [ "$attempts" -ge "$wait_attempts" ]; then
            timeout_spawned_emulator
            fail "Timed out waiting for Android boot completion. See $artifact_dir."
        fi
        sleep "$wait_interval"
    done
}

if [ -n "$android_serial" ]; then
    "$adb_bin" -s "$android_serial" get-state | grep -qx device ||
        fail "Configured ANDROID_SERIAL '$android_serial' is not an online device."
else
    android_serial=$(find_online_emulator)
    if [ -z "$android_serial" ]; then
        "$emulator_bin" -avd "$avd" -no-boot-anim -no-snapshot-save \
            >"$artifact_dir/emulator.log" 2>&1 &
        emulator_pid=$!
        printf '%s\n' "$emulator_pid" >"$emulator_pid_file"
        wait_for_online_serial
    fi
fi

wait_for_boot_completion
printf '%s\n' "$android_serial"
