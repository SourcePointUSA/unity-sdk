#!/bin/sh
set -eu
printf 'xcodebuild %s\n' "$*" >>"$FIXTURE_CALLS"
[ "${1:-}" != -version ] || exit 0
derived_data=
symroot=
while [ "$#" -gt 0 ]; do
    if [ "$1" = -derivedDataPath ]; then
        shift
        derived_data=$1
    elif [ "$1" = SYMROOT=* ]; then
        symroot=${1#SYMROOT=}
    fi
    shift
done
[ -n "$derived_data" ]
[ "${FIXTURE_XCODE_FAILURE:-0}" = 0 ] || exit 65
# A Unity/CocoaPods project may override the derived-data product directory. The
# runner must explicitly pin SYMROOT, and Xcode gives a command-line setting
# precedence over the generated project setting.
[ -n "$symroot" ] || { echo 'missing explicit SYMROOT' >&2; exit 1; }
product="$symroot/${FIXTURE_XCODE_CONFIGURATION:-ReleaseForRunning}-iphonesimulator/ConsentMessagePlugin.app"
mkdir -p "$product"
printf '<?xml version="1.0"?><plist version="1.0"><dict><key>CFBundleIdentifier</key><string>com.DefaultCompany.ConsentMessagePlugin</string></dict></plist>\n' >"$product/Info.plist"
if [ "${FIXTURE_MULTIPLE_APPS:-0}" = 1 ]; then
    product="$symroot/Other-iphonesimulator/ConsentMessagePlugin.app"
    mkdir -p "$product"
    printf '<?xml version="1.0"?><plist version="1.0"><dict><key>CFBundleIdentifier</key><string>com.DefaultCompany.ConsentMessagePlugin</string></dict></plist>\n' >"$product/Info.plist"
fi
