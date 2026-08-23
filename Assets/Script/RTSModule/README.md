# RTSModule

**Status:** Partial (camera/selection reusable; agents renamed to CombatUnit*)

## Purpose
RTS control layer extracted from Sky_Garden: camera, selection, pause/state machine, unit agents, BT combat nodes, unit data, world-space attribute UI.

## Folders
| Folder | Contents |
|--------|----------|
| `Input/` | `GameStateController`, `GameStateMachine` (pause + RTS camera/command hooks) |
| `Camera/` | `CameraController`, `MiniMapCam`, `CinemachineShake` |
| `Selection/` | `CursorManager` (hover + multi-select) |
| `Unit/` | `ControllableUnit`, `ObstacleAgent`, `FacingToAim` |
| `Agent/` | `CombatUnitAgent`, player/capture agents, BT action nodes |
| `Data/` | Battleship / equipment slot models |
| `WorldSpaceUI/` | Attribute bars / popups |

## Entry points
| Type | Role |
|------|------|
| `GameStateController` | Pause + owns state machine |
| `GameStateMachine` | Edge scroll, zoom, rotate, select/command hooks; **`LoadLevel` stub** |
| `CursorManager` | Raycast hover/select |
| `CombatUnitAgent` | Selectable combat unit |

## How to use
1. Boot scene: `GameStateController`, `CursorManager`, `CameraController`, Input + UI managers.
2. Configure Ground layer + selectable unit layers for raycasts.
3. Call `GameStateMachine.HookBattleInputAction` when RTS control should be live.
4. Spawn units with battleship data/agents (or rename/generalize for your genre).
5. **Implement `LoadLevel`** for your project (scene load + spawn + state).
6. Keep only Camera/Selection/Input if you need RTS camera without ship combat.

## Dependencies
Ability, Attribute, Equipment, Projectile, BehaviourTree, ObjectPooling, UI (`ISelectable`), Cinemachine, NavMesh, EPOOutline.

## Not included (by design)
Menu / open-world / shipyard flow, battle HUD panels, AVG/Naninovel, Quest/Newbie systems.

## Gaps / refactor
- Naming and agents are battleship-specific.
- `LoadLevel` is a no-op warning.
- `ControllableUnit` still has TODO error handling.
- Consider moving `CombatUnitBaseNode` here from BehaviourTreeModule.
