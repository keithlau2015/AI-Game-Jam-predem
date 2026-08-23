using UnityEngine;

namespace PortalEscort.Debug
{
    /// <summary>
    /// Draws a wire-circle showing the maximum portal placement range around this object.
    /// Radius is read from PortalPairController.maxPortalDistance when present (Contracts §4),
    /// otherwise falls back to the local inspector value.
    /// </summary>
    public class PortalRangeGizmo : MonoBehaviour
    {
        [SerializeField] private float maxPortalDistance = 6f;
        [SerializeField] private Color gizmoColor = Color.cyan;

        private void OnDrawGizmos()
        {
            float radius = maxPortalDistance;
            PortalPairController pair = GetComponent<PortalPairController>();
            if (pair != null)
                radius = pair.maxPortalDistance;

            Gizmos.color = gizmoColor;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}
