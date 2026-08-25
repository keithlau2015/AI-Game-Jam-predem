# GameDesign.md — Portal Escort

## 1. Project Overview

### Game Name
**Portal Escort**

### Genre
2D Top-down Real-time Route Puzzle / Escort Puzzle

### Theme
Dungeon

### Prototype Goal
玩家不能直接控制護送對象。

護送對象出生後只會沿目前方向持續前進。玩家需要利用傳送門改變護送對象的位置與移動方向，配合特殊地板、障礙物與炮台攻擊，把所有護送對象安全送到 Goal。

Prototype 優先驗證：

> 「自動前進角色 + 即時配置 Portal + 路線與時序預判」是否具備可玩性。

---

# 2. Core Gameplay

護送對象會：

- 自動前進。
- 不自行停止。
- 不自行避障。
- 不主動轉向。
- 不接受玩家直接移動操作。

玩家主要控制：

> Portal Entrance 與 Portal Exit。

Portal 可以：

- 改變護送對象位置。
- 改變護送對象方向。
- 跳過牆壁。
- 避開危險。
- 繞過炮台。
- 將護送對象送到特殊地板。

---

# 3. Core Loop

1. 關卡開始。
2. 護送對象依 Spawn 設定生成。
3. 護送對象沿 Start Direction 自動移動。
4. 玩家觀察前方路線與危險。
5. 玩家放置 Entrance Portal。
6. 玩家選擇 Entrance Direction。
7. 系統顯示 Exit 最大可放置範圍。
8. 玩家放置 Exit Portal。
9. 玩家選擇 Exit Direction。
10. Portal Pair 啟用。
11. 護送對象進入有效 Entrance。
12. 護送對象瞬間移動到 Exit。
13. 護送對象改為沿 Exit Direction 移動。
14. Portal 進入 Reconfiguration Cooldown。
15. 玩家觀察後續路線並準備下一次配置。
16. 護送對象抵達 Goal 或死亡。
17. 判定 Clear / Fail。

---

# 4. Escort Target

## 4.1 Movement

護送對象：

- 使用連續移動。
- 不採用格子逐格移動。
- 不使用 NavMesh。
- 只允許四方向。

Direction：

- Up
- Right
- Down
- Left

Prototype 基本速度：

**2 world units / second**

---

## 4.2 Spawn Data

每個 Spawn Point 至少包含：

- Spawn Position
- Spawn Count
- Spawn Interval
- Move Speed
- Start Direction

Prototype Example：

- Spawn Position = (-7, 2)
- Spawn Count = 3
- Spawn Interval = 1 sec
- Move Speed = 2
- Start Direction = Right

---

# 5. Escort Target Death

護送對象接觸以下任一內容時立即死亡：

- Obstacle
- Hazard
- Turret
- Projectile
- Map Boundary
- Outside Playable Area
- Other Lethal Object

Prototype 不設：

- HP
- Damage Value
- Knockback
- Invincibility

所有有效致死碰撞均為：

> Instant Death

---

# 6. Goal

Goal 為護送目的地。

護送對象進入 Goal：

> Rescue Success

Portal 不可以放置在 Goal 範圍。

Prototype 中：

> 所有護送對象成功抵達 Goal 才 Clear。

任何一名護送對象死亡：

> Fail。

---

# 7. Portal System

每組 Portal Pair 包含：

## Entrance
- Position
- Direction

## Exit
- Position
- Direction

Portal 完成配對後：

> 持續存在並保持 Active。

多個護送對象可以連續使用同一組 Portal。

---

# 8. Portal Placement

Portal 只能放置於：

> 沒有阻礙物的合法地面。

Portal 不可放置於：

- Obstacle
- Hazard
- Turret
- Goal
- Existing Portal
- Escort Target Current Position
- Non-ground Area

---

# 9. Portal Placement Flow

## Step 1
玩家左鍵選擇 Entrance Position。

## Step 2
玩家設定 Entrance Direction。

只允許：

- Up
- Right
- Down
- Left

左鍵確認。

## Step 3
系統顯示 Exit 有效距離。

## Step 4
玩家在合法地點放置 Exit。

## Step 5
玩家設定 Exit Direction。

左鍵確認。

## Step 6
Portal Pair 成為 Active。

---

# 10. Incomplete Portal Rule

如果只有 Entrance 尚未完成 Exit：

> Portal 不具有傳送功能。

護送對象經過未完成 Portal：

- 不傳送。
- 不轉向。
- 不死亡。
- 正常穿過。

只有完整 Portal Pair：

> Active

才可以傳送。

---

# 11. Portal Direction

Portal Direction 統一代表：

> 護送對象有效通過 Portal 時的移動方向。

## Entrance Direction

只有：

Escort Move Direction == Entrance Direction

時可以觸發。

例如：

Character Right  
Entrance Right

→ 可以傳送。

Character Left  
Entrance Right

→ 不傳送。

---

## Exit Direction

護送對象成功傳送後：

> Move Direction = Exit Direction

立即依新方向前進。

---

# 12. Portal Distance

Prototype：

**Max Portal Distance = 6 world units**

計算：

> Entrance 與 Exit 的世界直線距離。

不計：

- 格數
- 道路長度
- 尋路距離

牆壁不會阻止 Portal 配對。

