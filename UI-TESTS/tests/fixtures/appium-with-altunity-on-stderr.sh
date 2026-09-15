#!/bin/sh

if [ "${1:-}" = "driver" ]; then
    printf '%s\n' '{"uiautomator2":{"installed":true}}'
    exit 0
fi

if [ "${1:-}" = "plugin" ]; then
    printf '%s\n' 'altunity@1.3.3 [installed (npm)]' >&2
    exit 0
fi

exit 1
