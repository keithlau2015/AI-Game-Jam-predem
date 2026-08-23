#if UNITY_EDITOR
using UnityEngine;
[ExecuteInEditMode]
public class RestrictedPosition : MonoBehaviour
{

    private void Update()
    {
        this.transform.position = new Vector3(this.transform.position.x, 0, this.transform.position.z);
    }
}
#endif