只要 Entrance / Exit 都是合法位置，而且距離 ≤ 6：

> 可以跨牆建立 Portal。

---

# 13. Portal Persistent

Portal Pair 成功配置後：

> 持續存在。

角色使用後 Portal 不會消失。

同一 Portal 可以被多個護送對象連續使用。

---

# 14. Reconfiguration Cooldown

Prototype：

**3 seconds**

Cooldown 從 Portal Pair 完成配置後開始。

Cooldown 期間：

Portal：

- 保持 Active。
- 可以正常傳送。

玩家不能：

- 修改 Entrance。
- 修改 Exit。
- 修改方向。
- 建立新的 Portal Pair。

Cooldown 結束：

> 玩家可以重新配置 Portal。

如果玩家沒有重新配置：

> 原 Portal 繼續正常使用。

---

# 15. Floor Effects

地圖可配置特殊地板。

Effect Type：

- Normal
- SpeedUp
- SlowDown
- Turn

---

# 16. Speed Up Floor

護送對象進入後：

> Current Speed = Base Speed × Speed Multiplier

Prototype Example：

**Multiplier = 1.5**

Base Speed = 2

Result：

**3 units/sec**

離開地板：

> 回復 Base Speed。

---

# 17. Slow Floor

Prototype Example：

**Multiplier = 0.5**

Base Speed = 2

Result：

**1 unit/sec**

離開地板：

> 回復 Base Speed。

Slow Floor 可用於：

- 爭取 Portal Cooldown 時間。
- 改變護送對象間距。
- 配合炮台射擊節奏。

---

# 18. Turn Floor

Turn Floor 指定一個方向：

- Up
- Right
- Down
- Left

護送對象踩中後：

> Move Direction 立即改為指定 Direction。

---

# 19. Portal + Floor Interaction

護送對象由 Exit 出現後：

如果 Exit 所在位置具有 Floor Effect：

> 立即受到 Floor Effect。

處理順序：

1. Teleport 到 Exit。
2. Move Direction 設為 Exit Direction。
3. 判定 Exit 所在 Floor Effect。
4. 套用 Floor Effect。
5. 繼續移動。

---

# 20. Obstacle

Obstacle Data：

- Position
- Size

護送對象碰到：

> Death

Portal 不可以放在 Obstacle 上。

---

# 21. Turret

Turret 為固定位置動態危險。

Turret 週期性沿固定方向發射 Projectile。

Turret Data：

- Position
- Fire Direction
- Fire Interval
- Projectile Size
- Projectile Speed

方向：

- Up
- Right
- Down
- Left

護送對象碰到 Turret 本體：

> Death

---

# 22. Projectile

Projectile：

- 由 Turret 前方生成。
- 沿 Fire Direction 直線移動。
- 不追蹤。
- 不轉向。
- 不反彈。

護送對象碰到 Projectile：

> Death

Projectile 同時消失。

Projectile 碰到：

- Obstacle
- Boundary

→ Destroy

---

# 23. Portal + Projectile

Prototype 中：

> Projectile 不受 Portal 影響。

Projectile 經過 Portal：

> 正常穿過。

Portal 只處理 Escort Target。

---

# 24. Level Data

每個 Level 至少需要以下資料。

## Escort Spawn
- Position
- Count
- Interval
- Speed
- Start Direction

## Obstacles
- Position
- Size

## Goal
- Position
- Size

## Floor Effects
- Position
- Size
- Effect Type
- Effect Value
- Direction

## Turrets
- Position
- Fire Direction
- Fire Interval
- Projectile Size
- Projectile Speed

## Portal Rules
- Max Portal Distance = 6
- Reconfiguration Cooldown = 3 sec

---

# 25. Player Decision

核心決策：

### Position
Portal 放在哪裡？

### Direction
角色要從哪個方向進入，以及出去後往哪裡走？

### Timing
什麼時候應該配置或重設 Portal？

### Route
應該走哪一條安全路線？

### Floor Effect
應否利用加速、減速或轉向地板？

### Threat Prediction
角色抵達炮台射線時，Projectile 會在哪裡？

---

# 26. Prototype Scope

## Must Have
- Escort Spawn
- Auto Movement
- Four Directions
- Obstacles
- Goal
- Portal Placement
- Entrance Direction
- Exit Direction
- Instant Teleport
- Max Distance
- Reconfiguration Cooldown
- Speed Up Floor
- Slow Floor
- Turn Floor
- Turret
- Projectile
- Death
- Clear
- Fail
- Retry

## Out of Scope
- HP
- NavMesh
- Character AI
- Portal Capacity
- A/B Groups
- Projectile Portal
- Character Skills
- Equipment
- Shop
- Upgrade
- Complex Physics
- Portal Camera
- Momentum Preservation

---

# 27. Prototype Success Criteria

Prototype 應驗證：

- 玩家理解角色只會向前。
- 玩家理解 Entrance Direction。
- 玩家理解 Exit Direction。
- Portal Distance 會影響路線選擇。
- Cooldown 會令玩家提前規劃。
- Floor Effect 會改變路線與時間判斷。
- Turret 會帶來時序預判。
- Persistent Portal 適合多個護送對象使用。
- 失敗原因容易理解。
- 玩家願意失敗後立即重新嘗試。
