# Portal Escort — Documentation Index

Modular documentation for the **Portal Escort** 2D top-down prototype, organized so that
multiple AI agents can implement the game **in parallel** without file/code conflicts.

## How to use these docs

1. Read `GameDesign.md` → what the game is.
2. Read `TechnicalDesign.md` → how it is built (architecture, layers, data flow).
3. Read `TASKS.md` → the 21 phases (P0–P20) with acceptance criteria.
4. Read `Contracts.md` → the **shared public APIs** that agents must NOT break.
5. Read your agent doc in `Agents/` → your ownership boundary, files, deliverables.
6. Read `Sprints.md` → which phases run in parallel and in what order.
7. Read `Workflow.md` → how to run parallel agents safely (git worktree / domain isolation).

## Document map

| File | Purpose | Owner |
|------|---------|-------|
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
