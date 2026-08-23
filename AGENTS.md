# AGENTS.md — MANDATORY ENTRY POINT (all AI agents & humans)

> If you are an AI agent (Claude, OpenCode, Cursor, Codex, Gemini, or any other tool) **or** a
> human contributor working on **Portal Escort**, you MUST read this file first, every session,
> before touching any code, scene, or asset.

This project is built so that **many different AIs can work on it in parallel without losing
track of the whole**. The mechanism that makes that safe is a **single source of truth** that
everyone is forced to update.

---

## The one rule that overrides everything

**Before you start a task, and after every task, you MUST update the two tracking files:**

1. `docs/Progress.md` — the live whole-project status board (your row + Changelog).
2. `docs/Agents/<your-role>.md` — your domain doc (its `## Status` block).

Do **not** write code or change scenes/prefabs without first syncing your status here.
If you cannot update the docs, **STOP and tell the user** — never silently skip the update.

---

## Singular point of entry — read in this order

1. **`AGENTS.md`** (this file) — the rules.
2. **`docs/Progress.md`** — the whole-project tracker: what's done, who owns what, what's
   blocked, current sprint. *This is the single place that reflects total progress.*
3. **`docs/README.md`** — the document map.
4. **`docs/Contracts.md`** — frozen shared APIs. Never break these without notice.
5. **`docs/Agents/<your-role>.md`** — your ownership, tasks, and Status block.
6. **`docs/Sprints.md`** + **`docs/sprints/SprintN.md`** — what to build now.

---

## Mandatory doc-update discipline (forced for every agent)

After **ANY** change you make (code, prefab, scene, config), you MUST:

1. Update the `## Status` block at the top of `docs/Agents/<your-role>.md`:
   - `Last Updated` (YYYY-MM-DD), `Updated By` (your agent/AI name, e.g. `opencode/claude`),
     `Phases Done`, `Current`, `Blocked`.
2. Update your row in `docs/Progress.md` (Domain Matrix) and append one line to its
   `## Changelog` with date + agent + what changed.
3. If you changed a shared contract, update `docs/Contracts.md` **and** log it in
   `docs/Progress.md` → `## Contract Change Notices` before anyone merges.
4. Mark a phase `[x]` in `TASKS.md` / `Progress.md` **only after a real Unity Play Test**
   passes its acceptance criteria.

Example Changelog line:
`- 2026-08-24 (opencode): GameMechanism — finished P8 teleport; updated Contracts §3.`

---

## Anti-drift rules (how parallel agents stay safe)

- **Own your folders only.** Write inside your domain (see your agent doc). Outside changes →
  propose in `docs/Progress.md` and let the owning agent do it.
- **Contracts are APIs.** `Direction`, `DirectionUtility`, `EscortTarget`, `PortalPairController`,
  `GameManager` events are frozen. Change via `docs/Progress.md` notice + `Contracts.md` update.
- **No hardcoded tunables.** All Inspector values in `Contracts.md` §9 stay serialized.
- **One tracker.** All progress lives in `docs/Progress.md`. Do not keep private status notes
  that drift from it.

---

## Current focus

Read `docs/Progress.md` → `## Current Sprint` for what to do right now.

---

## Why this exists

> Prototype 的成功標準不是功能數量，而是：核心 Escort + Portal + Floor Effect + Turret Loop
> 可以完整、穩定地玩一局。

Many AIs, one coherent game. The docs are the glue. If the docs are wrong, the game breaks —
so updating them is part of the work, not extra credit.
