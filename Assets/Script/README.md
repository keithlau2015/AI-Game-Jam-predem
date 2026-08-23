# Script Modules Overview

Generic game template extracted from Sky_Garden. Each folder under `Assets/Script/` is a module with its own `README.md`.

## Status legend

| Status | Meaning |
|--------|---------|
| **Ready** | Usable as-is for new projects (minor polish OK) |
| **Partial** | Core idea works; controllers/APIs incomplete |
| **Needs refactor** | Works only with Sky_Garden assumptions; decouple before reuse |

## Module index

| Module | Status | Role |
|--------|--------|------|
| [Utilities](Utilities/README.md) | Ready | Singleton, Model, Addressables, tweeners, StateMachine |
| [FileManager](FileManager/README.md) | Ready | Encrypted save / CSV / log I/O |
| [LocalizationModule](LocalizationModule/README.md) | Ready | Multi-language strings + UI labels |
| [LogModule](LogModule/README.md) | Ready | `GameLog` gameplay logging |
| [ObjectPoolingModule](ObjectPoolingModule/README.md) | Ready | Addressables-backed object pools |
| [UIModule](UIModule/README.md) | Ready | UIManager, panels, hover/select contracts |
| [EventModule](EventModule/README.md) | Ready | Scene observables / observers |
| [AttributeModule](AttributeModule/README.md) | Ready | Entity attributes (HP, ATK, …) |
| [BehaviourTreeModule](BehaviourTreeModule/README.md) | Ready* | BT runtime (*move `CombatUnitBaseNode` out) |
| [DevTools](DevTools/README.md) | Ready | Editor CSV encrypt / fonts / version |
| [Managers](Managers/README.md) | Partial | Input, audio, graphics, loading, time |
| [AchievementModule](AchievementModule/README.md) | Partial | Data-driven achievements |
| [EquipmentModule](EquipmentModule/README.md) | Partial | Equipment data + skill anchors |
| [ItemModule](ItemModule/README.md) | Partial | Inventory stacks |
| [SkinModule](SkinModule/README.md) | Partial | Skinned-mesh skin swap |
| [CharacterModule](CharacterModule/README.md) | Partial | Character definition data only |
| [BugReportModule](BugReportModule/README.md) | Partial | Player bug report → Trello |
| [LevelModule](LevelModule/README.md) | Partial | Level select UI (load stubbed) |
| [AbilityModule](AbilityModule/README.md) | Partial | Skills via `ICombatUnit` (buffs unfinished) |
| [HitboxModule](HitboxModule/README.md) | Partial | `IDestructible` + child hitbox collector |
| [FormulaModule](FormulaModule/README.md) | Partial | Damage via `IAttributeHolder` |
| [ProjectileModule](ProjectileModule/README.md) | Partial | Projectiles via `ICombatUnit` |
| [RTSModule](RTSModule/README.md) | Partial | RTS control + `CombatUnit*` agents |
| [Bootstrap](Bootstrap/README.md) | Ready | Bootloader entry + GameEngineInitState |
| [NetworkModule](NetworkModule/README.md) | Partial | TCP client (configurable host; Request fixed) |

## Suggested boot order

1. `Utilities` / singletons in boot scene  
2. `FileManager` + `LocalizationModule` + `LogModule`  
3. `Managers` (Input, Time, Sound, Graphic, Loading) + `UIModule`  
4. `ObjectPoolingModule.SetUp`  
5. Game systems (Attributes, Events, SaveLoad, …)  
6. Optional: `RTSModule` for RTS camera / units  

## Cleanup done

1. SaveLoad: dual backends (RegistrySnapshot + DocumentDto), slot UI, enter-game after select  
2. Ability / Formula / Projectile: `ICombatUnit` / `IAttributeHolder`  
3. Stubs: `LoadLevel`, Item/Ability/Hitbox controllers, Network Request  
4. Ability buffs: `Buff` / `StackableBuff` / `BuffController`  
5. Secrets: `Resources/FileManagerCryptoConfig` + `Resources/BugReportConfig`  
6. RTS naming: `CombatUnitAgent`, `CombatUnitModel`, `ControllableUnit`, …  

## Before shipping

- Fill real Trello credentials in `BugReportConfig` (local only)  
- Rotate FileManager AES keys and re-encrypt CSVs  
- Add your levels to `LevelModel` + Build Settings  

