# Portal Escort — Documentation Index

Modular documentation for the **Portal Escort** 2D top-down prototype, organized so that
multiple AI agents can implement the game **in parallel** without file/code conflicts.

## How to use these docs

> **MANDATORY ENTRY POINT:** Every AI agent (Claude, OpenCode, Cursor, Codex, Gemini, or any
> other) and every human MUST first read `AGENTS.md` at the repo root. It enforces a single
> source of truth and forces doc updates on every task.

1. Read `AGENTS.md` → the rules + forced doc-update discipline (start here, every session).
2. Read `Progress.md` → the **live whole-project tracker** (one file = total progress).
3. Read `GameDesign.md` → what the game is.
4. Read `TechnicalDesign.md` → how it is built (architecture, layers, data flow).
5. Read `TASKS.md` → the 21 phases (P0–P20) with acceptance criteria.
6. Read `Contracts.md` → the **shared public APIs** that agents must NOT break.
7. Read your agent doc in `Agents/` → your ownership boundary, files, deliverables, `## Status`.
8. Read `Sprints.md` + `sprints/SprintN.md` → which phases run in parallel and in what order.
9. Read `Workflow.md` → how to run parallel agents safely (git worktree / domain isolation).

## Document map

| File | Purpose | Owner |
|------|---------|-------|
| `../AGENTS.md` | **Mandatory entry point** + forced doc-update rules for ALL agents | All |
| `Progress.md` | **Single source of truth** — live whole-project status board | All |
| `GameDesign.md` | Game design / player-facing rules | All |
| `TechnicalDesign.md` | Architecture, layers, data contracts | All |
| `TASKS.md` | P0–P20 phase checklist + acceptance | All |
| `Contracts.md` | Shared public APIs (do not change lightly) | All |
| `Sprints.md` | Parallel sprint schedule | Coordinator |
| `Workflow.md` | Parallel agent execution guide | Coordinator |
| `Agents/GameMechanism.md` | Core engine: Direction, GameManager, Escort, Portal logic, Floor | Mechanism agent |
| `Agents/Gameplay.md` | Spawner, Obstacle, Goal, Boundary, Turret, Projectile, Level | Gameplay agent |
| `Agents/UI_UX.md` | PrototypeUI, prompts, debug visualization, Canvas | UI/UX agent |
| `Agents/Transition.md` | State machines: Game flow, Portal placement, Cooldown, Retry | Transition agent |
| `Agents/Graphics.md` | Sprites, materials, colors, Gizmos, VFX, scene look | Graphics agent |
| `Agents/Music.md` | BGM, SFX, audio cues, mixing | Music agent |

## Core principle

> Prototype 的成功標準不是功能數量，而是：核心 Escort + Portal + Floor Effect + Turret Loop 可以完整、穩定地玩一局。

Every agent shares the same `Direction` enum and `DirectionUtility` (see `Contracts.md`).
All tunable values are Inspector-driven (no hardcoding in gameplay logic).

## Folder convention (target)

```
Assets/Scripts/
├── Core/        (GameMechanism + Transition)
├── Escort/      (GameMechanism + Gameplay)
├── Portal/      (GameMechanism + Transition)
├── Level/       (Gameplay)
├── Combat/      (Gameplay)
├── UI/          (UI/UX)
└── Debug/       (UI/UX + Graphics)
```

Each agent owns specific subfolders — see its doc. Agents may read any file but should
write only inside their owned domain. Shared changes go through `Contracts.md` first.
