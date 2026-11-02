#!/usr/bin/env bash
#
# Update the bundled Monaco Editor distribution in one command.
#
#   ./scripts/update-monaco.sh              # update to the latest stable release
#   ./scripts/update-monaco.sh 0.59.0       # update to an explicit version
#   ./scripts/update-monaco.sh --check      # CI: fail if a newer release exists
#   ./scripts/update-monaco.sh --dry-run    # show the plan, change nothing
#   ./scripts/update-monaco.sh --with-docs  # also bump version strings in docs/csproj
#
# What it does:
#   1. Reads the current version from BlazorMonaco/libman.json.
#   2. Resolves the target version (argument or latest stable on npm).
#   3. Downloads monaco-editor@<target> "min/**/*" into a staging directory.
#   4. Replaces BlazorMonaco/wwwroot/lib/monaco-editor with the staged files
#   5. Writes the new version back to BlazorMonaco/libman.json.
#   6. Optionally updates version strings in BlazorMonaco.csproj, README.md
#      and Bridge/MonacoLanguages.cs.
#
set -euo pipefail

SCRIPT_DIR="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" >/dev/null 2>&1 && pwd)"
REPO_ROOT="$(cd -- "$SCRIPT_DIR/.." >/dev/null 2>&1 && pwd)"
PROJECT_DIR="$REPO_ROOT/BlazorMonaco"
LIBMAN_JSON="$PROJECT_DIR/libman.json"
DEST_DIR="$PROJECT_DIR/wwwroot/lib/monaco-editor"
DOC_FILES=(
    "$PROJECT_DIR/BlazorMonaco.csproj"
    "$REPO_ROOT/README.md"
    "$PROJECT_DIR/Bridge/MonacoLanguages.cs"
)

CHECK_ONLY=0
DRY_RUN=0
WITH_DOCS=0
TARGET_VERSION=""

usage() {
    cat <<'EOF'
Usage: scripts/update-monaco.sh [VERSION] [options]

Arguments:
  VERSION            Target version, e.g. 0.59.0 (default: latest stable on npm)

Options:
  --check            Do not modify anything; exit 1 if libman.json is outdated
  --dry-run          Print what would be done without changing any files
  --with-docs        Also update version strings in csproj/README/MonacoLanguages.cs
  -h, --help         Show this help

Requirements:
  - libman: dotnet tool install -g Microsoft.Web.LibraryManager.Cli
  - curl (only when VERSION is omitted)

Examples:
  ./scripts/update-monaco.sh
  ./scripts/update-monaco.sh 0.59.0
  ./scripts/update-monaco.sh --check
EOF
}

die() {
    echo "error: $*" >&2
    exit 1
}

while (($#)); do
    case "$1" in
        -h|--help)
            usage
            exit 0
            ;;
        --check)
            CHECK_ONLY=1
            ;;
        --dry-run)
            DRY_RUN=1
            ;;
        --with-docs)
            WITH_DOCS=1
            ;;
        -*)
            die "unknown option: $1 (see --help)"
            ;;
        *)
            [[ -z "$TARGET_VERSION" ]] || die "only one VERSION argument is allowed"
            TARGET_VERSION="${1#v}"
            ;;
    esac
    shift
done

[[ -f "$LIBMAN_JSON" ]] || die "libman.json not found: $LIBMAN_JSON"

