#!/bin/sh
set -eu
test_dir=$(CDPATH= cd -- "$(dirname -- "$0")" && pwd)
project_root=$(CDPATH= cd -- "$test_dir/../.." && pwd)
unity_contents=${UNITY_CONTENTS_PATH:-/Applications/Unity/Installs/6000.5.10f1/Unity.app/Contents}
mono_bin="$unity_contents/Resources/Scripting/MonoBleedingEdge/bin/mono"
mcs_exe="$unity_contents/Resources/Scripting/MonoBleedingEdge/lib/mono/4.5/mcs.exe"
fixture_root=$(mktemp -d)
trap 'rm -rf "$fixture_root"' EXIT
managed="$unity_contents/Resources/Scripting/Managed"
engine="$managed/UnityEngine"
pbx="$unity_contents/Resources/BuildPipeline"
cp "$test_dir/fixtures/ios-podfile-contract.cs.txt" "$fixture_root/contract.cs"
"$mono_bin" "$mcs_exe" -define:UNITY_IOS -out:"$fixture_root/podfile.exe" \
    -r:"$engine/UnityEditor.CoreModule.dll" \
    -r:"$unity_contents/Resources/Scripting/MonoBleedingEdge/lib/mono/4.8-api/Facades/netstandard.dll" \
    -r:"$engine/UnityEngine.CoreModule.dll" -r:"$pbx/UnityEditor.iOS.Extensions.Xcode.dll" \
    -r:"$project_root/Library/ScriptAssemblies/Assembly-CSharp.dll" \
    "$project_root/Assets/ConsentManagementProvider/Editor/CMPPostProcessBuild.cs" "$fixture_root/contract.cs"
export MONO_PATH="$managed:$engine:$pbx:$project_root/Library/ScriptAssemblies"
if [ "$#" -eq 2 ]; then
    "$mono_bin" "$fixture_root/podfile.exe" "$1" "$2"
    exit
fi
"$mono_bin" "$fixture_root/podfile.exe" "$fixture_root/Podfile"
ruby "$test_dir/fixtures/ios-podfile-hook.rb" "$fixture_root/Podfile" 15.0
ruby "$test_dir/fixtures/ios-podfile-hook.rb" "$fixture_root/Podfile.raised" 16.0
