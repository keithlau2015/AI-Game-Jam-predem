# BugReportModule

**Status:** Partial (credentials externalized)

## Purpose
Player bug reports with pluggable reporters. Trello is implemented; Notion is still unimplemented.

## Entry points
| Type | Role |
|------|------|
| `BugReport` | Build + `SendReport()` |
| `BugReportConfig` | Credentials ScriptableObject (`Resources/BugReportConfig`) |
| `BugReportPanel` | UI entry |

## How to use
1. Create **Null Template → Bug Report Config** and save as `Assets/Resources/BugReportConfig.asset`.
2. Fill Trello API key / token / board id (do not commit secrets).
3. Open `BugReportPanel` from settings, or call `BugReport` API from code.
4. Screenshots are optional (`persistentDataPath/bugReport.jpg`).

## Dependencies
UIModule, LocalizationModule, LoadingManager, TimeManager.
