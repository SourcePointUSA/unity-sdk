#!/bin/sh

set -eu

script_dir=$(CDPATH= cd -- "$(dirname -- "$0")" && pwd)
project_root=$(CDPATH= cd -- "$script_dir/.." && pwd)

UNITY_PATH=${UNITY_PATH:-/Applications/Unity/Installs/6000.5.10f1/Unity.app/Contents/MacOS/Unity}
UNITY_ANDROID_PLAYER_PATH=${UNITY_ANDROID_PLAYER_PATH:-/Applications/Unity/Installs/6000.5.10f1/PlaybackEngines/AndroidPlayer}
ANDROID_SDK_ROOT=${ANDROID_SDK_ROOT:-$HOME/Library/Android/sdk}
CMP_ANDROID_AVD=${CMP_ANDROID_AVD:-CMP_Unity_API_37}
ANDROID_SERIAL=${ANDROID_SERIAL:-}
ALTTESTER_DESKTOP_PATH=${ALTTESTER_DESKTOP_PATH:-/Applications/AltTesterDesktop.app}
APPIUM_BIN=${APPIUM_BIN:-appium}
DOTNET_BIN=${DOTNET_BIN:-dotnet}
CMP_UI_ARTIFACTS_DIR=${CMP_UI_ARTIFACTS_DIR:-$script_dir/artifacts}
ALTTESTER_PORT=${ALTTESTER_PORT:-13000}
retry_policy_runner="$script_dir/run-dotnet-ui-tests-with-retries.sh"

adb_bin=${ADB_BIN:-}
emulator_bin=${EMULATOR_BIN:-}
appium_pid=
emulator_pid=
emulator_pid_file=
logcat_pid=
artifact_dir=

usage() {
    cat <<'EOF'
Usage: UI-TESTS/run-android-ui-tests.sh [--preflight]

Builds a fresh AltTester-instrumented Android APK and runs the full UI-TESTS suite.
Each initially failed test is retried individually up to three times. Every
attempt has dedicated TRX, console, Appium, logcat, and effective-config artifacts.

Environment overrides:
  UNITY_PATH                 Unity executable.
  UNITY_ANDROID_PLAYER_PATH  Unity AndroidPlayer directory (SDK/NDK/OpenJDK).
  ANDROID_SDK_ROOT           Android SDK used to find adb/emulator.
  CMP_ANDROID_AVD            AVD name (default: CMP_Unity_API_37).
  ANDROID_SERIAL             Existing Android serial; otherwise starts/finds an emulator.
  ALTTESTER_DESKTOP_PATH     AltTester Desktop .app path.
  APPIUM_BIN                 Appium command/path.
  DOTNET_BIN                 .NET SDK command/path.
  CMP_UI_ARTIFACTS_DIR       Parent directory for timestamped logs/results.

--preflight validates prerequisites only; it never starts Unity, Appium, or an emulator.
EOF
}

fail() {
    echo "ERROR: $*" >&2
    exit 1
}

require_executable() {
    [ -x "$1" ] || fail "$2 is missing or not executable: $1"
}

find_android_tools() {
    if [ -z "$adb_bin" ]; then
        adb_bin=$(command -v adb 2>/dev/null || true)
        [ -n "$adb_bin" ] || adb_bin="$ANDROID_SDK_ROOT/platform-tools/adb"
    fi
    if [ -z "$emulator_bin" ]; then
        emulator_bin=$(command -v emulator 2>/dev/null || true)
        [ -n "$emulator_bin" ] || emulator_bin="$ANDROID_SDK_ROOT/emulator/emulator"
    fi
}

