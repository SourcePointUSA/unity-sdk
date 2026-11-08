#!/bin/sh
set -eu
printf 'xcrun %s\n' "$*" >>"$FIXTURE_CALLS"
[ "${1:-}" != simctl ] || shift
case "${1:-}" in
    list)
        printf '{"devices":{"com.apple.CoreSimulator.SimRuntime.iOS-18-5":[{"name":"iPhone 14","udid":"fixture-udid","state":"%s","isAvailable":true}' "${FIXTURE_SIMULATOR_STATE:-Shutdown}"
        if [ "${FIXTURE_AMBIGUOUS:-0}" = 1 ]; then
            printf ',{"name":"iPhone 14","udid":"second-udid","state":"Shutdown","isAvailable":true}'
        fi
        printf ']}}\n'
        ;;
    boot|bootstatus|shutdown|install|launch) : ;;
    *) exit 2 ;;
esac
