using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using UnityEngine;
/// <summary>
/// Dependency:
/// Generic UI Module
/// </summary>
public class CountDownController : Singleton<CountDownController>
{
    private event Action onStart;
    public event Action<int> onCounting;
    public event Action onCompleted;

    private bool isPause;
    private int seconds;
    private CountDownPanel countDownPanel;

    public async void StartCount(int seconds)
    {
        if (seconds <= 0) return;
        this.seconds = seconds;
        if(countDownPanel == null)
        {
            countDownPanel = await UIManager.singleton.LoadUI<CountDownPanel>(typeof(CountDownPanel).Name);
        }
        countDownPanel.SetLabel(seconds.ToString());
        onStart?.Invoke();
        isPause = false;

        StartCoroutine(Counting());
    }

    public void StopCounting()
    {
        StopCoroutine(Counting());
        isPause = true;
    }

    public void PauseCounting()
    {
        isPause = true;
    }

    public void ResumeCounting()
    {
        isPause = false;
    }

    public void ResetCounter()
    {
        onStart = null;
        onCounting = null;
        onCompleted = null;
        this.seconds = 0;
        this.isPause = true;
    }

    private IEnumerator Counting()
    {
        for (int i = seconds; i > 0; i--)
        {
            if (isPause)
                yield return new WaitUntil(() => { return !isPause; });
            yield return new WaitForSeconds(i);
            countDownPanel.SetLabel(i.ToString());
            onCounting.Invoke(i);
        }
        onCompleted?.Invoke();
    }
}