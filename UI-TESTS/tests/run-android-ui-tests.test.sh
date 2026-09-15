#!/bin/sh

set -eu

test_dir=$(CDPATH= cd -- "$(dirname -- "$0")" && pwd)
runner=$(CDPATH= cd -- "$test_dir/.." && pwd)/run-android-ui-tests.sh
output_file=$(mktemp)
fixture_root=$(mktemp -d)
trap 'rm -f "$output_file"; rm -rf "$fixture_root"' EXIT

sh "$runner" --help >"$output_file"

grep -F 'CMP_ANDROID_AVD' "$output_file" >/dev/null
grep -F 'UNITY_ANDROID_PLAYER_PATH' "$output_file" >/dev/null
grep -F 'CMP_UI_ARTIFACTS_DIR' "$output_file" >/dev/null
grep -F 'DOTNET_BIN' "$output_file" >/dev/null
grep -F 'immersive_mode_confirmations confirmed' "$runner" >/dev/null

mkdir -p "$fixture_root/android-player/SDK" "$fixture_root/android-player/NDK" \
    "$fixture_root/android-player/OpenJDK" "$fixture_root/android-sdk/platform-tools" \
    "$fixture_root/android-sdk/emulator" "$fixture_root/android-player-without-sdk/NDK" \
    "$fixture_root/android-player-without-sdk/OpenJDK" "$fixture_root/alt-tester"

cp "$test_dir/fixtures/command-succeeds.sh" "$fixture_root/android-sdk/platform-tools/adb"
cp "$test_dir/fixtures/emulator-with-requested-avd.sh" "$fixture_root/android-sdk/emulator/emulator"

if ! UNITY_PATH="$test_dir/fixtures/command-succeeds.sh" \
    UNITY_ANDROID_PLAYER_PATH="$fixture_root/android-player-without-sdk" \
    ANDROID_SDK_ROOT="$fixture_root/android-sdk" \
    ALTTESTER_DESKTOP_PATH="$fixture_root/alt-tester" \
    APPIUM_BIN="$test_dir/fixtures/appium-with-altunity-on-stderr.sh" \
    DOTNET_BIN=/opt/homebrew/opt/dotnet@8/bin/dotnet \
    sh "$runner" --preflight >"$output_file" 2>&1; then
    cat "$output_file" >&2
    exit 1
fi

if UNITY_PATH="$test_dir/fixtures/command-succeeds.sh" \
    UNITY_ANDROID_PLAYER_PATH="$fixture_root/android-player" \
    ALTTESTER_DESKTOP_PATH="$fixture_root/alt-tester" \
    ADB_BIN="$test_dir/fixtures/command-succeeds.sh" \
    EMULATOR_BIN="$test_dir/fixtures/emulator-without-requested-avd.sh" \
    sh "$runner" --preflight >"$output_file" 2>&1; then
    echo "Expected unavailable-AVD preflight to fail." >&2
    exit 1
fi

grep -F "Android AVD 'CMP_Unity_API_37' is unavailable" "$output_file" >/dev/null

if ! UNITY_PATH="$test_dir/fixtures/command-succeeds.sh" \
    UNITY_ANDROID_PLAYER_PATH="$fixture_root/android-player" \
    ALTTESTER_DESKTOP_PATH="$fixture_root/alt-tester" \
    ADB_BIN="$test_dir/fixtures/command-succeeds.sh" \
    EMULATOR_BIN="$test_dir/fixtures/emulator-with-requested-avd.sh" \
    APPIUM_BIN="$test_dir/fixtures/appium-with-altunity-on-stderr.sh" \
    DOTNET_BIN=/opt/homebrew/opt/dotnet@8/bin/dotnet \
    sh "$runner" --preflight >"$output_file" 2>&1; then
    cat "$output_file" >&2
    exit 1
fi
