using System;
using UnityEngine;

public class PoolObjectProperty : MonoBehaviour
{
    public int ID { get; set; }
    public event Action onDiscard;
    public event Action onDestroy;
    public event Action onSpawn;

    private void OnEnable()
    {
        onSpawn?.Invoke();
    }

    private void OnDisable()
    {
        onDiscard?.Invoke();
    }

    private void OnDestroy()
    {
        onDestroy?.Invoke();
        onSpawn = null;
        onDiscard = null;
        onDestroy = null;
    }
}
