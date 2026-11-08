#!/bin/sh
set -eu
test_dir=$(CDPATH= cd -- "$(dirname -- "$0")" && pwd)
bridge="$test_dir/../../Assets/ConsentManagementProvider/Plugins/iOS/Source/SwiftBridge.swift"
fixture_root=$(mktemp -d)
trap 'rm -rf "$fixture_root"' EXIT

# Compile the bridge-owned logger and the actual logger declaration/print
# consumer without the pod's internal implementation. This catches references
# to inaccessible pod types using Swift's compiler, not a source-text ban.
printf 'import Foundation\n' >"$fixture_root/logging.swift"
awk '
    /^private struct UnityBridgeLogger/ { copying = 1 }
    /^@objc public class SwiftBridge/ { copying = 0 }
    copying { print }
' "$bridge" >>"$fixture_root/logging.swift"
printf 'class SwiftBridge {\n' >>"$fixture_root/logging.swift"
awk '/^[[:space:]]+(private )?let logger/ { print }' "$bridge" >>"$fixture_root/logging.swift"
awk '
    /public func print\(/ { copying = 1 }
    copying {
        print
        line = $0
        depth += gsub(/\{/, "{", line) - gsub(/\}/, "}", line)
        if (depth == 0) copying = 0
    }
' "$bridge" >>"$fixture_root/logging.swift"
printf '}\n' >>"$fixture_root/logging.swift"
awk '
    /^public func printLog\(/ { copying = 1 }
    copying { print; if ($0 == "}") copying = 0 }
' "$bridge" >>"$fixture_root/logging.swift"
printf 'extension SwiftBridge { func delegateEvent() { logger.log("delegate-event") } }\nlet bridge = SwiftBridge()\nbridge.delegateEvent()\nbridge.print("debug-print")\n' >>"$fixture_root/logging.swift"
mkdir -p "$fixture_root/Logging.app/Contents/MacOS"
xcrun swiftc -module-cache-path "$fixture_root/module-cache" "$fixture_root/logging.swift" -o "$fixture_root/Logging.app/Contents/MacOS/Logging"

check_level() {
    name=$1
    value=$2
    expected=$3
    printf '<?xml version="1.0"?><plist version="1.0"><dict><key>CFBundleIdentifier</key><string>com.sourcepoint.logging-contract</string><key>CFBundleExecutable</key><string>Logging</string>%s</dict></plist>\n' "$value" >"$fixture_root/Logging.app/Contents/Info.plist"
    "$fixture_root/Logging.app/Contents/MacOS/Logging" >"$fixture_root/$name.log" 2>&1
    if [ "$expected" = debug ]; then
        grep -F delegate-event "$fixture_root/$name.log" >/dev/null
        grep -F debug-print "$fixture_root/$name.log" >/dev/null
    else
        [ ! -s "$fixture_root/$name.log" ] || { cat "$fixture_root/$name.log"; echo "FAIL: $name enabled debug logging" >&2; exit 1; }
    fi
}
check_level debug '<key>SPLogLevel</key><string>debug</string>' debug
check_level prod '<key>SPLogLevel</key><string>prod</string>' quiet
check_level missing '' quiet
check_level invalid '<key>SPLogLevel</key><string>unexpected</string>' quiet
printf 'PASS: Unity-owned Swift logging compiles without pod internals and honors SPLogLevel\n'
