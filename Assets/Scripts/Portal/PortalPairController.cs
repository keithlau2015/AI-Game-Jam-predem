using UnityEngine;

public class PortalPairController : MonoBehaviour
{
    public PortalEndpoint entrance;
    public PortalEndpoint exit;
    public float maxPortalDistance = 6f;
    public float reconfigurationCooldown = 3f;
    public float exitOffset = 0.7f;

    public bool IsComplete
    {
        get
        {
            return entrance != null && exit != null
                && entrance.isActive && exit.isActive;
        }
    }

    public void TryTeleport(EscortTarget escort)
    {
        if (escort == null)
            return;
        if (!IsComplete)
            return;
        if (!entrance.isActive)
            return;
        if (escort.IsTeleportLocked)
            return;
        if (!DirectionUtility.AreSame(escort.currentDirection, entrance.direction))
            return;

        escort.TeleportTo(exit.transform.position, exit.direction, exitOffset);
        exit.isActive = true;
    }
}
