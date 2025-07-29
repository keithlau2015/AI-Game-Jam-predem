using GameUI;
using SaveLoadModule;
using System;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace ItemModule
{
    public class SaveLoadMenu : MonoBehaviour, IPreviousablePanel, LoopScrollPrefabSource, LoopScrollDataSource
    {
        [SerializeField]
        private SaveRecord saveRecordPrefab;
        [SerializeField]
        private int totalCount = -1;
        [SerializeField]
        private LoopVerticalScrollRect loopVerticalScrollRect;
        private ObjectPool<GameObject> pool;
        private Action onSelectCB;
        public GameObject GetObject(int index)
        {
            return pool.Get();
        }

        public void SetUp(Action onClickCB)
        {
            this.onSelectCB = onClickCB;
        }

        public void ProvideData(Transform transform, int idx)
        {
            if (idx > SaveDataModel.map.Count || idx < 0)
                return;

            SaveDataModel model = null;
            if (!SaveDataModel.map.TryGetValue(idx, out model))
                return;

            SaveRecord saveRecord = null;
            if (!transform.TryGetComponent(out saveRecord))
                return;

            saveRecord.SetUp(model.key as string);
        }

        public void ReturnObject(Transform trans)
        {
            pool.Release(trans.gameObject);
        }

        public void Show()
        {
            pool = new ObjectPool<GameObject>(
               () => Instantiate(saveRecordPrefab.gameObject),
               o => o.SetActive(true),
               o =>
               {
                   o.transform.SetParent(transform);
                   o.SetActive(false);
               });
            loopVerticalScrollRect.prefabSource = this;
            loopVerticalScrollRect.dataSource = this;
            loopVerticalScrollRect.totalCount = SaveDataModel.map.Count;
            loopVerticalScrollRect.RefillCells();
        }

        public void Hide()
        {
            Destroy(this.gameObject);
        }
    }
}