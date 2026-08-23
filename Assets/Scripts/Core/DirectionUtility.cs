using UnityEngine;

public enum Direction { Up, Right, Down, Left }

public static class DirectionUtility
{
    public static Vector2 GetVector(Direction d)
    {
        switch (d)
        {
            case Direction.Up:    return new Vector2(0f, 1f);
            case Direction.Right: return new Vector2(1f, 0f);
            case Direction.Down:  return new Vector2(0f, -1f);
            case Direction.Left:  return new Vector2(-1f, 0f);
            default:              return Vector2.zero;
        }
    }

    // Degrees for arrow / visual rotation (0 = Right, CCW positive in Unity).
    public static float GetRotationZ(Direction d)
    {
        switch (d)
        {
            case Direction.Right: return 0f;
            case Direction.Up:    return 90f;
            case Direction.Left:  return 180f;
            case Direction.Down:  return 270f;
            default:              return 0f;
        }
    }

    public static Direction FromDelta(Vector2 delta)
    {
        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            return delta.x >= 0f ? Direction.Right : Direction.Left;
        else
            return delta.y >= 0f ? Direction.Up : Direction.Down;
    }

    public static bool AreSame(Direction a, Direction b)
    {
        return a == b;
    }
}
