using UnityEngine;

public class MiniMapCam : MonoBehaviour
{
    private GameObject followTarget;

    public void SetFollowTarget(GameObject followTarget)
    {
        this.followTarget = followTarget;
    }

    private void Update()
    {
        if (followTarget == null) return;

        this.transform.position = new Vector3(followTarget.transform.position.x, this.transform.position.y, followTarget.transform.position.z);
    }
}