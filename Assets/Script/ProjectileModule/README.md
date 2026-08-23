# ProjectileModule

**Status:** Partial (uses `ICombatUnit`)

## Purpose
Pooled projectiles: setup from emitter, tracking/move, attribute damage, explosions.

## Entry points
| Type | Role |
|------|------|
| `Projectile` | `SetUp(ICombatUnit, ProjectileModel, formulaId)` |
| `Explosion` / `MoveForward` / `SelfDestruction` | Behaviour helpers |
| `ProjectileModel` | Definition |

## How to use
1. Define `ProjectileModel` + entity pool entries.
2. Spawn from pool; `SetUp(combatUnit, model, formulaId)`.
3. `AddTarget` for trackers; attach MoveForward / SelfDestruction as needed.
4. Targets need `ICombatUnit` + Hitable tag for impact.

## Dependencies
AttributeModule (`ICombatUnit`), FormulaModule, ObjectPoolingModule, GameLog.
