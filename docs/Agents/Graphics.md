# Agent: Graphics (Visuals, Sprites, Materials, Gizmos, VFX)

> **STATUS BLOCK — update after every task (see `AGENTS.md`).**

```text
## Status
- Last Updated: 2026-08-24
- Updated By: bootstrap
- Phases Done: —
- Current: P0 prefab shells
- Blocked: —
## Changelog
- 2026-08-24 (bootstrap): doc created; owns all visuals + gizmo colors.
```

**Domain:** Everything the player sees — sprites, colors, materials, debug gizmo visuals,
scene look, lightweight VFX. No gameplay logic.
**Owns (write access):** `Assets/Art/`, prefab visuals, materials, `Assets/Scripts/Debug/`
visual helpers (shared with UI/UX for gizmos), scene lighting/skybox setup.

## Responsibility

Make the prototype readable and pleasant using the project's existing art packs
(e.g. `GUI PRO Kit`, `Jettelly`, `Vefects`, skyboxes already in `Assets/`). Provide prefabs
and sprites that Gameplay/Transition/UI reference.

## Phases owned (visual side)

- **P0**: 2D top-down scene look — ground tile, camera framing, lighting. Dungeon test layout
  visuals.
- **P1/P2**: Escort sprite/prefab (clearly readable "moving thing"), spawn visuals.
- **P3**: Obstacle, Boundary, Goal visuals (Goal must read as a destination).
- **P4–P7**: Entrance/Exit portal visuals + placement preview (valid/invalid tint), direction
  arrow sprites (use `DirectionUtility.GetRotationZ` for orientation).
- **P11–P13**: Floor-effect visuals — SpeedUp / SlowDown / Turn floor tiles with distinct
  color coding.
- **P15/P16**: Turret sprite + fire-direction indicator, Projectile sprite.
- **P18**: Gizmo visuals (range circle, arrows, spawn dir, turret ray) — coordinate with UI/UX
  on who draws what; Graphics supplies materials/colors, UI/UX supplies the draw calls if code.

## Key contracts (see Contracts.md)

- Prefabs expose the expected component types (e.g. `EscortTarget`, `Turret`) so Gameplay can
  attach scripts. Graphics builds the visual shell; scripts are added by other agents.
- Direction arrows MUST be oriented via `DirectionUtility.GetRotationZ` (owned GameMechanism)
  so visual matches logic.
- Color coding convention (propose in this doc, keep consistent):
  - SpeedUp = green, SlowDown = blue, Turn = yellow, Goal = gold, Invalid = red, Portal = cyan.

## Deliverables

- Sprite/prefab assets under `Assets/Art/` and `Assets/Prefabs/`.
- Materials / palettes for floor effects and portals.
- Preview/arrow sprites for portal placement.
- Scene visual setup (lighting, camera bg) for P0 and P19.

## Dependencies on other agents

- **Gameplay**: needs escort/obstacle/goal/turret/projectile prefabs (visual shell).
- **Transition**: needs portal + arrow + preview visuals.
- **UI/UX**: needs HUD sprites/fonts, banner visuals, debug gizmo colors.
- **Music**: optional joint juice (hit VFX + SFX) — coordinated, not blocking.

## Parallelization notes

Most independent. Can produce placeholder-colored primitives immediately and upgrade to real
sprites later. Should deliver prefab *shells* early (empty GameObjects with sprites + colliders
sized correctly) so Gameplay/Transition can attach scripts without waiting for final art.

## Acceptance (key)

- Readable 4-direction movement; distinct floor colors; valid/invalid placement obvious;
  arrows point the actual direction; goal clearly signals destination.
