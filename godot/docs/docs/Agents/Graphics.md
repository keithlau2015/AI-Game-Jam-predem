# Agent: Graphics (Visuals, Sprites, Materials, Gizmos, VFX)

> **STATUS BLOCK — update after every task (see `AGENTS.md`).**

```text
## Status
- Last Updated: 2026-08-24
- Updated By: subagent:Graphics
- Phases Done: P0 shells
- Current: visual pass for P1–P3, P4–7, P11–13, P15–16, P18
- Blocked: —
## Changelog
- 2026-08-24 (bootstrap): doc created; owns all visuals + gizmo colors.
- 2026-08-24 (subagent:Graphics): added `Assets/Scripts/Graphics/Palette.cs` — single
  color-coding source of truth (SpeedUp=green, SlowDown=blue, Turn=yellow, Goal=gold,
  Invalid=red, Portal=cyan, Escort=white, Turret=dark red, Projectile=orange). Art plan for
  each entity documented below. Prefab-shell creation will happen in Unity during integration
  (shared editor not used by this subagent to avoid collisions).
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

## Art Plan (per entity)

Colors below are defined once in `Assets/Scripts/Graphics/Palette.cs` (`public static class
Palette`) and reused by every agent — do not hardcode hex elsewhere.

| Entity | Color (Palette) | Visual intent |
|--------|-----------------|---------------|
| Escort | `Escort` (white) | Clearly readable moving token; subtle outline so it stands out on any floor. |
| Obstacle | solid dark gray + `Invalid` tint on contact flash | Opaque blocker; lethal boundary shares the same family. |
| Goal | `Goal` (gold) | Glowing gold pad / ring that reads as "destination". |
| Portal (entrance/exit) | `Portal` (cyan) | Cyan ring/pad; paired endpoints share a link tint; orientation via `DirectionUtility.GetRotationZ`. |
| Placement preview (valid) | `Portal` / green-ish | Ghost of portal at cursor. |
| Placement preview (invalid) | `Invalid` (red) | Red ghost when placement is illegal (after P4–P7). |
| SpeedUp floor | `SpeedUp` (green) | Green tile + forward chevrons suggesting acceleration. |
| SlowDown floor | `SlowDown` (blue) | Blue tile + braking chevrons suggesting deceleration. |
| Turn floor | `Turn` (yellow) | Yellow tile + curved arrow indicating the redirect direction. |
| Turret | `Turret` (dark red) | Dark-red body with a muzzle indicating `fireDirection` (arrow rotated via `DirectionUtility`). |
| Projectile | `Projectile` (orange) | Orange bolt traveling along `direction`. |
| Debug gizmos (P18) | range = `Portal` ring, arrows = `Escort`/`Turn`, turret ray = `Turret`, invalid = `Invalid` | Coordinate colors with UI/UX; Graphics owns the swatches, UI/UX owns the draw calls. |

All sprites are placeholder-colored primitives initially (see Parallelization notes) and can
be upgraded to art-pack sprites (`GUI PRO Kit`, `Jettelly`, `Vefects`) later without changing
the palette.

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
