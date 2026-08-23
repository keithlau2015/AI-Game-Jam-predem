# Sprints.md — Parallel Execution Plan

Maps the 21 phases (P0–P20) into **sprints** designed for parallel AI-agent execution.
Each sprint lists the agents involved and the integration point that must pass before the
next sprint starts.

> Rule from TASKS.md: complete a phase, **Play Test in Unity**, then proceed.

## Sprint 0 — Baseline (serial-ish, short)
**Agents:** Transition + GameMechanism
- P0 scene/camera/ground/layers, P1 Direction + EscortTarget movement.
- **Gate:** scene runs, escort auto-moves 4 dirs, no errors.

## Sprint 1 — World & Hazards (parallel)
**Agents:** Gameplay (P2,P3,P15,P16) · GameMechanism (P11–P14 floor types) · Graphics (prefab shells) · Music (stub events)
- EscortSpawner, Obstacle/Boundary/Goal, Turret/Projectile, FloorEffect types.
- **Gate:** escorts spawn, die on hazards, reach goal → Rescue; floors change speed/turn.

## Sprint 2 — Portal Loop (integration-critical)
**Agents:** GameMechanism (P8,P9) + Transition (P4–P7,P10) tightly coupled on `PortalPairController` contract.
- Placement state machine, entrance/exit pos+dir, teleport, persistence, cooldown.
- **Gate:** full portal pair teleports matching-direction escorts; CD=3s; no edit during CD.

## Sprint 3 — Presentation (parallel)
**Agents:** UI/UX (P17,P18) · Graphics (visuals) · Music (cues)
- HUD, state/cooldown display, debug gizmos/arrows, portal & floor art, audio cues.
- **Gate:** player can read state, see range/arrows, hear cues.

## Sprint 4 — Level & Polish (parallel + integration)
**Agents:** Gameplay (P19) + all agents for cohesion.
- Assemble First Playable Level; verify it requires teleport+redirect+floor+turret-dodge.
- **Gate:** P19 acceptance passes via Play Test.

## Sprint 5 — Final Check (serial verification)
**Agents:** Transition (P20) leads; others fix issues.
- Start→Play→Clear/Fail→Retry stable; no NRE; no infinite trigger; build OK.

## Parallel agent matrix

| Sprint | GameMechanism | Gameplay | Transition | UI/UX | Graphics | Music |
|--------|--------------|----------|------------|-------|----------|-------|
| 0 | P0,P1 | — | P0 | — | P0 shell | — |
| 1 | P11–14 | P2,P3,P15,P16 | — | — | shells | stub |
| 2 | P8,P9 | — | P4–P7,P10 | — | portals | — |
| 3 | — | — | — | P17,P18 | art | cues |
| 4 | support | P19 | support | support | support | support |
| 5 | support | support | P20 | support | support | support |

## Conflict-avoidance rules (from research)
- Each agent writes ONLY its owned folders (see each `Agents/*.md`).
- Shared types live in `Core/` (GameMechanism) and are frozen by `Contracts.md`.
- Scene/prefab edits: one agent at a time (Transition owns scene bootstrap; Gameplay adds
  level content via prefabs, not by editing the shared scene directly where avoidable).
- Run `/clear` between unrelated tasks to keep agent context lean.
