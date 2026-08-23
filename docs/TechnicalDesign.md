# TechnicalDesign.md — Portal Escort

## 1. Technical Goal

建立一個可快速迭代的 2D Top-down Prototype。

技術優先順序：

1. 功能清楚。
2. 容易 Debug。
3. 所有關卡數值可由 Inspector 修改。
4. 避免過度架構。
5. 避免不必要的 AI / NavMesh / Physics 複雜度。

---

# 2. Technical Assumptions

## Game Type
2D Top-down

## Movement
Continuous 2D movement

## Main Directions
使用 Enum：

```text
Direction
- Up
- Right
- Down
- Left
```

Direction 對應 Vector2：

```text
Up    = (0, 1)
Right = (1, 0)
Down  = (0,-1)
Left  = (-1,0)
```

---

# 3. Recommended Script Structure

Prototype 建議：

```text
Assets/
└── Scripts/
    ├── Core/
    │   ├── GameManager.cs
    │   └── DirectionUtility.cs
    │
    ├── Escort/
    │   ├── EscortTarget.cs
    │   └── EscortSpawner.cs
    │
    ├── Portal/
    │   ├── PortalPlacementController.cs
    │   ├── PortalPairController.cs
    │   └── PortalEndpoint.cs
    │
    ├── Level/
    │   ├── Goal.cs
    │   ├── Obstacle.cs
    │   ├── Boundary.cs
    │   └── FloorEffect.cs
    │
    ├── Combat/
    │   ├── Turret.cs
    │   └── Projectile.cs
    │
    └── UI/
        └── PrototypeUI.cs
```

不要為 Prototype 建立大量抽象 Base Class。

---

# 4. DirectionUtility

DirectionUtility 負責：

- Direction → Vector2 values
- Direction → Rotation
- Direction Compare

例如：

```text
GetVector(Direction.Right)
→ Vector2.right
```

所有角色、Portal、Turn Floor、Turret 共用相同 Direction 定義。

---

# 5. EscortTarget

EscortTarget 負責：

- Current Direction
- Base Move Speed
- Current Move Speed
- Forward Movement
- Portal Teleport
- Floor Effect
- Death
- Goal Detection

主要資料：

```text
baseMoveSpeed
currentMoveSpeed
currentDirection
isAlive
```

Update / FixedUpdate：

> 持續沿 currentDirection 移動。

不使用 NavMesh。

---

# 6. Escort Spawn

EscortSpawner Inspector Data：

```text
spawnPosition
spawnCount
spawnInterval
moveSpeed
startDirection
escortPrefab
```

流程：

```text
Level Start
↓
Spawn Escort
↓
Wait SpawnInterval
↓
Spawn Next Escort
↓
直到 SpawnCount 完成
```

---

# 7. Portal Placement State Machine

PortalPlacementController 使用：

```text
PortalPlacementState
- Idle
- SelectingEntrancePosition
- SelectingEntranceDirection
- SelectingExitPosition
- SelectingExitDirection
- ActiveLocked
- ActiveReconfigurable
```

---

# 8. Portal Placement Input

玩家使用滑鼠。

Prototype 建議：

### Position Selection
滑鼠位置轉換成 World Position。

### Direction Selection
根據滑鼠相對 Portal Position 的方向決定四方向。

例如：

```text
abs(delta.x) > abs(delta.y)

→ Left / Right

否則

→ Up / Down
```

因此不需要自由旋轉。

---

# 9. Entrance Placement Validation

Entrance Position 必須：

- 位於 Ground Layer。
- 不與 Obstacle 重疊。
- 不與 Hazard 重疊。
- 不與 Turret 重疊。
- 不與 Goal 重疊。
- 不與 Existing Portal 重疊。
- 不與 Escort Target 重疊。

非法位置：

> 不允許 Left Click Confirm。

---

# 10. Exit Placement Validation

Exit 除上述條件外：

必須滿足：

```text
Vector2.Distance(
    entrancePosition,
    exitPosition
) <= 6f
```

MaxPortalDistance：

```text
6f
```

應設為 Inspector 可調參數。

---

# 11. Incomplete Portal

Portal Pair 未完成時：

```text
isActive = false
```

PortalEndpoint Collider 即使與 Escort 接觸：

> 不執行 Teleport。

不可因未完成 Portal 而：

- 改方向。
- Teleport。
- Death。
- Stop Movement。

---

# 12. Entrance Direction Check

EscortTarget 接觸 Active Entrance：

只有：

```text
escort.currentDirection
==
entrance.direction
```

才執行 Teleport。

不符合：

> Ignore。

---

# 13. Teleport Logic

成功進入 Entrance：

```text
escort position
=
exit position + exit safe offset
```

然後：

```text
escort.currentDirection
=
exit.direction
```

Teleport：

> Instant。

---

# 14. Teleport Safe Offset

避免 Escort 出現在 Exit Collider 中造成重複 Trigger。

建議：

```text
exitPosition
+
DirectionVector(exitDirection) * exitOffset
```

Prototype：

```text
exitOffset ≈ 0.5–1.0 world unit
```

實際值依 Collider 大小調整。

---

# 15. Teleport Lock

建議 EscortTarget 增加短暫：

```text
teleportLock
```

例如：

```text
0.1–0.2 sec
```

用途：

防止同一 Frame / Trigger 狀態造成重複傳送。

---

# 16. Portal Persistent

Portal Pair 完成後：

```text
isActive = true
```

不因 Escort 通過而 Destroy。

多個 Escort 可以連續觸發。

---

# 17. Reconfiguration Cooldown

Portal Pair 完成後：

```text
reconfigurationCooldown = 3f
```

State：

```text
ActiveLocked
```

Countdown 結束：

```text
ActiveReconfigurable
```

注意：

