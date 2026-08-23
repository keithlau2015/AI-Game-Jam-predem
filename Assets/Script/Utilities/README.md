# Utilities

**Status:** Ready

## Purpose
Shared foundation used by almost every module: singletons, data models, Addressables helpers, DOTween tweeners, and a lightweight state machine.

## Entry points
| Type | Role |
|------|------|
| `Singleton<T>` | Scene-persistent manager base |
| `Model<T>` | Static keyed data catalog (`.map`) |
| `AssetsBundleManager` / `GameAssetsBundleManager` | Addressable load helpers |
| `State` / `StateMachine` | Custom flow states |
| `Tweener_*` | UI/world tween components |
| `Ownership` / `Team` | Side / ownership enums |
| `DebugController` | Debug helpers |

## How to use
1. Put critical managers (that inherit `Singleton<T>`) in the boot scene.
2. Define static data as `Model<T>` subclasses; load into `.map` via FileManager/CSV.
3. Load assets with Addressable keys through the bundle managers (mind handle lifetime).
4. Subclass `StateMachine` for custom game flows, or use RTS `GameStateMachine`.
5. Attach tweeners on UI for show/hide motion.

## Dependencies
DOTween, Addressables, UniTask (via callers).

## Notes
- Addressables helpers may release handles aggressively — keep references if you need long-lived assets.
- `Team` is combat-oriented; replace if your game does not use sides.
