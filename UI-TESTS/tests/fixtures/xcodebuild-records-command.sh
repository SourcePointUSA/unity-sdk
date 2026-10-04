#!/bin/sh
set -eu
printf 'xcodebuild %s\n' "$*" >>"$FIXTURE_CALLS"
[ "${1:-}" != -version ] || exit 0
derived_data=
while [ "$#" -gt 0 ]; do
    if [ "$1" = -derivedDataPath ]; then
        shift
        derived_data=$1
    fi
    shift
done
[ -n "$derived_data" ]
[ "${FIXTURE_XCODE_FAILURE:-0}" = 0 ] || exit 65
product="$derived_data/Build/Products/${FIXTURE_XCODE_CONFIGURATION:-Debug}-iphonesimulator/ConsentMessagePlugin.app"
mkdir -p "$product"
printf '<?xml version="1.0"?><plist version="1.0"><dict><key>CFBundleIdentifier</key><string>com.DefaultCompany.ConsentMessagePlugin</string></dict></plist>\n' >"$product/Info.plist"
