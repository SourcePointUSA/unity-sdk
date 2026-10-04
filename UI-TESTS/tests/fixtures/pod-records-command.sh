#!/bin/sh
set -eu
printf 'pod cwd=%s %s\n' "$PWD" "$*" >>"$FIXTURE_CALLS"
[ "${1:-}" != --version ] || exit 0
[ "${1:-}" = install ]
mkdir -p Unity-iPhone.xcworkspace
printf 'PODS:\n  - ConsentViewController (7.7.1)\n' >Podfile.lock
