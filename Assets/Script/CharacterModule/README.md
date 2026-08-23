# CharacterModule

**Status:** Partial / Unfinished

## Purpose
Thin character definition model (icon / name / description localization IDs). No runtime controller yet.

## Entry points
| Type | Role |
|------|------|
| `CharacterModel` | Static definition only |

## How to use
1. Load CSV/encrypted models into `CharacterModel.map`.
2. Resolve by key for UI (`iconID`, `nameID`, `descriptionID`).
3. Add your own controller/party/runtime if characters are playable entities.
4. Otherwise treat as localization/metadata only.

## Dependencies
Utilities `Model` only.

## Gaps
- No controller, inventory link, or spawn logic.
