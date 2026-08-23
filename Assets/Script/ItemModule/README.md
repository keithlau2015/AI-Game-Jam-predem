# ItemModule

**Status:** Partial

## Purpose
Inventory stack data keyed by owner, backed by static `ItemModel` definitions.

## Entry points
| Type | Role |
|------|------|
| `ItemData` | Runtime stacks (`Stack`, owner queries) |
| `ItemModel` | Definition (`maxStack`, icon/entity ids) |
| `ItemController` | `Grant` / `Consume` / `GetCount` |

## How to use
1. Load `ItemModel` rows into `ItemModel.map`.
2. `ItemController.Grant(ownerUID, itemId, count)`.
3. `ItemController.Consume(ownerUID, itemId, count)`.
4. Query with `GetCount` or `ItemData.GetItemsByOwner*`.
5. Persist via SaveLoad (`SaveableModel`).

## Dependencies
SaveLoadModule, ObjectPoolingModule, Utilities `Model`.
