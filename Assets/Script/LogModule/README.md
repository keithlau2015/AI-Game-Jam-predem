# LogModule

**Status:** Ready

## Purpose
Generic gameplay logger: timestamped lines to disk (via FileManager) and Unity console.

## Entry points
| Type | Role |
|------|------|
| `GameLog.logger` | `Log` / `Warning` / `Error` |
| `onLogging` | Event for in-game consoles |

## How to use
1. Ensure FileManager is available (`FileType.Log`).
2. Call `GameLog.logger.Log("message")` from any system.
3. Optionally subscribe to `onLogging` for a debug HUD.
4. Place `TimeManager` if you want dated filenames.
5. Logging is skipped while `GameStateController.IsPause` is true (if present).

## Dependencies
FileManager, optional TimeManager / GameStateController.

## Notes
Session time prefers RTS `curBattleTime` when that state machine is active; otherwise uses elapsed session time.
