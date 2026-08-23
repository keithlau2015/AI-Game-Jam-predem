#if UNITY_EDITOR
using UnityEngine;

public class MarkConnector : MonoBehaviour
{
    public Transform[] connectedMarker;

    void OnDrawGizmos()
    {
        foreach (Transform marker in connectedMarker)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, marker.position);
        }
    }
}
#endif