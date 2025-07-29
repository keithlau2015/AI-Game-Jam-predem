using DG.Tweening;
using UnityEngine;

[ExecuteInEditMode]
public class Tweener_Scale : TweenerBase<Vector3>
{
    protected override void Awake()
    {
        if (transform == null)
            return;
        if(!isInit)
            base.SetUp(DOTween.To(() => this.transform.localScale, x => this.transform.localScale = x, to, duration));
        base.Awake();
    }

    protected override void OnEnable()
    {
        if (transform == null)
            return;
        if (!isInit)
            base.SetUp(DOTween.To(() => this.transform.localScale, x => this.transform.localScale = x, to, duration));
        base.OnEnable();
    }
}
