using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeBaseObserable : EvtObserable
{
    [SerializeField]
    private float interval;

    [SerializeField]
    private float delay;

    [SerializeField]
    private float maxTriggerCount;

    [SerializeField]
    private float triggerInterval;

    private float elapsedTime;
    private float triggerCount;
    private Coroutine coroutine;

    private void Awake()
    {
        coroutine = StartCoroutine(createCoroutine());
    }

    private IEnumerator createCoroutine()
    {
        yield return new WaitForSeconds(delay);

        while (triggerCount < maxTriggerCount)
        {
            yield return new WaitForSeconds(interval);
            elapsedTime += interval;

            if (elapsedTime >= triggerInterval)
            {
                triggerCount++;
                elapsedTime = 0;
                Notify();
            }
        }

        StopCoroutine(coroutine);
        coroutine = null;
        yield break;
    }

    private void OnDestroy()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
    }
}