preflight() {
    require_executable "$UNITY_PATH" "Unity 6000.5.10f1"
    [ -d "$UNITY_ANDROID_PLAYER_PATH/NDK" ] || fail "Unity Android NDK is missing: $UNITY_ANDROID_PLAYER_PATH/NDK"
    [ -d "$UNITY_ANDROID_PLAYER_PATH/OpenJDK" ] || fail "Unity Android OpenJDK is missing: $UNITY_ANDROID_PLAYER_PATH/OpenJDK"
    [ -d "$ALTTESTER_DESKTOP_PATH" ] || fail "AltTester Desktop is missing: $ALTTESTER_DESKTOP_PATH"
    [ -d "$ANDROID_SDK_ROOT" ] || fail "Android SDK is missing: $ANDROID_SDK_ROOT. Set ANDROID_SDK_ROOT to an Android SDK containing platform-tools and emulator."

    find_android_tools
    require_executable "$adb_bin" "adb"
    require_executable "$emulator_bin" "Android emulator"
    "$emulator_bin" -list-avds | grep -Fx "$CMP_ANDROID_AVD" >/dev/null || fail "Android AVD '$CMP_ANDROID_AVD' is unavailable; create it in Android Studio Device Manager."

    command -v "$DOTNET_BIN" >/dev/null 2>&1 || fail ".NET 8 is missing: $DOTNET_BIN"
    "$DOTNET_BIN" --list-sdks | grep -E '^[[:space:]]*8\.' >/dev/null || fail ".NET 8 SDK is missing; install a .NET 8 SDK."
    command -v "$APPIUM_BIN" >/dev/null 2>&1 || fail "Appium is missing: $APPIUM_BIN"
    "$APPIUM_BIN" driver list --installed --json | grep -q 'uiautomator2' || fail "Install Appium uiautomator2: appium driver install uiautomator2"
    "$APPIUM_BIN" plugin list --installed 2>&1 | grep -q 'altunity' || fail "Install Appium altunity: appium plugin install --source=npm appium-altunity-plugin"
    command -v curl >/dev/null 2>&1 || fail "curl is required for Appium readiness checks."
    command -v nc >/dev/null 2>&1 || fail "nc is required for AltTester port readiness checks."
}

find_unity_gradle_project() {
    gradle_project=
    for candidate in "$project_root"/Library/Bee/Android/Prj/*/Gradle; do
        [ -d "$candidate/unityLibrary" ] || continue
        gradle_project=$candidate
        break
    done
    [ -n "$gradle_project" ] || fail "Unity generated Gradle project was not found beneath Library/Bee/Android/Prj/*/Gradle with unityLibrary."
}

capture_and_assert_cmp_graph() {
    find_unity_gradle_project
    sh "$script_dir/run-unity-gradle.sh" "$UNITY_ANDROID_PLAYER_PATH" "$gradle_project" \
        :unityLibrary:dependencies --configuration releaseRuntimeClasspath \
        >"$artifact_dir/gradle-dependencies.txt" 2>&1

    cmp_version=$(sed -n 's/.*com\.sourcepoint\.cmplibrary:cmplibrary:\([0-9][0-9.]*\).*/\1/p' \
        "$project_root/Assets/ConsentManagementProvider/Editor/SourcepointDependencies.xml" | head -n 1)
    [ -n "$cmp_version" ] || fail "Could not determine the CMP version from SourcepointDependencies.xml."
    case "$cmp_version" in
        7.12.0|7.15.13)
            sh "$script_dir/assert-android-cmp-graph.sh" "$artifact_dir/gradle-dependencies.txt" "$cmp_version"
            ;;
        *)
            echo "CMP graph report saved for $cmp_version; no Stage 2 assertion contract is defined for this version."
            ;;
    esac
}

cleanup() {
    status=$?
    if [ -n "$artifact_dir" ] && [ -n "$adb_bin" ] && [ -x "$adb_bin" ]; then
        "$adb_bin" logcat -d -v threadtime >"$artifact_dir/logcat-final.txt" 2>&1 || true
    fi
    [ -z "$logcat_pid" ] || kill "$logcat_pid" 2>/dev/null || true
    [ -z "$appium_pid" ] || kill "$appium_pid" 2>/dev/null || true
    if [ -z "$emulator_pid" ] && [ -n "$emulator_pid_file" ] && [ -r "$emulator_pid_file" ]; then
        emulator_pid=$(sed -n '1p' "$emulator_pid_file")
    fi
    [ -z "$emulator_pid" ] || kill "$emulator_pid" 2>/dev/null || true
    exit "$status"
}

wait_for() {
    description=$1
    shift
    attempts=0
    until "$@"; do
        attempts=$((attempts + 1))
        [ "$attempts" -lt 60 ] || fail "Timed out waiting for $description. See $artifact_dir."
        sleep 2
    done
}

appium_is_ready() {
    curl -fsS http://127.0.0.1:4723/status >/dev/null 2>&1
}

start_emulator_if_needed() {
    emulator_pid_file="$artifact_dir/emulator.pid"
    ANDROID_SERIAL=$(sh "$script_dir/android-emulator-readiness.sh" \
        "$adb_bin" "$emulator_bin" "$CMP_ANDROID_AVD" "$artifact_dir" \
        "$ANDROID_SERIAL" "$emulator_pid_file")
    if [ -r "$emulator_pid_file" ]; then
        emulator_pid=$(sed -n '1p' "$emulator_pid_file")
    fi
}

start_appium_if_needed() {
    if appium_is_ready; then
        printf '%s\n' 'External Appium server was already ready at http://127.0.0.1:4723.' \
            >"$artifact_dir/appium.log"
        return
    fi
    "$APPIUM_BIN" --allow-insecure chromedriver_autodownload >"$artifact_dir/appium.log" 2>&1 &
    appium_pid=$!
    wait_for "Appium status endpoint" appium_is_ready
}

