# Progress.md — SINGLE SOURCE OF TRUTH (live project tracker)

> **This is the one file that reflects the WHOLE project progress** across all agents
> (Mechanism, Gameplay, UI/UX, Transition, Graphics, Music) and regardless of which AI tool
> runs them. Every agent updates their row here after every task. See `AGENTS.md` for the
> forced update rules.

---

## How to use
- **Agents:** update your Status row + Changelog after each task. No exceptions (see `AGENTS.md`).
- **Humans / coordinators:** read this to see overall health at a glance.

---

## Overall Status
- **Current Sprint:** Sprint 0 (Baseline) — *set by coordinator; update after each sprint.*
- **Core Loop Stable** (Escort + Portal + Floor + Turret playable end-to-end): Not yet
- **Last full Play Test:** none
- **Top Blocker:** none
- **Branch strategy:** main = stable; feature work on side branches, merged after Play Test.

---

## Domain Matrix  *(every agent keeps its row current)*

| Domain | Owner Agent | Phases | Done | Current | Blocked By | Last Updated | Updated By |
|--------|-------------|--------|------|---------|-----------|--------------|-----------|
| GameMechanism | GameMechanism | P0, P1, P8, P9, P11–P14 | — | P0 baseline | — | 2026-08-24 | bootstrap |
| Gameplay | Gameplay | P2, P3, P15, P16, P19 | — | — | — | 2026-08-24 | bootstrap |
| Transition | Transition | P0, P3, P4–P7, P10, P20 | — | P0 baseline | — | 2026-08-24 | bootstrap |
| UI/UX | UI/UX | P17, P18 | — | — | — | 2026-08-24 | bootstrap |
| Graphics | Graphics | P0, P1–3 vis, P4–7 vis, P11–13 vis, P15–16 vis, P18 vis | P0 shells | P1–P3 vis, P4–7 vis, P11–13 vis, P15–16 vis, P18 vis | — | 2026-08-24 | subagent:Graphics |
| Music | Music | audio cues (spawn/teleport/rescue/death/fail/cooldown/fire/invalid/clear) | — | — | — | 2026-08-24 | bootstrap |

---

## Phase P0–P20 Status  *(status: `[ ]` / `[~]` in-progress / `[x]` done+tested)*

| Phase | Title | Owner | Status | Notes |
|-------|-------|-------|--------|-------|
| P0 | Project Baseline | Transition + GameMechanism | [ ] | |
| P1 | Escort Basic Movement | GameMechanism | [ ] | |
| P2 | Escort Spawner | Gameplay | [ ] | |
| P3 | Obstacle / Death / Goal | Gameplay + Transition | [ ] | |
| P4 | Entrance Placement | Transition | [ ] | |
| P5 | Entrance Direction | Transition | [ ] | |
| P6 | Exit Placement | Transition | [ ] | |
| P7 | Exit Direction | Transition | [ ] | |
| P8 | Portal Teleport | GameMechanism | [ ] | |
| P9 | Portal Persistent | GameMechanism | [ ] | |
| P10 | Reconfiguration Cooldown | Transition | [ ] | |
| P11 | Speed Up Floor | GameMechanism | [ ] | |
| P12 | Slow Floor | GameMechanism | [ ] | |
| P13 | Turn Floor | GameMechanism | [ ] | |
| P14 | Portal + Floor Interaction | GameMechanism | [ ] | |
| P15 | Turret | Gameplay | [ ] | |
| P16 | Projectile | Gameplay | [ ] | |
| P17 | Prototype UI | UI/UX | [ ] | |
| P18 | Debug Visualization | UI/UX | [ ] | |
| P19 | First Playable Level | Gameplay + all | [ ] | |
| P20 | Final Prototype Check | Transition + all | [ ] | |

---

## Contract Change Notices
*(any change to `docs/Contracts.md` must be logged here: date + agent + what + why)*

- 2026-08-24 (bootstrap): initial `Contracts.md` established (Direction, EscortTarget,
  PortalPairController, GameManager, FloorEffect, Turret/Projectile, events, layers).

---

## Changelog
- 2026-08-24 (bootstrap): docs scaffold + `Progress.md` established as singular tracker;
  `AGENTS.md` enforced as mandatory entry; per-sprint docs created under `docs/sprints/`.
- 2026-08-24 (subagent:Graphics / feature/graphics): added Palette color-coding module + art plan; updated docs.
