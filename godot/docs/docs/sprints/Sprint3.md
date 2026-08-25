# Sprint 3 — Presentation

**Goal:** The player can *read* the game — HUD, state/cooldown display, debug gizmos/arrows,
portal & floor art, and audio cues.

**Agents**
- UI/UX — P17 (`PrototypeUI`: placement state, hints, cooldown timer, Clear/Fail, Retry,
  Rescued/Total), P18 (debug viz: range gizmo, dir arrows, spawn dir, turret ray,
  invalid-placement preview).
- Graphics — visuals for portals/floors/escort/obstacle/goal/turret/projectile + gizmo colors.
- Music — cues for spawn/teleport/rescue/death/fail/cooldown-end/turret-fire/invalid/clear.

**Phases**
- **P17** — All HUD elements update live; Retry reloads scene.
- **P18** — Range gizmo matches 6u; arrows match actual direction; invalid preview on illegal placement.

**Integration Gate:** Player understands state and can see range/arrows; distinct floor colors;
audio cues fire on the right events.

**Doc-update (mandatory):** UI/UX subscribes to `GameManager` events only (never calls gameplay).
Update `docs/Progress.md` + `docs/Agents/UI_UX.md`, `Graphics.md`, `Music.md` `## Status`. See `AGENTS.md`.
