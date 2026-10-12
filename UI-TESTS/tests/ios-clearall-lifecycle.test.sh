#!/bin/sh
set -eu

test_dir=$(CDPATH= cd -- "$(dirname -- "$0")" && pwd)
bridge="$test_dir/../../Assets/ConsentManagementProvider/Plugins/iOS/Source/SwiftBridge.swift"

require() {
    pattern=$1
    message=$2
    if ! grep -F "$pattern" "$bridge" >/dev/null; then
        echo "FAIL: $message" >&2
        exit 1
    fi
}

require 'private var consentManagerConfiguration' \
    'ClearAll must retain the configuration needed to replace the live iOS manager.'
require 'private func makeConsentManager' \
    'Initial setup and ClearAll recovery must share manager construction.'

clear_block=$(sed -n '/@objc public func onClearConsentTap()/,/@objc public func loadPrivacyManager/p' "$bridge")
printf '%s\n' "$clear_block" | grep -F 'SPConsentManager.clearAllData()' >/dev/null
printf '%s\n' "$clear_block" | grep -F 'consentManager = makeConsentManager(configuration)' >/dev/null || {
    echo 'FAIL: ClearAll must replace the manager so LoadMessage cannot reuse stale in-memory consent state.' >&2
    exit 1
}

printf '%s\n' 'PASS: iOS ClearAll rebuilds the live consent manager from its previous configuration'
