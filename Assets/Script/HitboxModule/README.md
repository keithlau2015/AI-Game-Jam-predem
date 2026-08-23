# HitboxModule

**Status:** Partial

## Purpose
Destructible contract for damage/repair, plus a nested hitbox collector.

## Entry points
| Type | Role |
|------|------|
| `IDestructible` | `OnHit`, `OnDestruct`, `OnRepair` |
| `HitboxController` | Collects child hitboxes; `FindChild(id)` |

## How to use
1. Implement `IDestructible` on damageable entities.
2. From projectiles/abilities, call `OnHit(dmg)` then `OnDestruct` on death.
3. Add `HitboxController` on roots; `Init()` gathers children (auto on enable).
4. RTS `CombatUnitAgent` already implements `IDestructible`.

## Dependencies
None internal. Consumers: Projectile / RTS.
