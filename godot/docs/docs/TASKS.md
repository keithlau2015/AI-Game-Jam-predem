# TASKS.md — Portal Escort

# Development Goal

完成一個可從：

> Start → Play → Clear / Fail → Retry

完整運行的 Portal Escort Prototype。

開發原則：

> 每完成一個 Phase，先在 Unity 實際 Play Test，再進入下一階段。

不要一次實作所有系統。

---

# P0 — Project Baseline

- [ ] 確認 Unity 專案可以正常開啟及 Play。
- [ ] 建立 Prototype Scene。
- [ ] 設定 2D Top-down Camera。
- [ ] 建立 Ground。
- [ ] 建立基本 Dungeon 測試場景。
- [ ] 建立必要 Layer / Tag。
- [ ] 建立 `Direction` Enum。
- [ ] 建立 `DirectionUtility`。

## Acceptance

- [ ] Scene 可以正常運行。
- [ ] Camera 可以完整看到測試區域。
- [ ] Direction 可以正確轉換為 Up / Right / Down / Left Vector。

---

# P1 — Escort Basic Movement

- [ ] 建立 `EscortTarget.cs`。
- [ ] Escort 自動向目前方向移動。
- [ ] Move Speed 可由 Inspector 修改。
- [ ] Start Direction 可設定。
- [ ] Prototype Default Speed = 2 units/sec。
- [ ] 角色只使用四方向。
- [ ] 不加入 NavMesh。
- [ ] 不加入玩家直接控制。

## Acceptance

- [ ] Escort Spawn 後不用任何操作會持續前進。
- [ ] Up / Right / Down / Left 四方向皆正確。
- [ ] 修改 Inspector Speed 後立即影響移速。

---

# P2 — Escort Spawner

- [ ] 建立 `EscortSpawner.cs`。
- [ ] 支援 Spawn Position。
- [ ] 支援 Spawn Count。
- [ ] 支援 Spawn Interval。
- [ ] 支援 Move Speed。
- [ ] 支援 Start Direction。
- [ ] 支援連續生成多個 Escort。

## Acceptance

設定：

```text
Count = 3
Interval = 1
```

- [ ] 能每隔約 1 秒生成一名 Escort。
- [ ] 共生成 3 名。
- [ ] 所有 Escort 正確使用 Start Direction。

---

# P3 — Obstacle / Death / Goal

- [ ] 建立 Obstacle Collider。
- [ ] Escort 撞 Obstacle 即死。
- [ ] 建立 Boundary。
- [ ] Escort 撞 Boundary 即死。
- [ ] 建立 Goal。
- [ ] Escort 進入 Goal 計算 Rescue。
- [ ] 建立 GameManager。
- [ ] 建立 Clear。
- [ ] 建立 Fail。
- [ ] 建立 Retry。

## Acceptance

- [ ] Escort 撞牆 → Fail。
- [ ] Escort 出界 → Fail。
- [ ] Escort 到 Goal → Rescue。
- [ ] 所有 Escort 到 Goal → Clear。
- [ ] 任一 Escort 死亡 → Fail。
- [ ] Retry 可以重新開始 Scene。

---

# P4 — Entrance Placement

- [ ] 建立 `PortalPlacementController.cs`。
- [ ] 滑鼠 World Position 判定。
- [ ] 顯示 Entrance Preview。
- [ ] 只允許 Ground。
- [ ] Obstacle 上不可放置。
- [ ] Goal 上不可放置。
- [ ] Escort 腳下不可放置。
- [ ] Turret 上不可放置。
- [ ] Existing Portal 上不可放置。
- [ ] 左鍵確認 Entrance Position。

## Acceptance

- [ ] 合法地面可以放 Entrance。
- [ ] 牆上不能放。
- [ ] Escort 經過時該位置不能放。
- [ ] Goal 上不能放。

---

# P5 — Entrance Direction

- [ ] Entrance 建立後進入 Direction Selection。
- [ ] 根據滑鼠位置選擇四方向。
- [ ] 顯示 Direction Arrow。
- [ ] 左鍵確認 Entrance Direction。

## Acceptance

- [ ] 只能選 Up / Right / Down / Left。
- [ ] 不會產生任意角度。
- [ ] Arrow 與實際 Direction 一致。

