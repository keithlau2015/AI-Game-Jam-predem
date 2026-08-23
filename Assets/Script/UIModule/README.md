# UIModule

**Status:** Ready (minor stubs)

## Purpose
UI shell: layered Addressable panel loading, back stack, common pop-ups, hover/select contracts, countdown, drag/drop, loading/pause helpers.

## Entry points
| Type | Role |
|------|------|
| `UIManager` | `LoadUI`, previous stack, common pop-ups |
| `IPreviousablePanel` | Back-navigation panels |
| `IHoverable` / `ISelectable` | World/UI interaction contracts |
| `CommonPopUpPanel` / `CommonPopTextPanel` | Confirm / message UI |
| `LoadingPanel`, `PausePanel`, `CreditPanel` | Standard panels |
| `Draggable` / `DropSlot`, `ButtonReaction` | Interaction helpers |
| `CountDownController`, `PromptDotController` | Timed / prompt UX |

## How to use
1. Wire `UIManager` layers + common pop-up references in the boot scene.
2. Register panels as Addressables; open with `await UIManager.singleton.LoadUI<T>(key)`.
3. Implement `IPreviousablePanel` for screens that use the back stack.
4. Use `ShowCommonPopUpTextPanel` for confirms.
5. Implement `ISelectable` / `IHoverable` on world objects for `CursorManager` (RTS).
6. Replace `CreditPanel` content per project (currently a stub).

## Dependencies
InputManager, Addressables, LocalizationModule, Utilities tweeners.

## Gaps
- `CreditPanel` is minimal placeholder content.
