using System.Collections.Generic;
using UnityEngine;

public class GlobalEvtController : Singleton<GlobalEvtController>
{
    [SerializeField]
    private List<EvtObserable> observables;
    public Dictionary<string, EvtObserable> observableMap { get; private set; } = new Dictionary<string, EvtObserable>();

    private void Start()
    {
        foreach (var observable in observables)
        {
            if (!observableMap.ContainsKey(observable.ID))
            {
                observableMap.Add(observable.ID, observable);
            }
            else
            {
                Debug.LogWarning($"EvtController: duplicate observable id {observable.ID} found, only the first one will be added to map");
            }
        }
    }
}