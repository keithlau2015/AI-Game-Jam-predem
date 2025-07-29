using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField]
    private GameObject prefab;
    [Tooltip("Size should smaller than the Max Pool Size")]
    [SerializeField]
    private int size;
    [SerializeField]
    private bool fixedSize = false;
    private Transform parent;
    private Queue<GameObject> pool = new Queue<GameObject>();

    public void init(GameObject prefab, int size, bool isFixedSize, Transform parent)
    {
        this.prefab = prefab;
        this.size = size;
        this.fixedSize = isFixedSize;
        this.parent = parent;

        for (int i = 0; i < size; i++)
        {
            AddObject2Pool();
        }
    }

    //Create New objData by Config
    public GameObject SpawnFromPool()
    {    
        if (pool.Count <= 0)
        {
            if (fixedSize)
            {
                Debug.LogWarning($"The pool is empty!! Recommand to increase to pool size!! Current pool size {this.pool.Count}");
                return null;
            }
            else
            {
                AddObject2Pool();
                Debug.LogWarning($"The pool is empty!! Before Dequeue, Will Be Add New Object, Current pool size: {this.pool.Count}");
            }
        }
        GameObject objectToSpawn = pool.Dequeue();
        objectToSpawn.SetActive(true);
        return objectToSpawn;
    }

    private GameObject AddObject2Pool()
    {
        GameObject obj = Instantiate(this.prefab, parent);
        obj.SetActive(false);
        PoolObjectProperty poolObjectProperty = null;
        if (obj.TryGetComponent(out poolObjectProperty))
        {
            poolObjectProperty.onDiscard += () => {
                if (poolObjectProperty.gameObject == null)
                    return;

                pool.Enqueue(obj);
            };

            poolObjectProperty.onDestroy += () => {
                pool = new Queue<GameObject>(pool.Where(x => !x.Equals(obj)));
            };
        }
        pool.Enqueue(obj);
        return obj;
    }

    public void ReleaseAllGameObjects()
    {
        if (pool.Count <= 0) return;
        foreach (GameObject go in pool)
        {
            Destroy(go);
        }
        pool.Clear();
    }
}
