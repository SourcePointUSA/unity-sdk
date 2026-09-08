#!/bin/sh

set -eu

if [ "${1:-}" = -s ] && [ "${2:-}" = existing-emulator ] && [ "${3:-}" = get-state ]; then
    printf '%s\n' device
    exit 0
fi

if [ "${1:-}" = -s ] && [ "${2:-}" = existing-emulator ] &&
    [ "${3:-}" = shell ] && [ "${4:-}" = getprop ] && [ "${5:-}" = sys.boot_completed ]; then
    printf '%s\n' 1
    exit 0
fi

echo "Unexpected adb fixture arguments: $*" >&2
exit 2