Cooldown 期間：

```text
Portal isActive = true
```

傳送功能不能被停用。

Cooldown 只限制：

> Player Edit。

---

# 18. Reconfiguration

在 ActiveReconfigurable 狀態：

玩家可以開始重新建立 Portal Pair。

Prototype 可採：

> 建立新 Portal 時移除舊 Portal。

避免同時存在兩組半完成 Portal。

重新完成配置後：

> Cooldown 重設為 3 秒。

---

# 19. Portal Range Preview

SelectingExitPosition 時：

顯示以 Entrance 為中心：

> Radius = 6 world units

的範圍 Preview。

Preview 只作視覺提示。

實際合法判定仍使用 Distance。

---

# 20. FloorEffect

FloorEffect Data：

```text
effectType
speedMultiplier
turnDirection
```

Enum：

```text
FloorEffectType
- Normal
- SpeedUp
- SlowDown
- Turn
```

---

# 21. Speed Floor Logic

Escort 進入 SpeedUp：

```text
currentMoveSpeed
=
baseMoveSpeed * speedMultiplier
```

Escort 離開：

```text
currentMoveSpeed
=
baseMoveSpeed
```

Prototype 先避免多層 Speed Effect Stack。

---

# 22. Slow Floor Logic

與 SpeedUp 共用相同倍率邏輯。

例如：

```text
speedMultiplier = 0.5f
```

---

# 23. Turn Floor Logic

Escort 進入 Turn Floor：

```text
currentDirection
=
turnDirection
```

立即生效。

---

# 24. Exit + Floor Effect Order

Teleport 流程：

```text
1. Move Escort to Exit
2. Set Exit Direction
3. Release Teleport Lock when appropriate
4. Floor Trigger applies effect
5. Continue Movement
```

如果 Exit 位於 Turn Floor：

Turn Floor 可以覆蓋 Exit Direction。

---

# 25. Obstacle

Obstacle 使用 Collider2D。

Escort 接觸：

```text
Die()
```

Projectile 接觸：

```text
DestroyProjectile()
```

Portal Placement：

Obstacle Layer 視為 Invalid。

---

# 26. Goal

Goal 使用 Trigger Collider2D。

Escort 進入：

```text
GameManager.OnEscortRescued()
```

Escort 從場上移除或標記 Completed。

Portal Placement：

Goal Area 視為 Invalid。

---

# 27. Turret

Turret Data：

```text
fireDirection
fireInterval
projectilePrefab
projectileSpeed
projectileSize
```

Turret：

- 固定位置。
- 不追蹤 Escort。
- 不旋轉。

---

# 28. Turret Fire Loop

Prototype：

```text
Wait fireInterval
↓
Spawn Projectile
↓
Repeat
```

第一發：

> 等待一次 fireInterval 後再發射。

---

# 29. Projectile

Projectile Data：

```text
direction
speed
size
```

Projectile：

持續：

```text
position += direction * speed * deltaTime
```

---

# 30. Projectile Collision

Projectile 接觸 Escort：

```text
Escort.Die()
Destroy Projectile
```

Projectile 接觸 Obstacle：

```text
Destroy Projectile
```

Projectile 離開 Boundary：

```text
Destroy()
```

---

# 31. Projectile + Portal

Projectile Layer 不被 Portal Trigger 處理。

PortalEndpoint：

只接受 EscortTarget Layer。

---

# 32. Turret Collision

Turret 本體 Collider：

Escort 接觸：

```text
Escort.Die()
```

Portal Placement：

Turret Collider 視為 Invalid。

---

# 33. Boundary

Boundary 可以使用：

- BoxCollider2D
- EdgeCollider2D

Escort：

碰到 Boundary：

```text
Die()
```

Projectile：

碰到 Boundary：

```text
Destroy()
```

---

# 34. GameManager

負責：

```text
TotalEscortCount
SpawnedCount
AliveCount
RescuedCount
DeadCount
GameState
```

GameState：

```text
Playing
Clear
Fail
```

---

# 35. Clear Condition

當：

```text
RescuedCount == TotalEscortCount
```

而且：

```text
DeadCount == 0
```

→ Clear。

---

# 36. Fail Condition

任何 Escort：

```text
Die()
```

→ Prototype 立即：

```text
GameState = Fail
```

停止或凍結遊戲流程。

---

# 37. Retry

Retry：

> Reload Current Scene。

Prototype 不建立額外 Save / Level Select。

---

# 38. Inspector-driven Data

以下資料不得硬編碼於主要 gameplay logic：

- Escort Spawn Count
- Spawn Interval
- Move Speed
- Start Direction
- Portal Max Distance
- Reconfiguration Cooldown
- Floor Speed Multiplier
- Floor Turn Direction
- Turret Fire Direction
- Fire Interval
- Projectile Speed
- Projectile Size

Prototype 應可直接由 Inspector 修改。

---

# 39. Suggested Layers

建議：

```text
Ground
Escort
Portal
Obstacle
Hazard
Goal
Turret
Projectile
Boundary
FloorEffect
```

Portal Placement Validation 應使用 LayerMask。

---

# 40. Debug Requirements

Prototype 建議保留以下 Debug Visual：

- Portal Max Range
- Entrance Direction Arrow
- Exit Direction Arrow
- Spawn Direction
- Turret Fire Direction
- Collider Gizmos

優先確保開發時可以快速找問題。

---

# 41. Technical Non-goals

不要實作：

- NavMesh
- Pathfinding
- ECS
- Complex State Framework
- Dependency Injection Framework
- Portal Rendering Camera
- Physics Momentum Transfer
- Generic Ability System
- Object Pooling unless performance requires it
- Save System
- Networking

Prototype 目標是：

> 快速做出穩定、容易修改的核心玩法。
