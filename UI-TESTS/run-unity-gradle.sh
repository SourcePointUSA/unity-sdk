#!/bin/sh

set -eu

usage() {
    echo "Usage: $0 <unity-android-player-path> <gradle-project> <gradle-arguments...>" >&2
    exit 2
}

[ "$#" -ge 3 ] || usage

android_player_path=$1
gradle_project=$2
shift 2

java_bin="$android_player_path/OpenJDK/bin/java"
[ -x "$java_bin" ] || {
    echo "ERROR: Unity Java is missing or not executable: $java_bin" >&2
    exit 1
}
[ -d "$gradle_project/unityLibrary" ] || {
    echo "ERROR: Unity Gradle project is missing unityLibrary: $gradle_project" >&2
    exit 1
}

set -- "$android_player_path"/Tools/gradle/lib/gradle-launcher-*.jar "$@"
[ -f "$1" ] || {
    echo "ERROR: Unity Gradle launcher was not found beneath $android_player_path/Tools/gradle/lib." >&2
    exit 1
}
gradle_launcher=$1
shift

case "$1" in
    "$android_player_path"/Tools/gradle/lib/gradle-launcher-*.jar)
        echo "ERROR: Multiple Unity Gradle launchers were found beneath $android_player_path/Tools/gradle/lib." >&2
        exit 1
        ;;
esac

cd "$gradle_project"
exec "$java_bin" -classpath "$gradle_launcher" org.gradle.launcher.GradleMain "$@"
