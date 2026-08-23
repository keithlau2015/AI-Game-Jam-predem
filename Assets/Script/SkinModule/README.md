# SkinModule

**Status:** Partial

## Purpose
Apply skinned-mesh skins (mesh + material variants) from definition or saved instance.

## Entry points
| Type | Role |
|------|------|
| `SkinController` | `SetSkinByIns`, `SetSkinByID` |
| `SkinModel` | Mesh / materials / VFX / SFX ids |
| `SkinData` | Owned instance + material index |

## How to use
1. Author `SkinModel` rows with Addressable mesh/material keys.
2. Add `SkinController` on skinned characters.
3. Call `SetSkinByID(skinId)` or `SetSkinByIns(uid)` with `SkinData`.
4. Ensure Addressables exist for referenced assets.
5. Persist skins via SaveLoad if needed.
6. Fix early-return paths so loading UI always hides (`HideMini` / `isIniting`).

## Dependencies
LoadingManager, UIManager, AssetsBundleManager, SaveLoadModule.

## Gaps
- Early returns can leave loading UI stuck.
- VFX/SFX fields on model unused by controller.
- Inherits SaveLoad GUID issues via `SkinData`.
