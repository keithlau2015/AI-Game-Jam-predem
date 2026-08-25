# Sprint 2 — Portal Loop  *(integration-critical)*

**Goal:** The full portal pair: place entrance (pos+dir), place exit (pos+dir, ≤6u), teleport
matching-direction escorts instantly, persist, and enforce the 3s reconfiguration cooldown.

**Agents (tightly coupled on `PortalPairController` contract)**
- GameMechanism — P8 (`PortalPairController`, `PortalEndpoint`: direction match, instant
  teleport, exit safe offset 0.5–1.0, teleport lock 0.1–0.2s), P9 (persistent pair).
- Transition — P4 (entrance placement + validation), P5 (entrance dir), P6 (exit placement +
  distance ≤6 + range preview), P7 (exit dir → pair Active), P10 (cooldown 3s: ActiveLocked
  during CD, editable after).

**Phases**
- **P8** — Teleport only when `isActive && escort.dir == entrance.dir`. Cases 1–4 in `TASKS.md`.
- **P9** — Pair survives use; multiple escorts reuse it.
- **P4–P7** — Placement state machine (`PortalPlacementState`), validation (Ground only;
  invalid on Obstacle/Goal/Escort/Turret/Existing Portal), 4-dir selection, 6u range.
- **P10** — Cooldown 3s; teleport works during CD; no edit during CD; editable after.

**Integration Gate:** Full pair teleports correct-direction escorts; wrong-direction ignored;
CD = 3s; no edit during CD; cross-wall placement allowed; exit not on wall.

**Doc-update (mandatory):** This is the main seam — keep `docs/Contracts.md`
(`PortalPairController`/`PortalEndpoint` `IsComplete`/`isActive`) in sync. Update
`docs/Progress.md` + both agents' `## Status`. See `AGENTS.md`.
