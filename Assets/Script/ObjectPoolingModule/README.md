# ObjectPoolingModule

**Status:** Ready

## Purpose
Addressables-driven object pools built from `EntityModel` definitions, with spawn/discard lifecycle events.

## Entry points
| Type | Role |
|------|------|
| `ObjectPoolManager` | `SetUp`, `pools` dictionary |
| `ObjectPool` | `SpawnFromPool` |
| `PoolObjectProperty` | `onSpawn` / `onDiscard` |
| `EntityModel` | Prefab Addressable key per entity |

> Namespace is currently `ObjetPoolModule` (typo). Prefer fixing later; do not mix spellings.

## How to use
1. Fill `EntityModel.map` with Addressable prefab keys.
2. Prefabs should include `PoolObjectProperty` (added automatically if missing during setup).
3. Boot: `await` / call `ObjectPoolManager.singleton.SetUp(progress)`.
4. Spawn: `ObjectPoolManager.singleton.pools[id].SpawnFromPool(position)`.
5. Disable/return objects so they re-enter the pool.
6. Listen to `onObjectSpawn` / `onObjectDiscard` for EventModule hooks.

## Dependencies
Addressables, Utilities (`Singleton`, `Model`).
