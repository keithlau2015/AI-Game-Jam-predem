using DG.Tweening;
using UnityEngine;

[ExecuteInEditMode]
public class Tweener_Rotation : TweenerBase<Vector3>
{
    protected override void Awake()
    {
        if (transform == null)
            return;
        base.SetUp(DOTween.To(() => transform.rotation, x => transform.rotation = x, to, duration));
        base.Awake();
    }
}
