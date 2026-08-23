using ObjetPoolModule;
using UnityEngine;

namespace EvtModule
{
    /// <summary>
    /// Spawns a pooled GameObject when the linked observable fires.
    /// Position can be fixed, or taken from notify payload ("position" / "gameObject").
    /// Optional ISpawnActivatable on the spawned object is called after spawn.
    /// </summary>
    public class ObjectSpawnObserver : EvtObserver
    {
        public enum PositionSource
        {
            Fixed = 0,
            FromNotifyPosition = 1,
            FromNotifyGameObject = 2,
        }

        [System.Serializable]
        private class SpawnInfo
        {
            [Tooltip("ObjectPoolManager pool id / entity key.")]
            public string poolKey;
            public PositionSource positionSource = PositionSource.Fixed;
            public Vector3 position;
            public bool inheritRotationFromNotify;
        }

        [SerializeField]
        private SpawnInfo spawnInfo = new SpawnInfo();

        protected override void OnExecute(EvtNotifyData evtNotifyData)
        {
            if (spawnInfo == null || string.IsNullOrEmpty(spawnInfo.poolKey))
            {
                Debug.LogError("[ObjectSpawnObserver] poolKey is empty.", this);
                return;
            }

            if (ObjectPoolManager.singleton == null
                || !ObjectPoolManager.singleton.pools.TryGetValue(spawnInfo.poolKey, out ObjectPool pool))
            {
                Debug.LogError($"[ObjectSpawnObserver] pool not found for key '{spawnInfo.poolKey}'", this);
                return;
            }

            Vector3 spawnPos = ResolvePosition(evtNotifyData);
            Quaternion spawnRot = ResolveRotation(evtNotifyData);

            GameObject go = pool.SpawnFromPool(spawnPos);
            if (go == null)
            {
                Debug.LogError($"[ObjectSpawnObserver] SpawnFromPool returned null for '{spawnInfo.poolKey}'", this);
                return;
            }

            if (spawnInfo.inheritRotationFromNotify)
                go.transform.rotation = spawnRot;

            ISpawnActivatable[] activatables = go.GetComponentsInChildren<ISpawnActivatable>(true);
            for (int i = 0; i < activatables.Length; i++)
                activatables[i].OnSpawnActivated();
        }

        private Vector3 ResolvePosition(EvtNotifyData evtNotifyData)
        {
            if (spawnInfo.positionSource == PositionSource.Fixed)
                return spawnInfo.position;

            if (evtNotifyData?.values == null)
                return spawnInfo.position;

            if (spawnInfo.positionSource == PositionSource.FromNotifyPosition
                && evtNotifyData.values.TryGetValue("position", out object posObj))
            {
                if (posObj is Vector3 v3)
                    return v3;
            }

            if (spawnInfo.positionSource == PositionSource.FromNotifyGameObject
                && evtNotifyData.values.TryGetValue("gameObject", out object goObj)
                && goObj is GameObject go
                && go != null)
            {
                return go.transform.position;
            }

            return spawnInfo.position;
        }

        private Quaternion ResolveRotation(EvtNotifyData evtNotifyData)
        {
            if (!spawnInfo.inheritRotationFromNotify || evtNotifyData?.values == null)
                return Quaternion.identity;

            if (evtNotifyData.values.TryGetValue("gameObject", out object goObj)
                && goObj is GameObject go
                && go != null)
            {
                return go.transform.rotation;
            }

            return Quaternion.identity;
        }
    }
}
