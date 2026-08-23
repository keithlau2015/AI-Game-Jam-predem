# Sprint 1 — World & Hazards

**Goal:** A populated world — escorts spawn in waves, die on hazards, reach the goal, and react
to floor effects. Mechanics foundation for the portal loop.

**Agents**
- GameMechanism — P11 (SpeedUp), P12 (Slow), P13 (Turn), P14 (Portal+Floor order).
- Gameplay — P2 (Spawner), P3 (Obstacle/Boundary/Goal), P15 (Turret), P16 (Projectile).
- Graphics — prefab *shells* (escort/obstacle/goal/turret/projectile/floor tiles).
- Music — stub event hooks (no clips yet).

**Phases**
- **P2** — `EscortSpawner`: pos/count/interval/speed/startDir/prefab. Acceptance: 3 @ ~1s, correct dir.
- **P3** — Obstacle/Boundary lethal; Goal trigger → `GameManager.OnEscortRescued`.
- **P11/P12** — `FloorEffect` SpeedUp (×1.5) / SlowDown (×0.5); restore on exit.
- **P13** — Turn Floor sets direction immediately.
- **P14** — Teleport→ExitDirection→FloorEffect order (Turn can override Exit dir).
- **P15** — `Turret`: fixed, fireDirection, fireInterval (2s), wait 1 interval, body lethal.
- **P16** — `Projectile`: linear, kills escort, destroyed by obstacle/boundary, ignores portals.

**Integration Gate:** Escorts spawn, die on hazards, reach goal → Rescue; floors change
speed/turn; turret fires and projectile kills. No NRE.

**Doc-update (mandatory):** Update `docs/Progress.md` (matrix + phase table + Changelog);
update your `docs/Agents/<role>.md` `## Status`. See `AGENTS.md`.
