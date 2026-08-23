# Agent: Transition (Flow, State, Portal Placement Orchestration)

> **STATUS BLOCK — update after every task (see `AGENTS.md`).**

```text
## Status
- Last Updated: 2026-08-24
- Updated By: bootstrap
- Phases Done: —
- Current: P0 baseline scene + GameManager (P3)
- Blocked: —
## Changelog
- 2026-08-24 (bootstrap): doc created; owns GameManager + portal placement state machine.
```

**Domain:** Game-state flow, portal placement state machine, cooldown lifecycle, scene retry.
**Owns (write access):** `Assets/Scripts/Core/GameManager.cs`,
`Assets/Scripts/Portal/PortalPlacementController.cs`.

## Responsibility

The "conductor". Owns macro state (Playing/Clear/Fail), the portal placement state machine,
cooldown timing, and scene restart. Drives `PortalPairController.TryTeleport` only when the
pair is complete & active.

## Phases owned

- **P0**: Project baseline scene bootstrap (with GameMechanism for layers/tags), 2D camera,
  ground, dungeon test layout. (Scene assembly may use Unity MCP; code-side setup here.)
- **P3**: `GameManager` — counters (total/spawned/alive/rescued/dead), `GameState`,
  Clear/Fail logic, `RestartLevel()` (scene reload). Events: `OnEscortRescued/Died/Clear/Fail`.
- **P4**: `PortalPlacementController` — mouse→world position, entrance preview, validation
  (Ground only; invalid on Obstacle/Goal/Escort/Turret/Existing Portal). Left-click confirm.
- **P5**: Entrance direction selection (4-dir from mouse delta), arrow, confirm.
- **P6**: Exit placement — show max range (6u), distance validation (`<= maxPortalDistance`),
  shared validation, invalid preview, confirm.
- **P7**: Exit direction selection (4-dir), arrow, confirm → pair `Active`.
- **P10**: Reconfiguration cooldown (3s). `ActiveLocked` during CD (teleport still works,
  player cannot edit). After CD → `ActiveReconfigurable`; player may rebuild pair (old removed).
- **P20**: Final flow check (Start→Play→Clear/Fail→Retry stable).

## Key contracts (see Contracts.md)

- `GameManager` is the single source of game state. All agents read it; only Transition
  mutates `gameState` and counters. Gameplay calls `OnEscortRescued/Died`.
- `PortalPlacementState` enum (Idle, SelectingEntrancePosition, SelectingEntranceDirection,
  SelectingExitPosition, SelectingExitDirection, ActiveLocked, ActiveReconfigurable) is
  published for UI/UX.
- `PortalPairController` (GameMechanism) exposes `IsComplete`/`isActive`; Transition toggles
  them and calls `TryTeleport`.
- Cooldown value is Inspector-driven (default 3). Max distance Inspector-driven (default 6).

## Deliverables

- `Core/GameManager.cs`
- `Portal/PortalPlacementController.cs`
- Scene bootstrap script / setup for P0 (camera, ground, layers wiring).

## Dependencies on other agents

- **GameMechanism**: consumes `PortalPairController`, `PortalEndpoint`, `DirectionUtility`,
  `EscortTarget`. Does not reimplement teleport.
- **Gameplay**: Goal/Boundary/Obstacle trigger death → GameManager; level content loaded.
- **UI/UX**: displays published state + cooldown.
- **Graphics**: placement preview visuals, arrow sprites (Transition positions them).

## Parallelization notes

P0 scene + GameManager (P3) should land early so other agents have a running scene and events
to hook. Portal placement (P4–P7, P10) can proceed in parallel with GameMechanism's teleport
implementation as long as both agree on `PortalPairController`'s `IsComplete`/`isActive`
contract. This is the **main integration seam** — keep `Contracts.md` in sync.

## Acceptance (key)

- P4: legal ground placeable; wall/goal/escort/turret/existing-portal invalid.
- P5/P7: only 4 dirs; arrow matches.
- P6: 5.9u ok, 6.1u invalid; cross-wall allowed; exit not on wall.
- P10: CD = 3s, teleport works during CD, no edit during CD, editable after.
- P20: no infinite trigger, no exit-stuck, stable retry.
