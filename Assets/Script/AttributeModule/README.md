# AttributeModule

**Status:** Ready

## Purpose
BigInteger attributes with min/max, edit modes, and change events. Includes a catalog model and simple HUD bar views.

## Entry points
| Type | Role |
|------|------|
| `AttributeData` | Value + `SetValue(EditMode)`, events |
| `IAttributeHolder` | `Dictionary<int, AttributeData> attributes` |
| `ICombatUnit` | Generic combat unit (team, anchors, equipment) for Ability/Projectile |
| `AttributeModel` / `AttributeType` | Catalog + enum |
| `OneAttribute` / `OneAttributeBar` | UI display |

## How to use
1. Load `AttributeModel` catalog (or hardcode needed types).
2. On entities: implement `IAttributeHolder` and fill `attributes`.
3. Mutate with `AttributeData.SetValue(value, EditMode.Add|Multiply|Replace)`.
4. Subscribe to `onValuePostChange` / min / max for death, UI, EventModule.
5. Bind HUD with `OneAttributeBar.Init(...)`.

## Dependencies
Utilities (`Model`), optional Localization / AssetsBundle for views.

## Notes
`AttributeType` still includes battleship-flavored entries (SHIELD, INSPECT_RANGE, …). Trim for non-combat games.
