# AbilityModule

**Status:** Partial

## Purpose
Skill runtime (cooldown, targeting, range preview, projectile placement) plus data models for skills/buffs/equipment links.

## Entry points
| Type | Role |
|------|------|
| `Skill` | Primary runtime (`Execute`, `SelectTarget`, CD, preview) — takes `ICombatUnit` |
| `AbilityController` | Create/register skills for an owner |
| `Buff` / `StackableBuff` / `BuffController` | Timed attribute modifiers |
| `BuffModel` / `SkillBuffModel` | Buff definitions + skill links |
| `SkillModel` | Definitions |

## How to use
1. Load `SkillModel` / `BuffModel` / `SkillBuffModel` via FileManager/CSV.
2. Implement `ICombatUnit` on your unit (RTS `CombatUnitAgent` already does).
3. `var ac = new AbilityController(unit); ac.CreateSkill(skillKey);`
4. Placement skills spawn projectiles; buff skills apply `BuffController.ApplyBuff` to targets (or self).
5. BuffModel fields: `duration`, `attributeType`, `attributeDelta`, `maxStack`.

## Dependencies
`ICombatUnit` (AttributeModule), GameStateController (pause), EquipmentModule, ProjectileModule, ObjectPooling.
