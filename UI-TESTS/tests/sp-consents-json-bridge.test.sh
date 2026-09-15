#!/bin/sh

set -eu

test_dir=$(CDPATH= cd -- "$(dirname -- "$0")" && pwd)
project_root=$(CDPATH= cd -- "$test_dir/../.." && pwd)

UNITY_ANDROID_PLAYER_PATH=${UNITY_ANDROID_PLAYER_PATH:-/Applications/Unity/Installs/6000.5.10f1/PlaybackEngines/AndroidPlayer}
GRADLE_USER_HOME=${GRADLE_USER_HOME:-$HOME/.gradle}

javac_bin="$UNITY_ANDROID_PLAYER_PATH/OpenJDK/bin/javac"
java_bin="$UNITY_ANDROID_PLAYER_PATH/OpenJDK/bin/java"
android_jar=$(find "$UNITY_ANDROID_PLAYER_PATH/SDK/platforms" -name android.jar -type f | sort | tail -n 1)
cmp_aar=$(find "$GRADLE_USER_HOME/caches/modules-2/files-2.1/com.sourcepoint.cmplibrary/cmplibrary/7.12.0" -name '*.aar' -type f | head -n 1)
core_aar=$(find "$GRADLE_USER_HOME/caches/modules-2/files-2.1/com.sourcepoint/core-android-debug/0.1.4" -name '*.aar' -type f | head -n 1)
kotlin_jar=$(find "$GRADLE_USER_HOME/caches/modules-2/files-2.1/org.jetbrains.kotlin/kotlin-stdlib" -name 'kotlin-stdlib-*.jar' -type f | sort | tail -n 1)
serialization_core_jar=$(find "$GRADLE_USER_HOME/caches/modules-2/files-2.1/org.jetbrains.kotlinx/kotlinx-serialization-core-jvm/1.7.3" -name '*.jar' -type f | head -n 1)
serialization_json_jar=$(find "$GRADLE_USER_HOME/caches/modules-2/files-2.1/org.jetbrains.kotlinx/kotlinx-serialization-json-jvm/1.7.3" -name '*.jar' -type f | head -n 1)

for required in "$javac_bin" "$java_bin" "$android_jar" "$cmp_aar" "$core_aar" "$kotlin_jar" "$serialization_core_jar" "$serialization_json_jar"; do
    [ -f "$required" ] || {
        echo "Required JVM bridge-test input is missing: $required" >&2
        exit 1
    }
done

fixture_root="$test_dir/fixtures/sp-consents-json-bridge"
bridge_source="$project_root/Assets/Plugins/Android/SpConsentsJsonBridge.java"
work_dir=$(mktemp -d /private/tmp/sp-consents-json-bridge.XXXXXX)
trap 'rm -rf "$work_dir"' EXIT

mkdir -p "$work_dir/cmp" "$work_dir/core" "$work_dir/classes"
unzip -q "$cmp_aar" classes.jar -d "$work_dir/cmp"
unzip -q "$core_aar" classes.jar -d "$work_dir/core"

classpath="$work_dir/cmp/classes.jar:$work_dir/core/classes.jar:$kotlin_jar:$serialization_core_jar:$serialization_json_jar:$android_jar"
"$javac_bin" -classpath "$classpath" -d "$work_dir/classes" \
    "$fixture_root/org/json/JSONObject.java" \
    "$fixture_root/org/json/JSONArray.java" \
    "$bridge_source" \
    "$fixture_root/com/sourcepoint/unity/SpConsentsJsonBridgeRegressionTest.java"

"$java_bin" -classpath "$work_dir/classes:$classpath" \
    com.sourcepoint.unity.SpConsentsJsonBridgeRegressionTest
