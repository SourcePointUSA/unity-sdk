#!/bin/sh
set -eu
printf 'xcrun %s\n' "$*" >>"$FIXTURE_CALLS"
printf '{"devices":{"com.apple.CoreSimulator.SimRuntime.iOS-18-5":[]}}\n'
