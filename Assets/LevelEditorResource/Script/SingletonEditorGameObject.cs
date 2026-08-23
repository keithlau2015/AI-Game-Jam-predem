#if UNITY_EDITOR
using UnityEngine;
[ExecuteInEditMode]
public class SingletonEditorGameObject : MonoBehaviour
{
    private void Awake()
    {
        var segos = GameObject.FindObjectsOfType<SingletonEditorGameObject>();
        if (segos != null && segos.Length > 1)
        {
            foreach (var sego in segos) {
                if (!sego.Equals(this))
                {
                    DestroyImmediate(sego.gameObject);
                }
            }
        }
    }
}
#endif