# Sprint 0 — Baseline

**Goal:** A running 2D top-down scene where an escort auto-moves in 4 directions. Establishes
the shared `Direction` contract that every later agent depends on.

**Agents:** Transition (scene/camera/ground/layers/tags) + GameMechanism (`Direction`, `DirectionUtility`, `EscortTarget`).

**Phases**
- **P0** — Project baseline: scene, 2D camera, ground, dungeon test layout, layers/tags.
  Acceptance: scene runs; camera frames test area; `Direction` converts to Up/Right/Down/Left vectors.
- **P1** — `EscortTarget.cs`: continuous 4-dir movement, Inspector speed (default 2 u/s),
  start direction, no NavMesh, no player control.
  Acceptance: escort moves on spawn; all 4 dirs correct; speed change immediate.

**Integration Gate:** Scene plays; escort walks; `DirectionUtility` usable by all agents.
No compile errors (Unity MCP `read_console`).

**Doc-update (mandatory):** Update `docs/Progress.md` Domain Matrix + Changelog; update
`docs/Agents/GameMechanism.md` & `docs/Agents/Transition.md` `## Status`. See `AGENTS.md`.
