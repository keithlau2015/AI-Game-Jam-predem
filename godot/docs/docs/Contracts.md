# Contracts.md — Shared Public APIs

These types, methods, and conventions are the **public contract** between agents.
Treat them as APIs. An agent may not change a contract lightly:

1. Update this file.
2. Update every agent doc that references it.
3. Update callers before merging.

## 1. Direction (shared by ALL agents)

Defined once in `Core/DirectionUtility.cs` (owner: **GameMechanism**).

```csharp
public enum Direction { Up, Right, Down, Left }

public static class DirectionUtility
{
    public static Vector2 GetVector(Direction d);      // Up=(0,1) Right=(1,0) Down=(0,-1) Left=(-1,0)
    public static float GetRotationZ(Direction d);     // degrees, for arrow/visual rotation
    public static Direction FromDelta(Vector2 delta);  // pick nearest 4-dir from mouse delta
    public static bool AreSame(Direction a, Direction b);
}
```

**Rule:** Every agent (Portal, Turret, Floor, Spawner, UI arrows) MUST use this enum and
utility. No agent defines its own direction type or angle math.

## 2. Layers (set in TagManager; owner: **GameMechanism** baseline, **Transition** runtime)

```text
Ground, Escort, Portal, Obstacle, Hazard, Goal, Turret, Projectile, Boundary, FloorEffect
```

`PortalPlacementController` validates placement using a `LayerMask`. Agents adding
colliders must tag the correct layer.

## 3. EscortTarget public surface (owner: **GameMechanism**)

```csharp
public class EscortTarget : MonoBehaviour
{
    public Direction currentDirection;
    public float baseMoveSpeed;     // Inspector
    public float currentMoveSpeed;  // derived (floor effects)
    public bool isAlive;

    public void TeleportTo(Vector2 exitPosition, Direction exitDirection, float safeOffset);
    public void ApplyFloorEffect(FloorEffectType type, float multiplier, Direction turnDir);
    public void Die();
}
```

**Contract notes:**
- `TeleportTo` sets position to `exitPosition + direction*safeOffset`, sets `currentDirection`,
  then releases teleport lock. Callers (Portal) must not move the escort manually.
- Floor effect application order is owned by **GameMechanism** (see TASKS P14).

## 4. PortalEndpoint / PortalPairController (owner: **GameMechanism** + **Transition**)

```csharp
public class PortalEndpoint : MonoBehaviour
{
    public Direction direction;
    public bool isActive;          // false until full pair completes
}

public class PortalPairController : MonoBehaviour
{
    public PortalEndpoint entrance;
    public PortalEndpoint exit;
    public float maxPortalDistance;   // Inspector, default 6
    public float reconfigurationCooldown; // Inspector, default 3
    public bool IsComplete { get; }
    public void TryTeleport(EscortTarget escort); // checks direction match + active
}
```

**Contract:** `TryTeleport` only fires when `isActive && escort.currentDirection == entrance.direction`.
It does NOT teleport projectiles (Projectile layer excluded by design).

## 5. GameManager public surface (owner: **Transition** + **GameMechanism**)

```csharp
public class GameManager : MonoBehaviour
{
    public GameState gameState;   // Playing, Clear, Fail
    public int totalEscortCount;
    public int rescuedCount;
    public int deadCount;

    public void OnEscortRescued(EscortTarget t);
    public void OnEscortDied(EscortTarget t);
    public void RestartLevel();   // Reload scene
}
```

Clear condition: `rescuedCount == totalEscortCount && deadCount == 0`.
Fail condition: any death → `GameState = Fail`.

## 6. FloorEffect public surface (owner: **GameMechanism**)

```csharp
public enum FloorEffectType { Normal, SpeedUp, SlowDown, Turn }

public class FloorEffect : MonoBehaviour
{
    public FloorEffectType effectType;
    public float speedMultiplier;   // Inspector
    public Direction turnDirection; // Inspector (Turn only)
}
```

## 7. Turret / Projectile public surface (owner: **Gameplay**)

```csharp
public class Turret : MonoBehaviour
{
    public Direction fireDirection;
    public float fireInterval;     // Inspector, default 2
    public float projectileSpeed;  // Inspector
    public float projectileSize;   // Inspector
    public GameObject projectilePrefab;
}

public class Projectile : MonoBehaviour
{
    public Direction direction;
    public float speed;
    public float size;
}
```

Projectile ignores Portals by layer. Contact with Escort → `Escort.Die()`.

## 8. Cross-agent events

Use UnityEvents / simple C# events on `GameManager` rather than direct cross-agent calls:
- `OnEscortRescued`, `OnEscortDied`, `OnGameClear`, `OnGameFail`, `OnPortalStateChanged`.

UI/UX agent subscribes to these; it must never call gameplay internals.

## 9. Inspector-driven rule (non-negotiable)

All values in §3–§7 marked "Inspector" must be serialized fields, NOT hardcoded in logic.
This is the prototypability contract for the whole project.

## 10. Naming / file ownership summary

| Type | File | Owning agent |
|------|------|--------------|
| `Direction`, `DirectionUtility` | `Core/DirectionUtility.cs` | GameMechanism |
| `GameManager` | `Core/GameManager.cs` | Transition |
| `EscortTarget` | `Escort/EscortTarget.cs` | GameMechanism |
| `EscortSpawner` | `Escort/EscortSpawner.cs` | Gameplay |
| `PortalPlacementController` | `Portal/PortalPlacementController.cs` | Transition |
| `PortalPairController`, `PortalEndpoint` | `Portal/...` | GameMechanism + Transition |
| `Goal`, `Obstacle`, `Boundary`, `FloorEffect` | `Level/...` | Gameplay + GameMechanism |
| `Turret`, `Projectile` | `Combat/...` | Gameplay |
| `PrototypeUI` | `UI/PrototypeUI.cs` | UI/UX |
| Debug Gizmos / Previews | `Debug/...` | UI/UX + Graphics |
