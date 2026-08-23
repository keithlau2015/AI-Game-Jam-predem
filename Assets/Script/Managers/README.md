# Managers

**Status:** Partial

## Purpose
Cross-cutting singletons: input, audio, graphics, loading UI, time, plus settings panels. Intended boot-layer services for any game.

## Entry points
| Type | Role |
|------|------|
| `InputManager` | Input System wrapper, back-stack, multi-select mode |
| `SoundManager` | Audio playback + prefs |
| `GraphicManager` | Quality / display prefs |
| `LoadingManager` | Loading overlay + progress tasks |
| `TimeManager` | Datetime / time helpers |
| `GameplayManager` | **Empty shell** — extend or delete |
| Settings panels | Audio / Graphic / Control / Game setting UIs |

## How to use
1. Place Input, Sound, Graphic, Loading, Time managers in the boot scene (with UIManager).
2. Call `SetUp` on Input / Sound / Graphic from saved prefs.
3. Drive loading UX: `LoadingManager.Show` → `AddTask` → `Hide`.
4. Play SFX/BGM through `SoundManager`.
5. Open settings via UIManager Addressable panels.
6. Implement gameplay rules in a real manager — do not leave `GameplayManager` empty in shipping projects.

## Dependencies
Utilities `Singleton`, UIModule, FileManager, Unity Input System (`PlayerControl`).

## Gaps
- `GameplayManager` has no gameplay API yet.
- Prefs persistence assumes FileManager paths exist.
