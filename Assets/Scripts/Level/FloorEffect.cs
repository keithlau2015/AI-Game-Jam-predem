using UnityEngine;

public enum FloorEffectType { Normal, SpeedUp, SlowDown, Turn }

public class FloorEffect : MonoBehaviour
{
    public FloorEffectType effectType = FloorEffectType.Normal;
    public float speedMultiplier = 1f;
    public Direction turnDirection = Direction.Up;
}
