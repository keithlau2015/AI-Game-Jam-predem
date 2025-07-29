using DG.Tweening;
using UnityEngine;

[ExecuteInEditMode]
public class Tweener_Position : TweenerBase<Vector3>
{
    protected override void Awake()
    {
        if (transform == null)
            return;
        base.SetUp(DOTween.To(() => transform.localPosition, x => transform.localPosition = x, to, duration));
        base.Awake();
    }
}
