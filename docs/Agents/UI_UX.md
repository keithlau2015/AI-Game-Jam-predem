# Agent: UI/UX (Interface, Feedback, Debug)

**Domain:** All on-screen communication — HUD, prompts, state display, and debug visualization.
**Owns (write access):** `Assets/Scripts/UI/PrototypeUI.cs`, `Assets/Scripts/Debug/`
(gizmos, previews, arrows), any Canvas/prefab UI assets.

## Responsibility

Tell the player what is happening and let them understand the system. Subscribe to
`GameManager` events; never call gameplay internals directly.

## Phases owned

- **P17**: `PrototypeUI` — shows:
  - Portal placement state (Idle / SelectingEntrance / SelectingExit / ActiveLocked / Reconfigurable)
  - Entrance/Exit operation hints
  - Reconfiguration cooldown timer
  - Clear / Fail banners
  - Retry button (calls `GameManager.RestartLevel`)
  - Rescued / Total counter
- **P18**: Debug visualization (gizmos + runtime):
  - Portal max-range circle (radius = maxPortalDistance)
  - Entrance direction arrow, Exit direction arrow
  - Spawn direction indicator
  - Turret fire direction ray
  - Invalid-placement preview (red tint when validation fails)

## Key contracts (see Contracts.md)

- UI reads via events: `OnEscortRescued`, `OnEscortDied`, `OnGameClear`, `OnGameFail`,
  `OnPortalStateChanged`. Do NOT poll gameplay fields.
- Portal placement state is owned/published by **Transition** (`PortalPlacementController`).
  UI only displays it.
- Direction arrows use `DirectionUtility.GetRotationZ` (owned by GameMechanism).

## Deliverables

- `UI/PrototypeUI.cs` (Canvas + Text/Image elements, wired to GameManager events).
- `Debug/PortalRangeGizmo.cs`, `Debug/DirectionArrow.cs`, `Debug/PlacementPreview.cs`.
- UI prefab(s) under `Assets/UI/` or `Assets/Prefabs/UI/`.

## Dependencies on other agents

- **Transition**: publishes portal state + cooldown value for HUD.
- **GameMechanism**: direction math for arrows; floor/projectile state is read-only if shown.
- **Graphics**: visual style of HUD, arrows, banners (colors, fonts, sprites).
- **Music**: trigger SFX on UI events (clear/fail/cooldown) via Music agent's audio hooks.

## Parallelization notes

Fully parallel with Mechanism/Gameplay — UI only consumes events. Can be built against a
mock `GameManager` (stub events) until Transition lands. Recommended to start after P3
(GameManager exists) but can stub earlier.

## Acceptance (key)

- P17: all listed elements visible and update live; Retry reloads scene.
- P18: range gizmo matches 6u; arrows match actual direction; invalid preview shows on
  illegal placement.
