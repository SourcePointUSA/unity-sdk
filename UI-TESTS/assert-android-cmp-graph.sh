#!/bin/sh

set -eu

usage() {
    echo "Usage: $0 <gradle-dependencies.txt> <cmp-version>" >&2
    exit 2
}

fail_missing() {
    echo "ERROR: Missing required coordinate: $1" >&2
    exit 1
}

fail_forbidden() {
    echo "ERROR: Forbidden coordinate: $1" >&2
    exit 1
}

require_coordinate() {
    coordinate=$1
    if ! grep -F "$coordinate" "$report" >/dev/null; then
        fail_missing "$coordinate"
    fi
}

print_coordinate() {
    grep -F "$1" "$report"
}

[ "$#" -eq 2 ] || usage

report=$1
cmp_version=$2
[ -r "$report" ] || {
    echo "ERROR: Dependency report is unreadable: $report" >&2
    exit 2
}

case "$cmp_version" in
    7.12.0)
        core_coordinate=com.sourcepoint:core:0.1.4
        ktor_version_pattern='3\.0\.'
        ;;
    7.15.13)
        core_coordinate=com.sourcepoint:core:0.1.16
        ktor_version_pattern='3\.'
        ;;
    *)
        echo "ERROR: Unsupported CMP version: $cmp_version" >&2
        exit 2
        ;;
esac

cmp_coordinate="com.sourcepoint.cmplibrary:cmplibrary:$cmp_version"
require_coordinate "$cmp_coordinate"
require_coordinate "$core_coordinate"

legacy_local=$(grep -Ei 'Assets/Plugins/Android/[^[:space:]]*(cmplibrary|sourcepoint|kotlin|ktor)[^[:space:]]*\.(aar|jar)' "$report" || true)
[ -z "$legacy_local" ] || fail_forbidden "$legacy_local"

ktor_two=$(grep -E 'io\.ktor:[^:[:space:]]+:2\.|io\.ktor:[^:[:space:]]+:[0-9]+\.[^[:space:]]*[[:space:]]+->[[:space:]]*2\.' "$report" || true)
[ -z "$ktor_two" ] || fail_forbidden "$ktor_two"

ktor_lines=$(grep -E 'io\.ktor:[^:[:space:]]+:[0-9]+\.' "$report" || true)
[ -n "$ktor_lines" ] || fail_missing "io.ktor:*:3.x"

if printf '%s\n' "$ktor_lines" | grep -Ev "io\.ktor:[^:[:space:]]+:$ktor_version_pattern" >/dev/null; then
    fail_forbidden "Ktor versions outside required $ktor_version_pattern: $(printf '%s\n' "$ktor_lines" | grep -Ev "io\.ktor:[^:[:space:]]+:$ktor_version_pattern")"
fi

if [ "$cmp_version" = 7.12.0 ] && printf '%s\n' "$ktor_lines" | grep -E '[[:space:]]+->[[:space:]]*[0-9]+\.' | grep -Ev '[[:space:]]+->[[:space:]]*3\.0\.' >/dev/null; then
    fail_forbidden "Ktor versions outside required 3.0.x: $(printf '%s\n' "$ktor_lines" | grep -E '[[:space:]]+->[[:space:]]*[0-9]+\.' | grep -Ev '[[:space:]]+->[[:space:]]*3\.0\.')"
fi

echo "CMP Android dependency graph accepted for $cmp_version:"
print_coordinate "$cmp_coordinate"
print_coordinate "$core_coordinate"
printf '%s\n' "$ktor_lines"