---

# P6 — Exit Placement

- [ ] Entrance 確認後顯示 Max Range。
- [ ] Default Max Range = 6 world units。
- [ ] Exit 只能在距離 ≤ 6 的位置。
- [ ] Exit 使用與 Entrance 相同 Placement Validation。
- [ ] 顯示 Exit Preview。
- [ ] 超距離位置顯示 Invalid。
- [ ] 左鍵確認 Exit Position。

## Acceptance

- [ ] 5.9 units 可以放。
- [ ] 6.1 units 不可以放。
- [ ] 中間有牆仍可建立 Portal Pair。
- [ ] Exit 本身不能放在牆上。

---

# P7 — Exit Direction

- [ ] Exit 建立後進入 Direction Selection。
- [ ] 只允許四方向。
- [ ] 顯示 Exit Direction Arrow。
- [ ] 左鍵確認。
- [ ] 完成後 Portal Pair 進入 Active。

## Acceptance

- [ ] Portal Pair 完成前不可傳送。
- [ ] Portal Pair 完成後可以觸發。

---

# P8 — Portal Teleport

- [ ] 建立 `PortalPairController.cs`。
- [ ] 建立 `PortalEndpoint.cs`。
- [ ] Entrance 檢查 Escort Direction。
- [ ] Direction 相同才可以 Teleport。
- [ ] Direction 不同直接 Ignore。
- [ ] Teleport 為瞬間完成。
- [ ] Escort 出現在 Exit。
- [ ] Escort Direction 改成 Exit Direction。
- [ ] 加入 Exit Safe Offset。
- [ ] 加入短暫 Teleport Lock。

## Acceptance

### Case 1

```text
Escort →
Entrance →
```

- [ ] 成功傳送。

### Case 2

```text
Escort ←
Entrance →
```

- [ ] 不傳送。

### Case 3

Exit Direction = Down

- [ ] Escort 傳送後立即向下移動。

### Case 4

只有 Entrance 未完成 Exit

- [ ] Escort 正常穿過，不傳送。

---

# P9 — Portal Persistent

- [ ] Escort 使用 Portal 後 Portal 不消失。
- [ ] 多個 Escort 可連續使用同一 Pair。

## Acceptance

- [ ] Escort A 成功通過。
- [ ] Portal 仍存在。
- [ ] Escort B 隨後亦可通過。

---

# P10 — Reconfiguration Cooldown

- [ ] Portal 完成後啟動 3 秒 Cooldown。
- [ ] Cooldown 期間 Portal 保持 Active。
- [ ] Cooldown 期間 Escort 仍可傳送。
- [ ] Cooldown 期間玩家不能重設。
- [ ] Cooldown 結束後進入 Reconfigurable。
- [ ] 玩家可重新建立 Portal Pair。
- [ ] 新 Pair 完成後重新開始 3 秒 Cooldown。

## Acceptance

- [ ] Cooldown = 3 秒。
- [ ] CD 期間傳送功能正常。
- [ ] CD 期間不能修改 Portal。
- [ ] CD 完結可修改。

---

# P11 — Speed Up Floor

- [ ] 建立 `FloorEffect.cs`。
- [ ] 建立 FloorEffectType。
- [ ] SpeedUp 使用 Multiplier。
- [ ] Escort 進入後更新 Current Speed。
- [ ] Escort 離開後回復 Base Speed。

## Acceptance

Base Speed = 2  
Multiplier = 1.5

- [ ] 地板上速度 = 3。
- [ ] 離開後速度 = 2。

---

# P12 — Slow Floor

- [ ] SlowDown 使用 Multiplier。
- [ ] Escort 進入後減速。
- [ ] 離開後回復 Base Speed。

## Acceptance

Base Speed = 2  
Multiplier = 0.5

- [ ] 地板上速度 = 1。
- [ ] 離開後速度 = 2。

---

# P13 — Turn Floor

- [ ] Turn Floor 可指定四方向。
- [ ] Escort 進入時立即改變 Direction。

## Acceptance

Escort Right  
Turn Floor Down

- [ ] Escort 踩中後立即改成 Down。

---