dismiss_immersive_mode_confirmation() {
    "$adb_bin" -s "$ANDROID_SERIAL" shell settings put secure immersive_mode_confirmations confirmed >/dev/null 2>&1 || true
}

run_tests() {
    mkdir -p "$CMP_UI_ARTIFACTS_DIR"
    artifact_dir="$CMP_UI_ARTIFACTS_DIR/$(date +%Y%m%d-%H%M%S)"
    mkdir -p "$artifact_dir"
    trap cleanup EXIT INT TERM

    cp "$script_dir/android.runsettings" "$artifact_dir/android.runsettings"
    start_emulator_if_needed

    {
        printf 'UNITY_PATH=%s\n' "$UNITY_PATH"
        printf 'UNITY_ANDROID_PLAYER_PATH=%s\n' "$UNITY_ANDROID_PLAYER_PATH"
        printf 'ANDROID_SDK_ROOT=%s\n' "$ANDROID_SDK_ROOT"
        printf 'CMP_ANDROID_AVD=%s\n' "$CMP_ANDROID_AVD"
        printf 'ANDROID_SERIAL=%s\n' "$ANDROID_SERIAL"
        printf 'ALTTESTER_DESKTOP_PATH=%s\n' "$ALTTESTER_DESKTOP_PATH"
        printf 'APPIUM_BIN=%s\n' "$APPIUM_BIN"
        printf 'DOTNET_BIN=%s\n' "$DOTNET_BIN"
        printf 'ALTTESTER_PORT=%s\n' "$ALTTESTER_PORT"
    } >"$artifact_dir/effective-test-config.txt"
    "$DOTNET_BIN" restore "$script_dir/UI-TESTS.csproj"
    "$DOTNET_BIN" test "$script_dir/UI-TESTS.csproj" --no-restore --list-tests >"$artifact_dir/test-discovery.txt"
    test_count=$(awk '
        /^The following Tests are available:$/ { discovered = 1; next }
        discovered && /^    [^[:space:]]/ { count += 1 }
        END { print count + 0 }
    ' "$artifact_dir/test-discovery.txt")
    [ "$test_count" -gt 0 ] || fail "No UI tests were discovered after restore; see $artifact_dir/test-discovery.txt"
    echo "Discovered $test_count UI tests."

    dismiss_immersive_mode_confirmation
    "$adb_bin" -s "$ANDROID_SERIAL" logcat -c
    "$adb_bin" -s "$ANDROID_SERIAL" logcat -v threadtime >"$artifact_dir/logcat.txt" 2>&1 &
    logcat_pid=$!
    start_appium_if_needed

    "$UNITY_PATH" -batchmode -quit -nographics -projectPath "$project_root" \
        -executeMethod AndroidUiTestBuild.Build \
        -cmpUiTestApk "$artifact_dir/ConsentMessagePlugin.apk" \
        -logFile "$artifact_dir/unity.log"
    [ -s "$artifact_dir/ConsentMessagePlugin.apk" ] || fail "Unity completed without a fresh APK at $artifact_dir/ConsentMessagePlugin.apk"
    capture_and_assert_cmp_graph

    open "$ALTTESTER_DESKTOP_PATH"
    "$adb_bin" -s "$ANDROID_SERIAL" install -r "$artifact_dir/ConsentMessagePlugin.apk" >"$artifact_dir/adb-install.log" 2>&1
    "$adb_bin" -s "$ANDROID_SERIAL" shell monkey -p com.DefaultCompany.ConsentMessagePlugin 1 >"$artifact_dir/adb-launch.log" 2>&1 || true
    wait_for "AltTester server port $ALTTESTER_PORT" nc -z 127.0.0.1 "$ALTTESTER_PORT"

    DOTNET_BIN="$DOTNET_BIN" ADB_BIN="$adb_bin" ANDROID_SERIAL="$ANDROID_SERIAL" \
        CMP_UI_APPIUM_LOG="$artifact_dir/appium.log" \
        sh "$retry_policy_runner" \
            "$script_dir/UI-TESTS.csproj" \
            "$script_dir/android.runsettings" \
            "$artifact_dir/ConsentMessagePlugin.apk" \
            "$artifact_dir" \
            "$artifact_dir/effective-test-config.txt"
}

case "${1:-}" in
    --help|-h)
        usage
        ;;
    --preflight)
        preflight
        ;;
    '')
        preflight
        run_tests
        ;;
    *)
        usage >&2
        exit 2
        ;;
esac
