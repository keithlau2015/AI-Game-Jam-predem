# AchievementModule

**Status:** Partial

## Purpose
Data-driven achievements that unlock by observing `INotifyPropertyChanged` fields on other models, with chain/progress queries.

## Entry points
| Type | Role |
|------|------|
| `AchievementController` | `IsAchieved`, chain progress queries |
| `AchievementModel` | Definition (observe model/field/target) |
| `AchievementHistoryModel` | Saveable progress + auto-subscribe |

## How to use
1. Author `AchievementModel` rows (observe type name, field, target value, optional pre-req).
2. Ensure observed types fire `INotifyPropertyChanged` and live in `Model<T>.map`.
3. Create/load `AchievementHistoryModel` per achievement for the player.
4. Query UI/progress via `AchievementController`.
5. Persist histories through SaveLoad like other `SaveableModel`s.

## Dependencies
SaveLoadModule, Utilities `Model`.

## Gaps
- No public force-grant API.
- Unlock path is implicit via history construction / reflection.
