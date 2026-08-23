# Workflow.md — Running Parallel AI Agents

Synthesized from current best-practice references for multi-agent Unity game dev
(Claude/OpenCode game-studio scaffolds, git worktree parallelism, dispatcher orchestration).

---

## 0. MANDATORY: single entry point & doc updates (read `AGENTS.md` first)

Every agent — no matter which AI tool runs it — MUST:

1. Read `AGENTS.md`, then `docs/Progress.md`, then its `docs/Agents/<role>.md`.
2. **Before and after each task**, update:
   - `docs/Progress.md` → its Domain Matrix row + `## Changelog` (date + agent + change).
   - `docs/Agents/<role>.md` → its `## Status` block (Last Updated, Updated By, Phases Done,
     Current, Blocked).
3. Never break `docs/Contracts.md` without logging a `## Contract Change Notice` in `Progress.md`.
4. Mark a phase done `[x]` only after a real Unity Play Test passes acceptance.

This is what keeps many AIs coherent. Skipping doc updates = drift = broken game.

## Principles (why parallel works)
1. **Domain separation** — each agent owns folders; files don't overlap → few merge conflicts.
2. **Contracts as APIs** — shared types frozen in `Contracts.md`; change via proposal only.
3. **Lean context** — keep each agent on ONE task; `/clear` between tasks. Context is the
   bottleneck, not speed.
4. **Event-driven decoupling** — agents talk via `GameManager` events, not direct calls.
5. **Play-test gates** — every sprint ends with a real Unity Play Test before next sprint.

## Option A — Same-folder, multi-terminal (simplest)
Open several OpenCode/Claude terminals in the project root. Assign each a different agent doc:
```
terminal 1: "You are the GameMechanism agent. Read docs/README.md, docs/Contracts.md, docs/Agents/GameMechanism.md. Implement your phases."
terminal 2: "You are the Gameplay agent. Read docs/README.md, docs/Contracts.md, docs/Agents/Gameplay.md. ..."
... etc
```
- Give each agent its exact owned folders and the contract file.
- Tell them: "Read-only outside your domain; write only inside it."
- Use Unity MCP to verify compilation after each agent's batch.

## Option B — git worktree isolation (safest for heavy parallel)
For larger teams, give each agent its own worktree + branch so Unity Library and scenes
don't collide:
```
git worktree add ../PortalEscort-mechanism feature/mechanism
git worktree add ../PortalEscort-gameplay  feature/gameplay
...
```
- Each worktree opens its own Unity Editor (costs RAM; 32GB+ recommended).
- Merge back per sprint; resolve only cross-domain contract changes (rare if `Contracts.md` held).

## Coordination contract
- **Source of truth:** `docs/Contracts.md`. Any shared-API change → update it first, then agents.
- **Shared state:** `GameManager` events. UI/UX and Music subscribe; they never call gameplay.
- **Scene/prefab rule:** avoid two agents editing the same `.unity`/`.prefab` simultaneously.
  Transition owns scene bootstrap; Gameplay delivers level content as prefabs.
- **Integration seam:** `PortalPairController.IsComplete` / `isActive` (Mechanism ↔ Transition).
  Keep this exact pairing stable through Sprint 2.

## Recommended launch order (fastest speedrun)
1. Sprint 0: Transition + GameMechanism (baseline + Direction + EscortTarget).
2. Sprint 1: Gameplay + GameMechanism(floors) + Graphics(shells) in parallel.
3. Sprint 2: Mechanism + Transition on the Portal contract (the critical pairing).
4. Sprint 3: UI/UX + Graphics + Music in parallel.
5. Sprint 4: Gameplay assembles P19 level; others polish.
6. Sprint 5: Transition verifies P20; fix-forward.

## Quality gates (per sprint)
- No compile errors (Unity MCP `read_console`).
- Acceptance checkboxes in `TASKS.md` for that phase are met via Play Test.
- `Contracts.md` unchanged or updated-with-approval only.

## Tooling notes
- Use Unity MCP (`manage_gameobject`, `manage_components`, `manage_scene`, `read_console`)
  to create/verify objects without hand-editing scenes.
- Prefer skills/short prompts over huge MCP payloads to keep context lean.
- After each agent batch: refresh Unity + read console for compile errors before next agent.
