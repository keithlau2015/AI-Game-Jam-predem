using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Collects nested hitbox children for damage / interaction hierarchies.
/// </summary>
public class HitboxController : MonoBehaviour
{
    [SerializeField]
    private string hitboxId;

    [SerializeField]
    private bool isAutoInit = true;

    [SerializeField]
    private List<HitboxController> childHitboxes = new List<HitboxController>();

    public string HitboxId => hitboxId;
    public IReadOnlyList<HitboxController> ChildHitboxes => childHitboxes;

    private void OnEnable()
    {
        if (isAutoInit)
            Init();
    }

    public void Init()
    {
        childHitboxes = GetComponentsInChildren<HitboxController>(true)
            .Where(h => h != null && h != this)
            .ToList();
    }

    public HitboxController FindChild(string id)
    {
        if (string.IsNullOrEmpty(id))
            return null;

        for (int i = 0; i < childHitboxes.Count; i++)
        {
            if (childHitboxes[i] != null && childHitboxes[i].hitboxId == id)
                return childHitboxes[i];
        }

        return null;
    }
}
