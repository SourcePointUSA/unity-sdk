#!/bin/sh
set -eu

fail() { echo "ERROR: $*" >&2; exit 1; }
[ "$#" -eq 3 ] || fail 'Usage: assert-ios-cmp-export.sh <export-root> <cmp-version> <mobile-core-version>'
export_root=$1
cmp_version=$2
core_version=$3
lockfile="$export_root/Podfile.lock"
project="$export_root/Unity-iPhone.xcodeproj/project.pbxproj"
[ -s "$lockfile" ] || fail "Missing Podfile.lock: $lockfile"
[ -s "$project" ] || fail "Missing Unity-iPhone.xcodeproj/project.pbxproj: $project"

# Match installed pods only inside PODS, and the exact dependency only inside
# the selected CMP entry. DEPENDENCIES or checksum text cannot satisfy a gate.
awk -v cmp="$cmp_version" -v core="$core_version" '
    /^PODS:$/ { pods = 1; next }
    /^[^[:space:]]/ { pods = 0 }
    pods && /^  - / {
        inCmp = ($0 == "  - ConsentViewController (" cmp "):")
        if (inCmp) foundCmp = 1
        if ($0 == "  - SPMobileCore (" core ")") foundCore = 1
    }
    inCmp && $0 == "    - SPMobileCore (= " core ")" { foundDependency = 1 }
    END {
        if (!foundCmp) { print "ERROR: Missing resolved ConsentViewController (" cmp ")"; exit 1 }
        if (!foundCore) { print "ERROR: Missing installed SPMobileCore (" core ")"; exit 1 }
        if (!foundDependency) { print "ERROR: Missing ConsentViewController dependency SPMobileCore (= " core ")"; exit 1 }
    }
' "$lockfile" >&2 || exit 1

# Follow native target -> source phase -> build file -> file reference. A mere
# file reference or a source in Unity-iPhone is not UnityFramework ownership.
# OpenStep PBX objects use hexadecimal GUIDs; comments are not identifiers.
awk '
    function uncomment(value) {
        gsub(/\/\*[^*]*\*\//, "", value)
        return value
    }
    function property(value, key, part) {
        value = uncomment(value)
        if (!match(value, "(^|[;{[:space:]])" key "[[:space:]]*=[[:space:]]*[^;]*;")) return ""
        part = substr(value, RSTART, RLENGTH)
        sub(/^[^=]*=[[:space:]]*/, "", part)
        sub(/;$/, "", part)
        gsub(/^[[:space:]"]+|[[:space:]"]+$/, "", part)
        return part
    }
    function list(value, key, part) {
        value = uncomment(value)
        if (!match(value, key "[[:space:]]*=[[:space:]]*\\([^)]*\\)")) return ""
        part = substr(value, RSTART, RLENGTH)
        sub(/^[^(]*\(/, "", part)
        sub(/\)$/, "", part)
        return part
    }
    function fullpath(id, depth, prefix) {
        if (depth > 100) return "INVALID_GROUP_CYCLE"
        if (tree[id] != "<group>" || parent[id] == "") return path[id]
        prefix = fullpath(parent[id], depth + 1)
        return (prefix == "" ? path[id] : prefix "/" path[id])
    }
    {
        clean = uncomment($0)
        if (id == "" && clean ~ /^[[:space:]]*[[:xdigit:]]+[[:space:]]*=[[:space:]]*\{/) {
            id = clean; sub(/^[[:space:]]*/, "", id); sub(/[[:space:]=].*$/, "", id)
            object = ""; depth = 0
        }
        if (id != "") {
            object = object $0 "\n"
            depth += gsub(/\{/, "{", clean) - gsub(/\}/, "}", clean)
            if (depth == 0) { objects[id] = object; id = "" }
        }
    }
    END {
        for (guid in objects) {
            kind[guid] = property(objects[guid], "isa")
            path[guid] = property(objects[guid], "path")
            tree[guid] = property(objects[guid], "sourceTree")
            if (kind[guid] == "PBXGroup") {
                count = split(list(objects[guid], "children"), children, /[^[:xdigit:]]+/)
                for (i = 1; i <= count; i++) if (children[i] != "") parent[children[i]] = guid
            }
        }
        for (guid in objects) {
            if (kind[guid] == "PBXFileReference" && path[guid] ~ /(^|\/)SwiftBridge\.swift$/) {
                resolved = fullpath(guid, 0)
                if (resolved ~ /(^|\/)Pods\/ConsentViewController(\/|$)/) {
                    print "ERROR: Forbidden SwiftBridge.swift path under Pods/ConsentViewController: " resolved; exit 1
                }
                bridge[guid] = (resolved ~ /^Libraries\//)
            }
            if (kind[guid] == "PBXBuildFile" && objects[guid] ~ /Pods\/ConsentViewController/ && objects[guid] ~ /SwiftBridge\.swift/) {
                print "ERROR: Forbidden SwiftBridge.swift build file under Pods/ConsentViewController"; exit 1
            }
        }
        for (guid in objects) {
            if (kind[guid] != "PBXNativeTarget" || property(objects[guid], "name") != "UnityFramework") continue
            phaseCount = split(list(objects[guid], "buildPhases"), phases, /[^[:xdigit:]]+/)
            for (p = 1; p <= phaseCount; p++) {
                if (kind[phases[p]] != "PBXSourcesBuildPhase") continue
                fileCount = split(list(objects[phases[p]], "files"), files, /[^[:xdigit:]]+/)
                for (f = 1; f <= fileCount; f++) {
                    if (kind[files[f]] != "PBXBuildFile") continue
                    reference = property(objects[files[f]], "fileRef")
                    if (bridge[reference]) {
                        found = 1
                        print "UnityFramework Sources -> " files[f] " -> " fullpath(reference, 0)
                    }
                }
            }
        }
        if (!found) { print "ERROR: Missing Unity-owned SwiftBridge.swift in UnityFramework Sources"; exit 1 }
    }
' "$project" || exit 1

grep -F -e "ConsentViewController ($cmp_version)" -e "SPMobileCore (= $core_version)" -e "SPMobileCore ($core_version)" "$lockfile"
grep -F -e SwiftBridge.swift -e UnityFramework "$project"
echo 'PASS: iOS CMP export versions and UnityFramework bridge ownership'
