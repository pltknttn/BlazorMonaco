# Updating the bundled Monaco Editor

`monaco-editor` is vendored through [LibMan](https://learn.microsoft.com/aspnet/core/client-side/libman/).
`BlazorMonaco/libman.json` is the source of truth, and the actual files live in
`BlazorMonaco/wwwroot/lib/monaco-editor/`.

`scripts/update-monaco.sh` updates both in one command.

## Requirements

- [ ] `libman` CLI:

  ```bash
  dotnet tool install -g Microsoft.Web.LibraryManager.Cli
  ```

- [ ] `curl` (only used to look up the latest version on npm)
- [ ] Bash 3.2+ (Linux, macOS, WSL or Git Bash)

## Usage

```bash
# Update to the latest stable release on npm
./scripts/update-monaco.sh

# Update to a specific version
./scripts/update-monaco.sh 0.59.0

# Preview without changing anything
./scripts/update-monaco.sh --dry-run

# CI: exit with code 1 if libman.json is outdated
./scripts/update-monaco.sh --check

# Also bump version strings in csproj/README/MonacoLanguages.cs
./scripts/update-monaco.sh --with-docs
```

| Option | Description |
| --- | --- |
| `VERSION` | Target version. Defaults to the npm `latest` dist-tag. |
| `--check` | Read-only; exits `1` when a newer version is available. |
| `--dry-run` | Prints the planned changes and exits. |
| `--with-docs` | Also replaces the old version string in `BlazorMonaco.csproj`, `README.md` and `Bridge/MonacoLanguages.cs`. |
| `-h`, `--help` | Shows help. |

## What the script does

1. Reads the current version from `BlazorMonaco/libman.json`.
2. Resolves the target version (argument or npm `latest`).
3. Downloads `monaco-editor@<target>` with `libman restore` into a temporary
   staging directory inside `BlazorMonaco/`.
4. Replaces `BlazorMonaco/wwwroot/lib/monaco-editor/` with the staged files.
   Because the whole directory is replaced, hashed files from the old release
   (for example `editor-BdtEMBbM.js`) do not linger.
5. Writes the new version back to `BlazorMonaco/libman.json`, preserving the
   existing formatting and `CRLF` line endings.
6. Verifies that `min/vs/loader.js` and `min/vs/editor/editor.main.js` exist in
   the new distribution.

The download happens before anything is replaced, so a failed download or an
invalid version leaves the working tree untouched. The previous directory is
restored automatically if the swap is interrupted.

## After updating

1. Review the diff:

   ```bash
   git status
   git diff --stat
   ```

2. Add an entry to `CHANGELOG.md` (the script does not write changelog prose).
3. If you did not use `--with-docs`, update the version in:
   - `BlazorMonaco/BlazorMonaco.csproj` (`<Description>`)
   - `README.md` (badge and "Uses `Monaco Editor vX.Y.Z`")
   - `BlazorMonaco/Bridge/MonacoLanguages.cs` (comment)
4. Build and smoke-test the samples:

   ```bash
   dotnet build BlazorMonaco.sln
   dotnet run --project SampleWasmApp
   ```

## Notes

- Pre-releases are not selected automatically. Pass the exact version, e.g.
  `./scripts/update-monaco.sh 0.59.0-rc.1`.
- `libman update monaco-editor` cannot be used as-is: it rewrites `libman.json`
  with different formatting and does not refresh the directory when the
  library is already at the target version.
- On Windows, run the script from Git Bash or WSL. Alternatively, run
  `libman update monaco-editor` from `BlazorMonaco/` manually, then reformat
  `libman.json` and update the documentation versions by hand.
