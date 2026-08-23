# EventModule

**Status:** Ready

## Purpose
Scene-level observer/observable system with AND/OR gates, trigger counts, saveable records, and stock triggers (area, key, time, attributes, spawn, entity count).

## Entry points
| Type | Role |
|------|------|
| `EvtObserable` | Base trigger (note typo in name) |
| `EvtObserver` | Base reaction |
| `GlobalEvtController` | Optional registry |
| `EvtNotifyData` / `EvtRecordData` | Payload + persistence |
| `ISpawnActivatable` | Post-spawn hook for pooled objects |

### Stock observables
`EnterAreaObservable`, `EnterSceneObservable`, `KeyPressedObserable`, `TimeBaseObserable`, `EntityCountObservable`, `NotifyCountObserable`, `PlayerItemCountObservable`, `UnitAttributeObservable`

### Stock observers
`ObjectSpawnObserver`, `UnitAttributeObserver`, `ResetObserver`

## How to use
1. Place an observable in the scene; assign a unique `id`.
2. Place an observer; link observables (AND/OR) and set `evtNameId`.
3. Subclass `EvtObserver.OnExecute` for custom actions, or use stock spawn/attribute observers.
4. For pool spawns: set pool key; implement `ISpawnActivatable` on prefabs if needed.
5. `EnterAreaObservable`: filter by tag/layer (generic — no battleship types).
6. Ignore or remap `EvtModel` enums like InstantWin / EnterBattle / PlayAVG if unused.

## Dependencies
ObjectPoolingModule, AttributeModule, ItemModule (item-count observable), SaveLoadModule (records).

## Gaps
- Naming typos (`Obserable`) kept for compatibility.
- Some `EvtModel.EventType` values are campaign leftovers.
