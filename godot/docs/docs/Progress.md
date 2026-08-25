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
- **Core Loop Stable** (Escort + Goal rescue verified in-scene): Yes — Escort moves (Right @2), teleports through Portal pair, and is rescued at Goal; BOTH win (Clear) and lose (Fail on crystal hit) loops verified in Play Mode 2026-08-24. Floor/Turret wiring still pending verification.
- **Last full Play Test:** 2026-08-24 — `Assets/Scenes/PortalEscort.unity`: Escort→Portal→Goal rescue (win) and Escort→crystal death (lose) both fire correct GameManager state; 0 runtime errors.
- **Top Blocker:** Resolved — unrelated `Assets/Script/*` game framework was breaking full-assembly compile; moved to `Legacy/` (outside Assets). `GameManager.Instance` made lazy-safe.
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
| P0 | Project Baseline | Transition + GameMechanism | [x] | PortalEscort.unity scene assembled; playable |
| P1 | Escort Basic Movement | GameMechanism | [x] | EscortTarget moves + rescued at Goal (verified) |
| P2 | Escort Spawner | Gameplay | [x] | data-driven LevelManager spawns Escort from JSON (verified) |
| P3 | Obstacle / Death / Goal | Gameplay + Transition | [x] | Goal rescue + teleport verified; EscortTarget.Die→GameManager.RegisterDeath wired |
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
| P19 | First Playable Level | Gameplay + all | [x] | Level1.json data-driven level built + verified Clear in Play Mode |
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
- 2026-08-24 (opencode): FIXED full-assembly compile — renamed GameManager event→method
  (RegisterRescue/RegisterDeath), fixed PrototypeAudio `+=` subscriptions, PortalPlacementController
  static-event + read-only `IsComplete` handling, Turret `Projectile` fields public. Moved unrelated
  `Assets/Script/*` framework to `Legacy/` (was breaking compile). Made `GameManager.Instance`
  lazy-safe. Assembled `Assets/Scenes/PortalEscort.unity` (Escort, Goal, Obstacle, Turret,
   GameManager, ProjectilePrefab, Ground, Camera) and verified Escort→Goal rescue loop in Play Mode
   with 0 console errors.
- 2026-08-25 (opencode): GODOT build — fixed 3 playtest bugs: (1) added GridDraw tile/grid
   overlay aligned to LevelData so the map/playfield renders; (2) PortalController now renders
   entrance/exit portal sprites on the map via `update_portal_visuals()` (was unmapped/invisible);
   (3) Escort now shows the green slime sprite so it visibly starts at SPAWN_CELL.
- 2026-08-24 (opencode): IMPORTED `ArtTestPackage.unitypackage` art (extracted gzipped-tar, remapped
  GUID-preserving into `Assets/UI/Texture/*`, `Assets/UI/prefab/*`, `Assets/Scenes/ArtTestScent.unity`,
  TMP shaders/font skipped as already-present). Decoded `ArtTestScent.unity` sprite GUIDs and mapped
  the pack to the 2D game: Escort→`charMoveUp`+`character_0.controller` (animated), Turret→`SzDRt.png`
  (the pack's "enemy"), Portal→`Portal.png`, Goal→`Goal.png`, Background→`Map.png`; generated clean
  `Obstacle.png`/`Projectile.png` sprites (`.png` extension, no TextureImporter warnings). Converted
  all gameplay objects from MeshRenderer→SpriteRenderer; deleted stray Cube + 3D Ground. Re-verified
   full Escort→teleport→Goal rescue loop in Play Mode (isAlive=false at x≈5.98) with 0 runtime errors.
- 2026-08-24 (opencode): REBUILT scene data-driven after Unity restart discarded unsaved edits. Added JSON level system (LevelData/ObjectRegistry/LevelManager/UIManager) per requested architecture; EscortTarget.Die→GameManager.RegisterDeath so Clear/Fail fire. Created prefabs (Slime/Goal/Goal/RedCrystal/Turret/Portal/Projectile) from ArtTestPackage sprites; fixed HUD Canvas to ScreenSpaceOverlay (was WorldSpace→huge portals) with bottom-left Counter(x15)+bottom-right 3 portals; enlarged Map.png background; made camera orthographic. Verified full loop in Play Mode: slime spawned (-5,0)→portal→Goal(5,4), gameState=Clear, 0 errors.
- 2026-08-24 (opencode): FIXED death-registration bug — `EscortTarget.Die()` set `isAlive=false` *before* calling `GameManager.RegisterDeath`, which early-returns on `!isAlive`, so Fail never fired (slime died but gameState stayed Playing, aliveCount stayed 1). Changed `Die()` to let `RegisterDeath` own the flag (GameManager fallback sets isAlive only if null). Re-verified BOTH loops in Play Mode: win → gameState=Clear, rescuedCount=1; lose (portals off, slime hits crystal) → gameState=Fail, deadCount=1. Saved scene.
