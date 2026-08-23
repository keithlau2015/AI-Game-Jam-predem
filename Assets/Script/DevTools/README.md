# DevTools

**Status:** Ready (Editor-only)

## Purpose
Editor utilities: CSV encrypt pipeline, font batching, version bump menus, local save-data reset.

## Entry points
| Type | Role |
|------|------|
| `AssetsProcessPipline` | Auto-encrypt imported CSV |
| `EncryptFileEditor` / folder encrypt | Manual encrypt menus |
| `FontEditor` | Font batch tools |
| `VersionIncrementor` | Version bump (`NPI/DevTools/...`) |
| `GameFileManagment` | Reset local game data |

## How to use
1. Keep these scripts editor-only (`#if UNITY_EDITOR`).
2. Use **NPI/DevTools** menu items to encrypt CSV, bump version, batch fonts.
3. Drop CSVs into watched folders so the import pipeline re-encrypts.
4. Call reset tools when clearing local saves during development.
5. Retarget menu names if you are not shipping under NPI branding.

## Dependencies
FileManager.
