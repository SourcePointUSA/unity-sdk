#!/bin/sh

set -eu

test_dir=$(CDPATH= cd -- "$(dirname -- "$0")" && pwd)
runner=$(CDPATH= cd -- "$test_dir/.." && pwd)/run-unity-gradle.sh
fixture_root=$(mktemp -d)
output_file=$(mktemp)
trap 'rm -rf "$fixture_root"; rm -f "$output_file"' EXIT

android_player="$fixture_root/android-player"
gradle_project="$fixture_root/gradle-project"
mkdir -p "$android_player/OpenJDK/bin" "$android_player/Tools/gradle/lib" \
    "$gradle_project/unityLibrary"
cp "$test_dir/fixtures/java-records-gradle-command.sh" "$android_player/OpenJDK/bin/java"
touch "$android_player/Tools/gradle/lib/gradle-launcher-9.1.0.jar"

sh "$runner" "$android_player" "$gradle_project" \
    :unityLibrary:dependencies --configuration releaseRuntimeClasspath >"$output_file"

grep -Fx "cwd=$gradle_project" "$output_file" >/dev/null
grep -Fx "arg=-classpath" "$output_file" >/dev/null
grep -Fx "arg=$android_player/Tools/gradle/lib/gradle-launcher-9.1.0.jar" "$output_file" >/dev/null
grep -Fx "arg=org.gradle.launcher.GradleMain" "$output_file" >/dev/null
grep -Fx "arg=:unityLibrary:dependencies" "$output_file" >/dev/null
grep -Fx "arg=--configuration" "$output_file" >/dev/null
grep -Fx "arg=releaseRuntimeClasspath" "$output_file" >/dev/null
