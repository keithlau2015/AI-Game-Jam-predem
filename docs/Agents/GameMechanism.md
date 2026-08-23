# Agent: GameMechanism (Core Systems)

> **STATUS BLOCK — update after every task (see `AGENTS.md`).**

```text
## Status
- Last Updated: 2026-08-24
- Updated By: bootstrap
- Phases Done: —
- Current: P0 baseline (Direction + EscortTarget)
- Blocked: —
## Changelog
- 2026-08-24 (bootstrap): doc created; owns Direction/EscortTarget/teleport/floor contracts.
```

**Domain:** Core gameplay engine — the rules that make the escort + portal + floor loop work.
**Owns (write access):** `Assets/Scripts/Core/`, `Assets/Scripts/Escort/EscortTarget.cs`,
`Assets/Scripts/Portal/PortalPairController.cs`, `Assets/Scripts/Portal/PortalEndpoint.cs`,
`Assets/Scripts/Level/FloorEffect.cs`.

## Responsibility

Implement the deterministic, testable rules. This agent owns the **shared contracts**
(`Direction`, `DirectionUtility`, `EscortTarget`, portal teleport logic, floor effects).

## Phases owned

- **P0**: `Direction` enum + `DirectionUtility` (shared contract). Layers/tags baseline (with Transition).
- **P1**: `EscortTarget.cs` continuous 4-direction movement, Inspector speed/direction, default 2 u/s.
- **P8**: `PortalPairController` + `PortalEndpoint`. Direction-match teleport, instant move,
  exit safe offset (0.5–1.0), teleport lock (0.1–0.2s). See TASKS P8 cases 1–4.
- **P9**: Portal persists; multiple escorts reuse same pair.
- **P11**: `FloorEffect` SpeedUp (×multiplier), restore on exit.
- **P12**: SlowDown (×multiplier), restore on exit.
- **P13**: Turn Floor sets `currentDirection` immediately.
- **P14**: Teleport→Exit Direction→Floor Effect order (Turn Floor may override Exit Direction).

## Key contracts (see Contracts.md)

- `DirectionUtility.GetVector/GetRotationZ/FromDelta/AreSame` — single source of truth.
- `EscortTarget.TeleportTo`, `.ApplyFloorEffect`, `.Die`.
- `PortalPairController.TryTeleport` only when `isActive && direction match`.
- Floor effect order: set exit direction first, then apply floor effect.

## Deliverables

- `Core/DirectionUtility.cs`
- `Escort/EscortTarget.cs`
- `Portal/PortalEndpoint.cs`
- `Portal/PortalPairController.cs`
- `Level/FloorEffect.cs`
- Unit-style test stubs for teleport/floor logic (optional but recommended; pure C# where possible).

## Dependencies on other agents

- **Transition**: calls `TryTeleport` from `PortalPlacementController` once a pair is active;
  reads `GameManager` state. Does NOT modify teleport internals.
- **Gameplay**: spawns `EscortTarget` prefab, provides `Obstacle`/`Goal`/`Boundary` that call
  `EscortTarget.Die()`. Mechanism owns death method, Gameplay owns trigger colliders.
- **UI/UX**: subscribes to escort/portal events for debug + HUD.

## Parallelization notes

This agent can start immediately after P0 baseline. Teleport + floor logic (P8, P11–P14)
can be built before the placement UI exists, because `PortalPairController` can be driven by
tests / direct inspector setup. Coordinates with **Transition** only on the `isActive` flag.

## Acceptance (key)

- P1: escort moves 4 dirs at inspector speed; no NavMesh, no player movement control.
- P8 case 2: wrong-direction escort is NOT teleported. Case 3: exit direction applied instantly.
- P11: speed 2 ×1.5 = 3 on floor, 2 off. P12: ×0.5 = 1 on floor. P13: turn applied instantly.
- P14: exit Down + Turn-Right floor → final direction Right.
