#!/bin/sh
set -eu
printf 'pod cwd=%s %s\n' "$PWD" "$*" >>"$FIXTURE_CALLS"
[ "${1:-}" != --version ] || exit 0
[ "${1:-}" = install ]
mkdir -p Unity-iPhone.xcworkspace
fixture=good
[ "${FIXTURE_BAD_CMP:-0}" != 1 ] || fixture=wrong-cmp
cp "$FIXTURE_IOS_EXPORT/$fixture/Podfile.lock" Podfile.lock