current_version="$(sed -n 's/.*"library"[[:space:]]*:[[:space:]]*"monaco-editor@\([^"]*\)".*/\1/p' "$LIBMAN_JSON")"
[[ -n "$current_version" ]] || die "could not find a monaco-editor entry in $LIBMAN_JSON"
if [[ "$(printf '%s\n' "$current_version" | wc -l)" -ne 1 ]]; then
    die "more than one monaco-editor entry found in $LIBMAN_JSON"
fi

resolve_latest() {
    command -v curl >/dev/null 2>&1 || die "curl is required to look up the latest version"
    local json latest
    if ! json="$(curl -fsSL --max-time 30 "https://registry.npmjs.org/-/package/monaco-editor/dist-tags")"; then
        die "could not query the npm registry; pass the target version explicitly"
    fi
    latest="$(printf '%s' "$json" | sed -n 's/.*"latest"[[:space:]]*:[[:space:]]*"\([^"]*\)".*/\1/p')"
    [[ -n "$latest" ]] || die "could not parse the latest version from the npm registry response"
    printf '%s' "$latest"
}

if [[ -z "$TARGET_VERSION" ]]; then
    TARGET_VERSION="$(resolve_latest)"
fi

[[ "$TARGET_VERSION" =~ ^[0-9]+\.[0-9]+\.[0-9]+([-+][0-9A-Za-z.-]+)?$ ]] \
    || die "invalid version: $TARGET_VERSION"

if [[ "$TARGET_VERSION" == "$current_version" ]]; then
    echo "monaco-editor is already at $current_version; nothing to do."
    exit 0
fi

if ((CHECK_ONLY)); then
    echo "monaco-editor is outdated: libman.json has $current_version, target is $TARGET_VERSION." >&2
    exit 1
fi

echo "monaco-editor: $current_version -> $TARGET_VERSION"

if ((DRY_RUN)); then
    echo "Would download monaco-editor@$TARGET_VERSION (min/**/*) and replace:"
    echo "  $DEST_DIR"
    echo "Would set \"library\": \"monaco-editor@$TARGET_VERSION\" in:"
    echo "  $LIBMAN_JSON"
    if ((WITH_DOCS)); then
        echo "Would update version strings in:"
        printf '  %s\n' "${DOC_FILES[@]}"
    fi
    exit 0
fi

command -v libman >/dev/null 2>&1 \
    || die "libman not found; install it with: dotnet tool install -g Microsoft.Web.LibraryManager.Cli"

stage_dir="$(mktemp -d "$PROJECT_DIR/.monaco-update.XXXXXX")"
backup_dir="$PROJECT_DIR/wwwroot/lib/.monaco-editor.bak.$$"

cleanup() {
    rm -rf -- "$stage_dir"
    # Restore the previous distribution if the swap did not complete.
    if [[ -d "$backup_dir" ]]; then
        if [[ ! -d "$DEST_DIR" ]]; then
            mv -- "$backup_dir" "$DEST_DIR"
        else
            rm -rf -- "$backup_dir"
        fi
    fi
}
trap cleanup EXIT

cat > "$stage_dir/libman.json" <<EOF
{
    "version": "1.0",
    "defaultProvider": "unpkg",
    "libraries": [
        {
            "library": "monaco-editor@$TARGET_VERSION",
            "destination": "pkg/",
            "files": [
                "min/**/*"
            ]
        }
    ]
}
EOF

echo "Downloading with libman ..."
if ! (cd -- "$stage_dir" && libman restore --verbosity quiet) >"$stage_dir/libman.log" 2>&1; then
    cat "$stage_dir/libman.log" >&2
    die "libman restore failed for monaco-editor@$TARGET_VERSION"
fi

for entry in min/vs/loader.js min/vs/editor/editor.main.js; do
    [[ -f "$stage_dir/pkg/$entry" ]] || die "downloaded package is missing $entry"
done

mkdir -p -- "$(dirname -- "$DEST_DIR")"
if [[ -d "$DEST_DIR" ]]; then
    mv -- "$DEST_DIR" "$backup_dir"
fi
if ! mv -- "$stage_dir/pkg" "$DEST_DIR"; then
    [[ -d "$backup_dir" ]] && mv -- "$backup_dir" "$DEST_DIR"
    die "could not move the downloaded files to $DEST_DIR"
fi
rm -rf -- "$backup_dir"

sed -i.bak -E \
    "s/(\"library\"[[:space:]]*:[[:space:]]*\"monaco-editor@)[^\"]+\"/\1${TARGET_VERSION}\"/" \
    "$LIBMAN_JSON"
rm -f -- "$LIBMAN_JSON.bak"
grep -q "\"monaco-editor@$TARGET_VERSION\"" "$LIBMAN_JSON" \
    || die "failed to update the version in $LIBMAN_JSON"

if ((WITH_DOCS)); then
    echo "Updating version strings in documentation ..."
    current_pattern="${current_version//./\\.}"
    for doc in "${DOC_FILES[@]}"; do
        [[ -f "$doc" ]] || continue
        sed -i.bak "s/$current_pattern/$TARGET_VERSION/g" "$doc"
        rm -f -- "$doc.bak"
    done
fi

file_count="$(find "$DEST_DIR" -type f | wc -l | tr -d ' ')"
echo
echo "Done: monaco-editor@$TARGET_VERSION ($file_count files)"
echo "  - $LIBMAN_JSON"
echo "  - $DEST_DIR"
if ((!WITH_DOCS)); then
    echo
    echo "Note: BlazorMonaco.csproj, README.md and Bridge/MonacoLanguages.cs may still"
    echo "      mention $current_version. Re-run with --with-docs to update them."
fi
echo
echo "Next steps: review 'git status', add a CHANGELOG.md entry, and build/test the solution."