# P14 — Portal + Floor Interaction

- [ ] Escort Teleport 到 Exit 後檢查 Floor Effect。
- [ ] Exit Direction 先套用。
- [ ] Floor Effect 隨後套用。

## Acceptance

Exit Direction = Down  
Exit 位於 Turn Right Floor

- [ ] Escort 最終 Direction = Right。

---

# P15 — Turret

- [ ] 建立 `Turret.cs`。
- [ ] Turret Position 可調。
- [ ] Fire Direction 可調。
- [ ] Fire Interval 可調。
- [ ] 關卡開始後等待一個 Interval。
- [ ] 定時 Spawn Projectile。
- [ ] Turret 本體為 Lethal。

## Acceptance

Fire Interval = 2 sec

- [ ] 約每 2 秒發射一發。
- [ ] 射擊方向正確。
- [ ] Escort 撞 Turret → Fail。

---

# P16 — Projectile

- [ ] 建立 `Projectile.cs`。
- [ ] Projectile Size 可調。
- [ ] Projectile Speed 可調。
- [ ] 沿指定方向直線移動。
- [ ] 撞 Escort → Escort Death。
- [ ] 撞 Escort 後 Projectile Destroy。
- [ ] 撞 Obstacle → Destroy。
- [ ] 撞 Boundary → Destroy。
- [ ] Projectile 不受 Portal 影響。

## Acceptance

- [ ] Projectile 方向正確。
- [ ] Speed 修改有效。
- [ ] Escort 碰 Projectile → Fail。
- [ ] Projectile 穿 Portal 不傳送。

---

# P17 — Prototype UI

- [ ] 顯示 Portal Placement State。
- [ ] 顯示 Entrance / Exit 操作提示。
- [ ] 顯示 Reconfiguration Cooldown。
- [ ] 顯示 Clear。
- [ ] 顯示 Fail。
- [ ] 顯示 Retry。
- [ ] 顯示 Rescued / Total。

---

# P18 — Debug Visualization

- [ ] Portal Range Gizmo。
- [ ] Entrance Direction Arrow。
- [ ] Exit Direction Arrow。
- [ ] Spawn Direction。
- [ ] Turret Fire Direction。
- [ ] Invalid Placement Preview。

---

# P19 — First Playable Level

建立一個完整測試 Level。

至少包含：

- [ ] 1 Escort Spawn。
- [ ] 3 Escorts。
- [ ] 1+ Obstacle。
- [ ] 1 Goal。
- [ ] 1 Slow Floor。
- [ ] 1 Turn Floor。
- [ ] 1 Turret。
- [ ] Portal Max Distance = 6。
- [ ] Reconfiguration Cooldown = 3。

## Level Acceptance

玩家必須至少使用：

- [ ] Portal Teleport。
- [ ] Portal Redirect。
- [ ] 一次特殊地板。
- [ ] 避開一次 Turret / Projectile。

才能全部 Escort 到 Goal。

---

# P20 — Final Prototype Check

- [ ] Start → Play 正常。
- [ ] Clear 正常。
- [ ] Fail 正常。
- [ ] Retry 正常。
- [ ] 沒有明顯 NullReferenceException。
- [ ] 未完成 Portal 不會 Teleport。
- [ ] Portal 不會無限 Trigger。
- [ ] Escort 不會卡在 Exit。
- [ ] 多 Escort 可以穩定連續 Portal。
- [ ] Projectile 不會誤入 Portal。
- [ ] 所有核心數值可由 Inspector 修改。
- [ ] Build 後核心玩法正常。

---

# Do Not Implement Yet

除非以上 P0–P20 已完成並穩定，否則不要新增：

- [ ] A/B Portal Groups
- [ ] Portal Capacity
- [ ] Projectile Teleport
- [ ] HP
- [ ] Character Skill
- [ ] Enemy AI
- [ ] NavMesh
- [ ] Upgrade
- [ ] Shop
- [ ] Equipment
- [ ] Multiple Portal Pairs
- [ ] Portal Rendering Camera
- [ ] Momentum Physics

Prototype 的成功標準不是功能數量，而是：

> **核心 Escort + Portal + Floor Effect + Turret Loop 可以完整、穩定地玩一局。**
