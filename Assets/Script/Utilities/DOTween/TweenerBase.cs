using DG.Tweening;
using System;
using UnityEngine;

public abstract class TweenerBase<T> : MonoBehaviour
{
    [SerializeField]
    protected bool playWhenOnEnable;
    [SerializeField]
    protected bool playWhenOnAwake;
    [SerializeField]
    private LoopType loopType;
    [Tooltip("-1 => loop infinitely")]
    [SerializeField]
    private int loop = -1;
    [SerializeField]
    protected float duration;
    [SerializeField]
    protected float speed;
    [SerializeField]
    protected float delay;
    [SerializeField]
    protected T from;
    [SerializeField]
    protected T to;
    [SerializeField]
    protected AnimationCurve animationCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));

    protected Action onPlayCB;
    protected Action onCompleteCB;
    protected bool isInit = false;
    protected TweenParams tweenParams = new TweenParams();
    protected Tweener tweener;

    public bool isTweening { get; private set; } = false;

    public virtual void Play()
    {
        tweener.Play();
    }

    public virtual void Stop()
    {
        tweener.Pause();
    }

    public virtual void SetUp(Tweener tweener, Action onPlayCB = null, Action onCompleteCB = null)
    {
        if (tweener == null)
            return;

        this.tweener = tweener;
        this.onPlayCB = onPlayCB;
        this.onCompleteCB = onCompleteCB;
        SetConfig();
        isInit = true;
    }

    public void SetTween(T from = default, T to = default)
    {
        this.to = to;
        this.from = from;
        SetConfig();
    }

    public void SetOnPlayCB(Action onPlayCB)
    {
        this.onPlayCB = onPlayCB;
        SetConfig();
    }

    public void SetOnCompleteCB(Action onCompleteCB)
    {
        this.onCompleteCB = onCompleteCB;
        SetConfig();
    }

    public void ClearOnPlayCB()
    {
        this.onPlayCB = null;
        SetConfig();
    }

    public void ClearOnCompleteCB()
    {
        this.onCompleteCB = null;
        SetConfig();
    }

    public void SetLoop(int loop, LoopType loopType)
    {
        this.loop = loop;
        this.loopType = loopType;
        SetConfig();
    }

    public void SetDelay(float delay)
    {
        this.delay = delay;
        SetConfig();
    }

    private void SetConfig()
    {
        if (this.tweener == null)
            return;

        this.tweenParams.SetLoops(loop, loopType);
        this.tweenParams.SetEase(animationCurve);
        this.tweenParams.OnPlay(() => {
            isTweening = true;
            onPlayCB?.Invoke(); 
        });
        this.tweenParams.OnComplete(() => {
            isTweening = false;
            onCompleteCB?.Invoke(); 
        });
        this.tweenParams.SetDelay(delay);
        this.tweenParams.SetAutoKill(false);
        if(tweener != null)
        {
            this.tweener.SetAs(tweenParams);
            if (from != null)
                this.tweener.ChangeStartValue(from);
            if (to != null)
                this.tweener.ChangeEndValue(to);
            this.tweener.Pause();
        }
    }

    protected virtual void Awake()
    {
        if(this.playWhenOnAwake && isInit)
        {
            tweener.Play();
        }
    }

    protected virtual void OnEnable()
    {
        if (this.playWhenOnEnable && isInit)
        {
            tweener.Play();
        }
    }

    protected virtual void OnDisable()
    {
        if (tweener == null || this.gameObject == null)
            return;

        tweener.TogglePause();
        tweener.From();
    }
}
