#!/bin/sh

set -eu

case "${1:-}" in
    devices)
        printf '%s\n' 'List of devices attached'
        ;;
    *)
        echo "Unexpected adb fixture arguments: $*" >&2
        exit 2
        ;;
esac
