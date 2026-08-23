# LocalizationModule

**Status:** Ready

## Purpose
Key → multi-language string lookup with UI auto-bind and a language preference panel.

## Entry points
| Type | Role |
|------|------|
| `LocalizationManager` | `GetLocalization`, `SetLanguage` |
| `LocalizationModel` | Row data (`tw`/`cn`/`en`/`jp`/`kr`) |
| `UISystemLabel` | Text bound to a localization key |
| `LangPrefSettingPanel` | Settings UI |

## How to use
1. Load localization rows into `LocalizationModel.map`.
2. Place `LocalizationManager` in the boot scene.
3. Use `UISystemLabel` on Texts, or call `GetLocalization(key, args)`.
4. Change language via `SetLanguage` / settings panel.
5. Keep `SYS_*` keys that other modules reference in the table.

## Dependencies
Utilities (`Singleton`, `Model`), UIModule (settings panel).
