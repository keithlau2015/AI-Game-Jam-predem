# Bootstrap

Game entry point for the Null template (ported from Sky_Garden Bootloader).

## Scene
`Assets/Scenes/Bootloader.unity` — first scene in Build Settings.

Contains: `NullTechGameEngineEntry`, `CursorManager`, `LoadingManager`, cameras, UI shell (`UIManager`, common popups, loading/pause panels).

## Flow
1. `NullTechEngineEntryPoint.Awake` → `GameStateController.InitializeEngine()`
2. `GameEngineInitState` loads data catalogs + object pools
3. Shows Addressable `LandingPanel` → `MainMenuPanel` / save / settings

## Scripts
| Path | Role |
|------|------|
| `NullTechEngineEntryPoint` | Scene entry |
| `GameState` / `GameEngineInitState` | Boot load sequence |
| `View/*` | Landing / main menu / announcements / create save |
