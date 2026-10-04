#!/bin/sh
set -eu
test_dir=$(CDPATH= cd -- "$(dirname -- "$0")" && pwd)
project_root=$(CDPATH= cd -- "$test_dir/../.." && pwd)
unity_contents=${UNITY_CONTENTS_PATH:-/Applications/Unity/Installs/6000.5.10f1/Unity.app/Contents}
command -v mcs >/dev/null
command -v mono >/dev/null
fixture_root=$(mktemp -d)
trap 'rm -rf "$fixture_root"' EXIT
managed="$unity_contents/Resources/Scripting/Managed"
engine="$managed/UnityEngine"
pbx="$unity_contents/Resources/BuildPipeline"
cp "$test_dir/fixtures/ios-postprocess-contract.cs.txt" "$fixture_root/contract.cs"
mcs -define:UNITY_IOS -out:"$fixture_root/postprocess.exe" \
    -r:"$engine/UnityEditor.CoreModule.dll" \
    -r:"$unity_contents/Resources/Scripting/MonoBleedingEdge/lib/mono/4.8-api/Facades/netstandard.dll" \
    -r:"$engine/UnityEngine.CoreModule.dll" -r:"$pbx/UnityEditor.iOS.Extensions.Xcode.dll" \
    -r:"$project_root/Library/ScriptAssemblies/Assembly-CSharp.dll" \
    "$project_root/Assets/ConsentManagementProvider/Editor/CMPPostProcessBuild.cs" \
    "$fixture_root/contract.cs"
MONO_PATH="$managed:$engine:$pbx:$project_root/Library/ScriptAssemblies" mono "$fixture_root/postprocess.exe" "$fixture_root/export"
cp "$test_dir/fixtures/ios-cmp-export/good/Podfile.lock" "$fixture_root/export/Podfile.lock"
sh "$test_dir/../assert-ios-cmp-export.sh" "$fixture_root/export" 7.12.11 0.1.17
