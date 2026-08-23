using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class EvtObserable : MonoBehaviour
{
    public static event Action<EvtObserable> OnObservableRegistered;
    public static event Action<EvtObserable> OnObservableUnregistered;
    private static readonly Dictionary<string, HashSet<EvtObserable>> registryById = new Dictionary<string, HashSet<EvtObserable>>();

    //[SerializeField]
    private List<EvtObserver> _observers = new List<EvtObserver>();
    [SerializeField]
    private string id;
    public string ID => EvtIdentityValidation.Normalize(id);
    [SerializeField]
    private bool onDestroyDataReset = false;

    public EvtRecordData recordData;

    /*
    public List<EvtObserver> observers
    {
        get
        {
            return _observers;
        }
    }
    */

    public void Subscribe(EvtObserver observer)
    {
        if (_observers.Contains(observer))
            return;
        _observers.Add(observer);
    }

    public void Unsubscribe(EvtObserver observer)
    {
        _observers.Remove(observer);
    }

    public static IEnumerable<EvtObserable> GetById(string id)
    {
        string normalizedId = EvtIdentityValidation.Normalize(id);
        if (string.IsNullOrEmpty(normalizedId))
            yield break;
        if (!registryById.TryGetValue(normalizedId, out HashSet<EvtObserable> observables))
            yield break;

        foreach (EvtObserable observable in observables)
        {
            if (observable != null)
                yield return observable;
        }
    }

    private void OnEnable()
    {
        if (onDestroyDataReset)
            Reset();
        RegisterObservable();
    }

    private void OnDisable()
    {
        UnregisterObservable();
    }

    private void RegisterObservable()
    {
        string key = ID;
        if (string.IsNullOrEmpty(key))
            return;

        if (!registryById.TryGetValue(key, out HashSet<EvtObserable> observables))
        {
            observables = new HashSet<EvtObserable>();
            registryById.Add(key, observables);
        }

        if (observables.Add(this))
            OnObservableRegistered?.Invoke(this);
    }

    private void UnregisterObservable()
    {
        string key = ID;
        if (string.IsNullOrEmpty(key))
            return;
        if (!registryById.TryGetValue(key, out HashSet<EvtObserable> observables))
            return;

        if (observables.Remove(this))
            OnObservableUnregistered?.Invoke(this);
        if (observables.Count == 0)
            registryById.Remove(key);
    }

    protected void Notify(EvtNotifyData notifyData = null)
    {
        Debug.Log("Notify: " + ID);
        if (notifyData == null)
            notifyData = new EvtNotifyData();
        if (notifyData.observable == null)
            notifyData.observable = this;

        if(recordData == null)
        {
            string key = ID;
            if(EvtRecordData.mapByEvtName.TryGetValue(key, out recordData))
            {
                Debug.Log($"EvtObserable {key} found record data in map");

            }
            else
            {
                Debug.LogWarning($"EvtObserable {key} cannot find record data in map");
                recordData = new EvtRecordData(key);
            }
        }

        recordData.value++;

        foreach (var observer in _observers)
        {
            observer.Notify(notifyData);
        }
    }

    public void Reset()
    {
        string key = ID;
        if (!string.IsNullOrEmpty(key) && EvtRecordData.mapByEvtName.TryGetValue(key, out EvtRecordData data))
            data.value = 0;
        recordData = null;
    }

    protected void OnDestroy()
    {
        UnregisterObservable();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        id = EvtIdentityValidation.Normalize(id);
        UnityEditor.EditorApplication.delayCall += DelayedValidateIdentity;
    }

    private void DelayedValidateIdentity()
    {
        if (this == null)
            return;

        if (!EvtIdentityValidation.IsValid(id))
        {
            Debug.LogWarning($"EvtObserable on '{name}' has an empty id. Assign a unique id in the Inspector.", this);
            return;
        }

        string conflict = EvtIdentityValidation.DescribeIdentityConflict(id);
        if (conflict != null)
            Debug.LogError(conflict, this);
    }
#endif
}