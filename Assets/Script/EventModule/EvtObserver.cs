using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class EvtObserver : MonoBehaviour
{
    [Serializable]
    private class EvtObserableOption
    {
        public enum  Operator
        {
            AND,
            OR
        }

        public Operator operatorValue;
        public EvtObserable evtObserable;
    }

    [SerializeField]
    private List<EvtObserableOption> observables = new List<EvtObserableOption>();

    [SerializeField]
    private string evtNameId;
    [SerializeField]
    private int maxTriggerCount = -1; // -1 for unlimited

    public string EvtNameId => EvtIdentityValidation.Normalize(evtNameId);
    private EvtRecordData recordData;
    private readonly HashSet<EvtObserable> runtimeSubscribedObservables = new HashSet<EvtObserable>();

    private void Start()
    {
        EvtObserable.OnObservableRegistered += OnObservableRegistered;

        foreach (var observable in observables)
        {
            Subscribe(observable.evtObserable);

            if (observable.evtObserable == null)
                continue;

            string targetId = observable.evtObserable.ID;
            foreach (EvtObserable liveObservable in EvtObserable.GetById(targetId))
            {
                Subscribe(liveObservable);
            }
        }

        if(recordData == null)
        {
            string key = EvtNameId;
            if(!EvtRecordData.mapByEvtName.TryGetValue(key, out recordData))
                recordData = new EvtRecordData(key);
        }
    }

    public void Subscribe(EvtObserable obserable)
    {
        if (obserable == null)
            return;
        if (!runtimeSubscribedObservables.Add(obserable))
            return;
        obserable.Subscribe(this);
    }

    public void Unsubscribe(EvtObserable obserable)
    {
        if (obserable == null)
            return;
        runtimeSubscribedObservables.Remove(obserable);
        obserable.Unsubscribe(this);
    }

    protected abstract void OnExecute(EvtNotifyData notifyData);

    private void Execute(EvtNotifyData notifyData)
    {
        OnExecute(notifyData);
        if(recordData != null) {
            recordData.value++;
        }
        else {
            string key = EvtNameId;
            if(!EvtRecordData.mapByEvtName.TryGetValue(key, out recordData))
                recordData = new EvtRecordData(key);
            recordData.value++;
        }
    }

    public void Notify(EvtNotifyData notifyData = null)
    {
        if (maxTriggerCount == 0 || (maxTriggerCount > 0 && recordData != null && recordData.value >= maxTriggerCount))
            return;

        if (observables.Count == 0)
        {
            Execute(notifyData);
            return;
        }

        bool shouldExecute = observables[0].operatorValue == EvtObserableOption.Operator.AND ? true : false;
        foreach (var observable in observables)
        {
            bool isTriggered = IsObservableTriggered(observable.evtObserable);
            if (observable.operatorValue == EvtObserableOption.Operator.AND)
            {
                shouldExecute = shouldExecute && isTriggered;
            }
            else if (observable.operatorValue == EvtObserableOption.Operator.OR)
            {
                shouldExecute = shouldExecute || isTriggered;
            }
        }

        if (shouldExecute)
        {
            Execute(notifyData);
        }
    }

    private void OnDestroy()
    {
        EvtObserable.OnObservableRegistered -= OnObservableRegistered;

        foreach (var observable in runtimeSubscribedObservables.ToList())
        {
            Unsubscribe(observable);
        }
    }

    private bool IsObservableTriggered(EvtObserable observable)
    {
        if (observable == null)
            return false;

        string targetId = observable.ID;
        if (string.IsNullOrEmpty(targetId))
            return false;

        if (EvtRecordData.mapByEvtName.TryGetValue(targetId, out EvtRecordData recordData))
            return recordData.value > 0;

        return false;
    }

    private void OnObservableRegistered(EvtObserable runtimeObservable)
    {
        if (runtimeObservable == null)
            return;

        for (int i = 0; i < observables.Count; i++)
        {
            EvtObserable configured = observables[i].evtObserable;
            if (configured == null)
                continue;
            if (!string.Equals(configured.ID, runtimeObservable.ID, StringComparison.Ordinal))
                continue;

            Subscribe(runtimeObservable);
            break;
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        evtNameId = EvtIdentityValidation.Normalize(evtNameId);
        UnityEditor.EditorApplication.delayCall += DelayedValidateIdentity;
    }

    private void DelayedValidateIdentity()
    {
        if (this == null)
            return;

        if (!EvtIdentityValidation.IsValid(evtNameId))
        {
            Debug.LogWarning($"EvtObserver on '{name}' has an empty evtNameId. Assign a unique evtNameId in the Inspector.", this);
            return;
        }

        string conflict = EvtIdentityValidation.DescribeIdentityConflict(evtNameId);
        if (conflict != null)
            Debug.LogError(conflict, this);
    }
#endif
}