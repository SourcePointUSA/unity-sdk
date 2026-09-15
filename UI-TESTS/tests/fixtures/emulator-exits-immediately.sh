#!/bin/sh

set -eu

[ "${1:-}" = -avd ] || {
    echo "Expected -avd, got: $*" >&2
    exit 2
}

exit 7
