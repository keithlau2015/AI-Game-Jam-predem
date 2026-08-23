# SaveLoadModule

**Status:** Dual-backend template (RegistrySnapshot + DocumentDto)

## Purpose
Persist campaign / player progress. The template ships **two designs** behind one facade (`SaveService`) so you can keep the original reflective approach or switch to a typical shipped-game SaveGame DTO document.

## Folder layout

| Folder | Design |
|--------|--------|
| `Shared/` | `SaveService`, `ISaveBackend`, `SaveSlotInfo`, settings |
| `RegistrySnapshot/` | Original design (refactored): `SaveableModel<T>` + `SaveDataModel` blob |
| `DocumentDto/` | Usual shipped pattern: `SaveGameDocument` + `ISaveParticipant` sections |
| `View/` | Select / menu UI (slot-list based) |

## Choosing a backend

1. Create `Resources/SaveLoadSettings` (menu: **Null Template → Save Load Settings**), or
2. Call `SaveService.SetBackend(SaveBackendKind.DocumentDto)` at boot.

Default is **RegistrySnapshot**.

## Shared API

```csharp
await SaveService.CreateSave("Slot 1");
SaveService.LoadSave(slotId, out string error);
await SaveService.DeleteSave(slotId);
IReadOnlyList<SaveSlotInfo> slots = SaveService.ListSlots();
```

`SaveLoadController` still exists as a thin compatibility wrapper over `SaveService`.

## RegistrySnapshot (your original, refactored)

- Types inherit `SaveableModel<T>` and live in static `map`s.
- Save captures every `SaveableModel<>` map into one `SaveDataModel` **per slot**.
- Files: `persistentDataPath/Game/reg_{slotId}` (encrypted via FileManager).
- Fixes vs old code: boot catalog from disk, one file per slot, typed JSON (`preserveTypeInfo`), side-index rebuild (`ItemData.RebuildOwnerGroups`), UI uses slot list not `map[int]`.

## DocumentDto (usual shipped SaveGame)

- Root document: `SaveGameDocument` (version, slot meta, `sections` dictionary).
- Each system implements `ISaveParticipant` (`CaptureJson` / `RestoreJson` / `ClearRuntime`).
- Defaults registered in `DocumentSaveBootstrap` (items, equipment, units, slots, skins, achievements, events, meta).
- Files: `persistentDataPath/Game/dto_{slotId}`.
- Add a section: implement `ISaveParticipant`, then `RegisterParticipant` or extend the bootstrap.

## Dependencies
FileManager, UIModule, Localization, LoadingManager, GameStateController, LevelModule, UniTask, Newtonsoft.Json.
