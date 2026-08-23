using DG.Tweening;
using UnityEngine;

[ExecuteInEditMode]
public class Tweener_Alpha : TweenerBase<float>
{
    [SerializeField]
    private CanvasGroup canvasGroup;

    protected override void Awake()
    {
        if (canvasGroup == null)
            TryGetComponent(out canvasGroup);
        if (canvasGroup == null)
            return;
        if (!isInit)
            base.SetUp(DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, to, duration));
        base.Awake();
    }

    protected override void OnEnable()
    {
        if (canvasGroup == null)
            TryGetComponent(out canvasGroup);
        if (canvasGroup == null)
            return;
        if (!isInit)
            base.SetUp(DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, to, duration));
        base.OnEnable();
    }

    public void SetCanvasGroupAlpha(float alpha)
    {
        this.canvasGroup.alpha = alpha;
    }
}
