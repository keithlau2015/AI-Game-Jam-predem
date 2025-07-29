using DG.Tweening;
using UnityEngine;

[ExecuteInEditMode]
public class Tweener_AnchorPosition : TweenerBase<Vector2>
{
    [SerializeField]
    private RectTransform rectTransform;

    protected override void Awake()
    {
        if (rectTransform == null)
            TryGetComponent(out rectTransform);
        if (rectTransform == null)
            return;
        base.SetUp(DOTween.To(() => rectTransform.anchoredPosition, x => rectTransform.anchoredPosition = x, to, duration));
        base.Awake();
    }

    public void SetRectTransform(Vector2 value)
    {
        rectTransform.anchoredPosition = value;
    }
}