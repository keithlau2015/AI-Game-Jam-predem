using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class AssetsBundleManager
{
    private static async UniTask<T> LoadAssets<T>(string key)
    {
        AsyncOperationHandle<T> loadOp = Addressables.LoadAssetAsync<T>(key);
        await loadOp;
        if (loadOp.Status == AsyncOperationStatus.Succeeded)
        {
            T result = loadOp.Result;
            Addressables.Release(loadOp);
            return result;
        }
        else
        {
            Addressables.Release(loadOp);
            return default;
        }
    }

    public static async UniTask<Texture2D> LoadTexture2D(string key)
    {
        return await LoadAssets<Texture2D>(key);
    }

    public static async UniTask<Sprite> LoadSprite(string key)
    {
        return await LoadAssets<Sprite>(key);
    }

    public static async UniTask<Mesh> LoadMesh(string key)
    {
        return await LoadAssets<Mesh>(key);
    }

    public static async UniTask<Material> LoadMaterial(string key)
    {
        return await LoadAssets<Material>(key);
    }

    public static async UniTask<GameObject> LoadPrefab(string key, Transform parent = null)
    {
        AsyncOperationHandle<GameObject> loadOp = Addressables.LoadAssetAsync<GameObject>(key);
        await loadOp;
        if (loadOp.Status == AsyncOperationStatus.Succeeded)
        {
            AsyncOperationHandle asyncInstantiateOP = Addressables.InstantiateAsync(key, parent);
            await asyncInstantiateOP;
            if (asyncInstantiateOP.IsDone)
            {
                GameObject go = asyncInstantiateOP.Result as GameObject;
                Addressables.Release(loadOp);
                return go;
            }
        }
        return await LoadAssets<GameObject>(key); ;
    }
}
