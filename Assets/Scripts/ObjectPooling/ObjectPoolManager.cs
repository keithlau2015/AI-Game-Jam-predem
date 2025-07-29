using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GenericGameModule;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[System.Serializable]
public struct ObjectPoolProperties
{
    public string id;
    public int initPoolSize;
    public bool isFixedSize;
    public GameObject targetObject;
}

public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    [SerializeField]
    private List<ObjectPoolProperties> properties = new List<ObjectPoolProperties>();
    public Dictionary<string, ObjectPool> pools = new Dictionary<string, ObjectPool>();    

    protected override void Awake()
    {
        base.Awake();
        Application.lowMemory += ReleaseAllNonUseGameObject;
    }

    public async void SetUp(IProgress<int> progress)
    {
        int totalProgress = 0;
        properties.Clear();
        pools.Clear();

        foreach (KeyValuePair<object, EntityModel> keyValuePair in EntityModel.map)
        {
            ObjectPoolProperties newPoolProperties = new ObjectPoolProperties();
            newPoolProperties.id = keyValuePair.Key.ToString();
            newPoolProperties.initPoolSize = 0;
            newPoolProperties.isFixedSize = false;

            if (!string.IsNullOrEmpty(keyValuePair.Key.ToString()))
            {
                AsyncOperationHandle<GameObject> loadOp = Addressables.LoadAssetAsync<GameObject>(keyValuePair.Value.prefabKey);
                loadOp.Completed += (asyncOp) => {
                    if (asyncOp.Status == AsyncOperationStatus.Succeeded)
                    {
                        newPoolProperties.targetObject = asyncOp.Result;
                        if (!newPoolProperties.targetObject.TryGetComponent<PoolObjectProperty>(out _))
                            newPoolProperties.targetObject.AddComponent(typeof(PoolObjectProperty));
                    }

                    Addressables.Release(asyncOp);
                };
                await loadOp;
                properties.Add(newPoolProperties);                
            }
            totalProgress++;
            progress.Report(totalProgress);
        }

        for (int i = 0; i < properties.Count; i++)
        {
            ObjectPoolProperties property = properties[i];
            GameObject gameObjectPool = new GameObject($"Pool [{property.targetObject.name}]");
            gameObjectPool.transform.SetParent(this.transform);
            ObjectPool pool = gameObjectPool.AddComponent<ObjectPool>();
            pool.init(property.targetObject, property.initPoolSize, property.isFixedSize, gameObjectPool.transform);
            pools.Add(property.id, pool);

        }
        progress.Report(totalProgress + 1);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        Application.lowMemory -= ReleaseAllNonUseGameObject;
    }

    private void ReleaseAllNonUseGameObject()
    {
        foreach (ObjectPool objectPool in pools.Values)
        {
            objectPool.ReleaseAllGameObjects();
        }
    }
}
