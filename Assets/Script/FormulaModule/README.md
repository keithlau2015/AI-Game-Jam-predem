# FormulaModule

**Status:** Partial (decoupled from CombatUnitAgent)

## Purpose
Combat damage formulas. Also contains a generic string expression evaluator (`FormulaModel`) that the controller does not use yet.

## Entry points
| Type | Role |
|------|------|
| `FormulaController.GetDmg(IAttributeHolder, IAttributeHolder, formula)` | Damage API |
| `FormulaType` | Gunfire / Laser / Missile / Mine, … |
| `FormulaModel` | `${owner.field}` expression eval (**orphaned**) |

## How to use
1. Call `FormulaController.GetDmg(attacker, defender, formulaId)` with any `IAttributeHolder`s.
2. Both sides need combat attributes (HP/ATK/DEF/CRI/…).
3. Pass formula id from `SkillModel.formula`.

## Dependencies
AttributeModule.

## Remaining gaps
- Weapon cases are still hard-coded names; prefer data-driven `FormulaModel` later.
