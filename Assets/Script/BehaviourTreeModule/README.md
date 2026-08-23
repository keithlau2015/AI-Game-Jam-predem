# BehaviourTreeModule

**Status:** Ready

## Purpose
Lightweight behaviour-tree runtime: tick executor, blackboard, composites, decorators, wait/timer/condition helpers.

## Entry points
| Type | Role |
|------|------|
| `TreeExecutor` | Attach to actor; implement `ConstructTree()` |
| `Node` | Base node |
| `Blackboard` | Shared BT memory |
| `Sequence` / `Selector` / `Parallel` | Composites |
| `Inverter` / `Succeeder` / `RepeatNode` | Decorators |
| `ConditionNode` / `WaitNode` / `TimerNode` | Helpers |

> Battleship-specific BT base lives in `RTSModule/Agent/CombatUnitBaseNode.cs`.

## How to use
1. Subclass `TreeExecutor` and implement `ConstructTree()`.
2. Build the tree with Sequence/Selector/Parallel + custom nodes.
3. Store shared state in `Blackboard`.
4. Attach the executor to the actor GameObject.
5. For pause: gate ticks yourself or use blackboard flags.

## Dependencies
None (generic BT only).
