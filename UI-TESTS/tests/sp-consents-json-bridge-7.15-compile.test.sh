#!/bin/sh

set -eu

test_dir=$(CDPATH= cd -- "$(dirname -- "$0")" && pwd)
project_root=$(CDPATH= cd -- "$test_dir/../.." && pwd)

UNITY_ANDROID_PLAYER_PATH=${UNITY_ANDROID_PLAYER_PATH:-/Applications/Unity/Installs/6000.5.10f1/PlaybackEngines/AndroidPlayer}
GRADLE_USER_HOME=${GRADLE_USER_HOME:-$HOME/.gradle}

javac_bin="$UNITY_ANDROID_PLAYER_PATH/OpenJDK/bin/javac"
android_jar=$(find "$UNITY_ANDROID_PLAYER_PATH/SDK/platforms" -name android.jar -type f | sort | tail -n 1)
cmp_aar=$(find "$GRADLE_USER_HOME/caches/modules-2/files-2.1/com.sourcepoint.cmplibrary/cmplibrary/7.15.13" -name '*.aar' -type f | head -n 1)
kotlin_jar=$(find "$GRADLE_USER_HOME/caches/modules-2/files-2.1/org.jetbrains.kotlin/kotlin-stdlib" -name 'kotlin-stdlib-*.jar' -type f | sort | tail -n 1)

for required in "$javac_bin" "$android_jar" "$cmp_aar" "$kotlin_jar"; do
    [ -f "$required" ] || {
        echo "Required CMP 7.15 bridge-compile input is missing: $required" >&2
        exit 1
    }
done

bridge_source="$project_root/Assets/Plugins/Android/SpConsentsJsonBridge.java"
work_dir=$(mktemp -d /private/tmp/sp-consents-json-bridge-715.XXXXXX)
trap 'rm -rf "$work_dir"' EXIT

mkdir -p "$work_dir/cmp" "$work_dir/classes"
unzip -q "$cmp_aar" classes.jar -d "$work_dir/cmp"

classpath="$work_dir/cmp/classes.jar:$kotlin_jar:$android_jar"
"$javac_bin" -classpath "$classpath" -d "$work_dir/classes" "$bridge_source"
