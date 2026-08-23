using System;
using UnityEngine;

public class PoolObjectProperty : MonoBehaviour
{
    public int ID { get; set; }
    public event Action<PoolObjectProperty> onDiscard;
    public event Action onDestroy;
    public event Action<PoolObjectProperty> onSpawn;

    protected virtual void OnEnable()
    {
        onSpawn?.Invoke(this);
    }

    protected virtual void OnDisable()
    {
        onDiscard?.Invoke(this);
    }

    private void OnDestroy()
    {
        onDestroy?.Invoke();
        onSpawn = null;
        onDiscard = null;
        onDestroy = null;
    }
}
