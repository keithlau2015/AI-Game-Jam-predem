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
- **Current Sprint:** Sprint 1–2 (mechanics + portal loop being integrated)
- **Core Loop Stable** (Escort + Portal + Floor + Turret playable end-to-end): Partial — scripts integrated, Play Test pending
- **Last full Play Test:** none yet (scene assembly in progress)
- **Top Blocker:** none (code merged to main; needs Unity Play Test)
- **Branch strategy:** feature/* branches implemented in parallel; merged into main.

---

## Domain Matrix  *(every agent keeps its row current)*

| Domain | Owner Agent | Phases | Done | Current | Blocked By | Last Updated | Updated By |
|--------|-------------|--------|------|---------|-----------|--------------|-----------|
| GameMechanism | GameMechanism | P0, P1, P8, P9, P11–P14 | P0, P1 | P8, P9, P11–P14 | — | 2026-08-24 | subagent:GameMechanism |
| Gameplay | Gameplay | P2, P3, P15, P16, P19 | P2, P3 | P15, P16, P19 | — | 2026-08-24 | subagent:Gameplay |
| Transition | Transition | P0, P3, P4–P7, P10, P20 | P0, P3 | P4–P7, P10, P20 | — | 2026-08-24 | subagent:Transition |
| UI/UX | UI/UX | P17, P18 | — | P17, P18 | — | 2026-08-24 | subagent:UI/UX |
| Graphics | Graphics | P0, P1–3 vis, P4–7 vis, P11–13 vis, P15–16 vis, P18 vis | P0 shells | visual pass for P1–P3, P4–7, P11–13, P15–16, P18 | — | 2026-08-24 | subagent:Graphics |
| Music | Music | audio cues | — | audio cues (spawn/teleport/rescue/death/fail/cooldown/turret/invalid/clear) | — | 2026-08-24 | subagent:Music |

---

## Phase P0–P20 Status  *(status: `[ ]` / `[~]` in-progress / `[x]` done+tested)*

| Phase | Title | Owner | Status | Notes |
|-------|-------|-------|--------|-------|
| P0 | Project Baseline | Transition + GameMechanism | [~] | scripts in; scene assembly pending |
| P1 | Escort Basic Movement | GameMechanism | [~] | EscortTarget implemented; needs scene |
| P2 | Escort Spawner | Gameplay | [~] | implemented |
| P3 | Obstacle / Death / Goal | Gameplay + Transition | [~] | implemented |
| P4 | Entrance Placement | Transition | [~] | state machine scaffold |
| P5 | Entrance Direction | Transition | [~] | scaffold |
| P6 | Exit Placement | Transition | [~] | scaffold |
| P7 | Exit Direction | Transition | [~] | scaffold |
| P8 | Portal Teleport | GameMechanism | [~] | TryTeleport implemented |
| P9 | Portal Persistent | GameMechanism | [~] | implemented |
| P10 | Reconfiguration Cooldown | Transition | [~] | cooldown in placement controller |
| P11 | Speed Up Floor | GameMechanism | [~] | FloorEffect implemented |
| P12 | Slow Floor | GameMechanism | [~] | FloorEffect implemented |
| P13 | Turn Floor | GameMechanism | [~] | FloorEffect implemented |
| P14 | Portal + Floor Interaction | GameMechanism | [~] | order specified |
| P15 | Turret | Gameplay | [~] | implemented |
| P16 | Projectile | Gameplay | [~] | implemented |
| P17 | Prototype UI | UI/UX | [~] | PrototypeUI implemented |
| P18 | Debug Visualization | UI/UX | [~] | gizmos implemented |
| P19 | First Playable Level | Gameplay + all | [ ] | pending scene assembly |
| P20 | Final Prototype Check | Transition + all | [ ] | pending |

---

## Contract Change Notices
*(any change to `docs/Contracts.md` must be logged here: date + agent + what + why)*

- 2026-08-24 (bootstrap): initial `Contracts.md` established (Direction, EscortTarget,
  PortalPairController, GameManager, FloorEffect, Turret/Projectile, events, layers).
- 2026-08-24 (subagents): implemented all contract types across feature/* branches and merged to main.

---

## Changelog
- 2026-08-24 (bootstrap): docs scaffold + `Progress.md` established as singular tracker.
- 2026-08-24 (subagent:GameMechanism / feature/mechanism): Direction, DirectionUtility,
  EscortTarget, PortalEndpoint, PortalPairController, FloorEffect; docs updated.
- 2026-08-24 (subagent:Gameplay / feature/gameplay): EscortSpawner, Obstacle, Boundary, Goal,
  Turret, Projectile; docs updated.
- 2026-08-24 (subagent:Transition / feature/transition): GameManager + PortalPlacementController
  state machine; docs updated.
- 2026-08-24 (subagent:UI/UX / feature/ui-ux): PrototypeUI + debug gizmos/arrows/preview; docs updated.
- 2026-08-24 (subagent:Graphics / feature/graphics): Palette color-coding + art plan; docs updated.
- 2026-08-24 (subagent:Music / feature/music): PrototypeAudio event-driven cues; docs updated.
- 2026-08-24 (coordinator): merged 6 feature/* branches into main; integrated all scripts + agent
  docs; Progress.md consolidated.
