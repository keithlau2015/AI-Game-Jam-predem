using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class GameAssetsBundleManager : Singleton<GameAssetsBundleManager>
{
    public static async UniTask<Sprite> LoadSprite(string key)
    {
        AsyncOperationHandle<Sprite> loadOp = Addressables.LoadAssetAsync<Sprite>(key);
        await loadOp;
        if (loadOp.Status == AsyncOperationStatus.Succeeded)
        {
            Sprite sprite = loadOp.Result;
            Addressables.Release(loadOp);
            return sprite;
        }
        else
        {
            Addressables.Release(loadOp);
            return null;
        }
    }

    public static async UniTask<GameObject> LoadGameObject(string key)
    {
        AsyncOperationHandle<GameObject> loadOp = Addressables.LoadAssetAsync<GameObject>(key);
        await loadOp;
        if (loadOp.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject go = loadOp.Result;
            Addressables.Release(loadOp);
            return go;
        }
        else
        {
            Addressables.Release(loadOp);
            return null;
        }
    }

    public static async UniTask<AudioClip> LoadAudio(string key)
    {
        AsyncOperationHandle<AudioClip> loadOp = Addressables.LoadAssetAsync<AudioClip>(key);
        await loadOp;
        if(loadOp.Status == AsyncOperationStatus.Succeeded)
        {
            AudioClip audioClip = loadOp.Result;
            Addressables.Release(loadOp);
            return audioClip;
        }
        else
        {
            Addressables.Release(loadOp);
            return null;
        }
    }
}
