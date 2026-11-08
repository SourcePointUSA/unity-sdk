#!/bin/sh
set -eu

test_dir=$(CDPATH= cd -- "$(dirname -- "$0")" && pwd)
assertion="$test_dir/../assert-ios-cmp-export.sh"
[ -f "$assertion" ] || { echo 'FAIL: iOS export assertion is absent' >&2; exit 1; }
fixture_root=$(mktemp -d)
trap 'rm -rf "$fixture_root"' EXIT
fixtures="$test_dir/fixtures/ios-cmp-export"

sh "$assertion" "$fixtures/good" 7.12.11 0.1.17 >"$fixture_root/good.log" 2>&1 || {
    cat "$fixture_root/good.log"; echo 'FAIL: Unity-owned export was rejected' >&2; exit 1;
}
for value in 'ConsentViewController (7.12.11)' 'SPMobileCore (= 0.1.17)' 'SPMobileCore (0.1.17)' SwiftBridge.swift UnityFramework; do
    grep -F "$value" "$fixture_root/good.log" >/dev/null
done

# Each wrong fixture replaces only its relevant boundary input. A gate that
# accepts mismatched resolved versions or a bridge in another target fails.
expect_failure() {
    name=$1
    expected=$2
    mkdir -p "$fixture_root/$name/Unity-iPhone.xcodeproj"
    cp "$fixtures/good/Podfile.lock" "$fixture_root/$name/Podfile.lock"
    cp "$fixtures/good/Unity-iPhone.xcodeproj/project.pbxproj" "$fixture_root/$name/Unity-iPhone.xcodeproj/project.pbxproj"
    if [ -f "$fixtures/$name/Podfile.lock" ]; then
        cp "$fixtures/$name/Podfile.lock" "$fixture_root/$name/Podfile.lock"
    else
        cp "$fixtures/$name/Unity-iPhone.xcodeproj/project.pbxproj" "$fixture_root/$name/Unity-iPhone.xcodeproj/project.pbxproj"
    fi
    if sh "$assertion" "$fixture_root/$name" 7.12.11 0.1.17 >"$fixture_root/$name.log" 2>&1; then
        echo "FAIL: $name export passed" >&2; exit 1
    fi
    grep -F "$expected" "$fixture_root/$name.log" >/dev/null || {
        cat "$fixture_root/$name.log"; echo "FAIL: $name did not name $expected" >&2; exit 1;
    }
}
expect_failure wrong-cmp 'ConsentViewController (7.12.11)'
expect_failure wrong-core 'SPMobileCore (0.1.17)'
expect_failure wrong-core-dependency 'SPMobileCore (= 0.1.17)'
expect_failure pod-owned-bridge 'Pods/ConsentViewController'
expect_failure wrong-target UnityFramework
expect_failure unbuilt-bridge UnityFramework
expect_failure pod-group-bridge 'Pods/ConsentViewController'
printf 'PASS: iOS resolved versions and UnityFramework bridge ownership\n'
