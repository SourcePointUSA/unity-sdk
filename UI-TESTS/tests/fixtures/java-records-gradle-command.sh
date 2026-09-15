#!/bin/sh

printf 'cwd=%s\n' "$(pwd)"
for arg in "$@"; do
    printf 'arg=%s\n' "$arg"
done
