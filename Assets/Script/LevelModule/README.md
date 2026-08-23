# LevelModule

**Status:** Partial

## Purpose
Level catalog + select UI that confirms a pick and asks the game-state layer to start that level.

## Entry points
| Type | Role |
|------|------|
| `LevelModel` | `sceneIndex`, name, description |
| `SelectLevelPanel` | Loop-scroll picker |
| `OneLevel` | Row view |

## How to use
1. Populate `LevelModel.map` (key → scene index / name).
2. Show `SelectLevelPanel` via UIManager; `SetUp` + `Show`.
3. `GameStateMachine.LoadLevel(key)` loads `LevelModel.sceneIndex` from Build Settings.
4. Add your scenes to Build Settings so indexes match.
5. Keep `OnErrorOccur` wiring for failed starts (panel already listens).

## Dependencies
UIModule, LocalizationModule, LoadingManager, RTSModule `GameStateController` / `GameStateMachine`.

## Gaps
- Spawn / mission setup after scene load is still project-specific.
- Addressables scene loading not wired (build-index only).
