# Agent: Gameplay (Level Content & Combat)

**Domain:** Spawning, hazards, level geometry, turret/projectile combat.
**Owns (write access):** `Assets/Scripts/Escort/EscortSpawner.cs`,
`Assets/Scripts/Level/Goal.cs`, `Assets/Scripts/Level/Obstacle.cs`,
`Assets/Scripts/Level/Boundary.cs`, `Assets/Scripts/Combat/Turret.cs`,
`Assets/Scripts/Combat/Projectile.cs`, level prefabs/scenes content.

## Responsibility

Build everything the escort interacts with in the world: spawner, obstacles, goal, boundary,
turrets, projectiles. Wire lethal collisions to `EscortTarget.Die()` (method owned by
GameMechanism).

## Phases owned

- **P2**: `EscortSpawner.cs` — position, count, interval, speed, startDirection, prefab.
  Spawns N escorts spaced by interval (P2 acceptance: 3 @ 1s intervals).
- **P3**: `Obstacle` (lethal on contact), `Boundary` (lethal on contact), `Goal`
  (trigger → `GameManager.OnEscortRescued`).
- **P15**: `Turret.cs` — fixed position, fireDirection, fireInterval (default 2s), waits one
  interval before first shot, spawns `Projectile` along fireDirection. Turret body lethal.
- **P16**: `Projectile.cs` — linear move at speed, size; hits Escort → `Die()` + destroy;
  hits Obstacle/Boundary → destroy. **Ignores portals** (layer-excluded).
- **P19**: Assemble the First Playable Level (1 spawn, 3 escorts, obstacle, goal, slow floor,
  turn floor, turret, portal rules 6 / 3s).

## Key contracts (see Contracts.md)

- `EscortTarget.Die()` is the ONLY death entry point (owned by GameMechanism).
- `GameManager.OnEscortRescued(t)` on Goal entry; `OnEscortDied(t)` is called by EscortTarget.
- Projectile layer must NOT be in Portal's accepted layers.
- All tunables (count, interval, speed, fire interval, projectile speed/size) → Inspector.

## Deliverables

- `Escort/EscortSpawner.cs` + escort prefab (visual from Graphics agent).
- `Level/Goal.cs`, `Level/Obstacle.cs`, `Level/Boundary.cs`.
- `Combat/Turret.cs`, `Combat/Projectile.cs` + projectile prefab.
- `P19` test level scene (`Scenes/PortalEscort_Level01`) using prefabs from Graphics.

## Dependencies on other agents

- **GameMechanism**: uses `EscortTarget` prefab + `Die()`; consumes `FloorEffect` data (does
  not implement floor logic). Uses `Direction` enum for turret fire & projectile direction.
- **Graphics**: provides sprites/prefabs for escort, obstacle, goal, turret, projectile, floor.
  Gameplay references them by prefab field; must not block on final art.
- **Transition**: Goal/Boundary feed `GameManager`; level load/retry handled by Transition.
- **UI/UX**: HUD reads rescued/total from GameManager.

## Parallelization notes

Can run fully in parallel with GameMechanism (different folders) and with UI/UX. The P19 level
assembly is the **integration milestone** — best done after Mechanism + Transition + Graphics
have v1 prefabs. Uses placeholder cubes/sprites until Graphics delivers.

## Acceptance (key)

- P2: 3 escorts spawn ~1s apart with correct start direction.
- P3: wall/boundary → Fail; goal → Rescue.
- P15: ~1 shot / 2s, correct direction, body lethal.
- P16: projectile kills escort, destroyed by obstacle/boundary, passes through portals.
- P19: level forces use of teleport + redirect + one floor + dodge one turret.
