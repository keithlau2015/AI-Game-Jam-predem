using GameUI;
using SaveLoadModule;
using System;
using System.Collections.Generic;
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
        private IReadOnlyList<SaveSlotInfo> _slots = Array.Empty<SaveSlotInfo>();

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
            if (_slots == null || idx < 0 || idx >= _slots.Count)
                return;

            if (!transform.TryGetComponent(out SaveRecord saveRecord))
                return;

            saveRecord.SetUp(_slots[idx]);
        }

        public void ReturnObject(Transform trans)
        {
            pool.Release(trans.gameObject);
        }

        public async void Show()
        {
            await SaveService.EnsureCatalogLoaded();
            _slots = SaveService.ListSlots();

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
            loopVerticalScrollRect.totalCount = _slots.Count;
            loopVerticalScrollRect.RefillCells();
        }

        public void Hide()
        {
            Destroy(this.gameObject);
        }
    }
}
