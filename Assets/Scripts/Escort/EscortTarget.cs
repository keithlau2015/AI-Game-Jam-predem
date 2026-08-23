using UnityEngine;

public class EscortTarget : MonoBehaviour
{
    public float baseMoveSpeed = 2f;
    public float currentMoveSpeed;
    public Direction currentDirection = Direction.Right;
    public bool isAlive = true;

    private float teleportLockTimer = 0f;

    public bool IsTeleportLocked
    {
        get { return teleportLockTimer > 0f; }
    }

    private void Awake()
    {
        currentMoveSpeed = baseMoveSpeed;
    }

    private void Update()
    {
        if (teleportLockTimer > 0f)
            teleportLockTimer = Mathf.Max(0f, teleportLockTimer - Time.deltaTime);

        if (!isAlive)
            return;

        Vector2 dir = DirectionUtility.GetVector(currentDirection);
        transform.position += (Vector3)(dir * currentMoveSpeed * Time.deltaTime);
    }

    public void Die()
    {
        isAlive = false;
    }

    public void TeleportTo(Vector2 exitPosition, Direction exitDirection, float safeOffset)
    {
        Vector2 dir = DirectionUtility.GetVector(exitDirection);
        transform.position = (Vector3)(exitPosition + dir * safeOffset);
        currentDirection = exitDirection;
        teleportLockTimer = 0.15f;
    }

    public void ApplyFloorEffect(FloorEffectType type, float multiplier, Direction turnDir)
    {
        switch (type)
        {
            case FloorEffectType.SpeedUp:
            case FloorEffectType.SlowDown:
                currentMoveSpeed = baseMoveSpeed * multiplier;
                break;
            case FloorEffectType.Turn:
                currentDirection = turnDir;
                break;
            default:
                currentMoveSpeed = baseMoveSpeed;
                break;
        }
    }
}
