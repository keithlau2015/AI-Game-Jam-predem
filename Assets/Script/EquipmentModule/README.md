# EquipmentModule

**Status:** Partial

## Purpose
Equipment catalog + instance data, scene components that hold skills and projectile anchors, optional turret aim helper.

## Entry points
| Type | Role |
|------|------|
| `Equipment` | MonoBehaviour: skills + projectile anchors |
| `EquipmentData` | Saveable instance (`Equip`, model lookup) |
| `EquipmentModel` | Definition / stats |
| `AutoRotateToLockedTarget` | Aim assist for lock-on skills |

## How to use
1. Load `EquipmentModel` (+ linked `ItemModel` if used).
2. Create `EquipmentData(id, ownerUID)` for inventory.
3. Put `Equipment` on the prefab; assign projectile anchors.
4. After building `Skill` list: `equipment.SetUp(skills)`.
5. Add `AutoRotateToLockedTarget` when skills need aim.
6. Finish real loadout rules — `Equip()` currently only validates ownership.

## Dependencies
AbilityModule, ItemModule, SaveLoadModule. Heavily used by RTS `CombatUnitAgent`.

## Gaps
- `Equip()` incomplete (no slot assignment).
- Stat block assumes combat attribute layout.
