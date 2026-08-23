# FileManager

**Status:** Ready

## Purpose
Central file I/O: encrypted JSON saves, encrypted CSV model loading, log directory writes, and directory helpers.

## Entry points
| API | Role |
|-----|------|
| `FileManager.SaveFile` / `WriteFile` / `LoadFile` | Object persistence |
| `LoadEncryptedModel` / `SaveCSV` / `LoadCSV` | Data pipeline |
| `FileType` | `Config`, `Save`, `Log` roots |
| `onFinishSaveObj` | Save-complete callback |

## How to use
1. Call any API — `Init()` runs lazily and sets paths under `Application.dataPath` / `persistentDataPath`.
2. Author CSV configs; encrypt with DevTools or `SaveCSV`.
3. Runtime: `LoadEncryptedModel<T>()` for catalogs; `LoadFile` / `SaveFile` for player saves.
4. Logs go to `persistentDataPath/GameLog/` (`FileType.Log`).
5. Create `Assets/Resources/FileManagerCryptoConfig.asset` (**Null Template → File Manager Crypto Config**) and set AES key/IV.
6. **Before shipping:** rotate those keys (re-encrypt config CSVs after rotation).

## Dependencies
UniTask, Newtonsoft.Json.

## Notes
If the Resources config is missing, a built-in **dev fallback** key is used (with a warning) so existing encrypted data still loads.
