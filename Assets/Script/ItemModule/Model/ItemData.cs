using System;
using System.Collections.Generic;
using Model;
using SaveLoadModule;
using System.Linq;

namespace ItemModule
{
    public class ItemData : SaveableModel<ItemData>
    {
        public string id { get; private set; }
        public long count { get; private set; }
        public string ownerUID { get; set; } = string.Empty;

        public event Action<long> OnItemCountChanged;
        public static SortedDictionary<string, Dictionary<string, List<ItemData>>> itemGroupsByOwner { get; private set; } = new SortedDictionary<string, Dictionary<string, List<ItemData>>>();

        public ItemData(string id, string owner = "") : base()
        {
            this.id = id;
            this.ownerUID = owner ?? string.Empty;
            RegisterInOwnerGroups();
        }

        /// <summary>Restore a persisted stack without allocating a new key.</summary>
        public static ItemData FromSave(object key, string id, long count, string owner)
        {
            ItemData item = new ItemData(key);
            item.id = id;
            item.count = count;
            item.ownerUID = owner ?? string.Empty;
            item.RegisterInOwnerGroups();
            return item;
        }

        // Load ctor used by registry + FromSave
        public ItemData(object key) : base(key)
        {
        }

        public new static void Load(List<ItemData> loadedData)
        {
            map.Clear();
            if (loadedData != null)
            {
                for (int i = 0; i < loadedData.Count; i++)
                {
                    ItemData model = loadedData[i];
                    if (model == null || model.key == null)
                        continue;
                    map[model.key] = model;
                }
            }
            RebuildOwnerGroups();
        }

        public static void RebuildOwnerGroups()
        {
            itemGroupsByOwner.Clear();
            foreach (ItemData item in map.Values)
            {
                if (item == null)
                    continue;
                item.RegisterInOwnerGroups();
            }
        }

        private void RegisterInOwnerGroups()
        {
            if (!itemGroupsByOwner.TryGetValue(ownerUID, out Dictionary<string, List<ItemData>> ownerGroups))
            {
                ownerGroups = new Dictionary<string, List<ItemData>>();
                itemGroupsByOwner[ownerUID] = ownerGroups;
            }

            if (!ownerGroups.TryGetValue(id, out List<ItemData> group))
            {
                group = new List<ItemData>();
                ownerGroups[id] = group;
            }

            if (!group.Contains(this))
                group.Add(this);
        }

        public void Stack(long delta)
        {
            ItemModel itemModel = null;
            if (!ItemModel.map.TryGetValue(this.id, out itemModel))
                return;

            long next = this.count + delta;
            if (next < 0)
                next = 0;
            if (itemModel.maxStack > 0 && next > itemModel.maxStack)
                next = itemModel.maxStack;

            this.count = next;
            OnItemCountChanged?.Invoke(delta);
        }


        public static List<ItemData> GetItemsByOwner(string ownerUID)
        {            
            if (itemGroupsByOwner.TryGetValue(ownerUID, out var ownerGroups))
            {
                return ownerGroups.Values.SelectMany(group => group).ToList();
            }

            return new List<ItemData>();
        }

        public static List<ItemData> GetItemsByOwnerNID(string ownerUID, string id)
        {
            List<ItemData> items = new List<ItemData>();
            if (!itemGroupsByOwner.TryGetValue(ownerUID ?? string.Empty, out var ownerGroups))
                return items;

            if (ownerGroups.TryGetValue(id, out var group))
                items.AddRange(group);

            return items;
        }

        public static ItemData GetStackableItemDataByOwnerNID(string ownerUID, string id)
        {
            var items = GetItemsByOwnerNID(ownerUID, id);
            ItemModel itemModel = null;
            if (ItemModel.map.TryGetValue(id, out itemModel))
            {
                return items.Find(item => item.count < itemModel.maxStack);
            }
            return null;
        }

        public ItemData Clone()
        {
            ItemData itemInstance = new ItemData(this.id);
            itemInstance.count = this.count;
            itemInstance.ownerUID = this.ownerUID;
            return itemInstance;
        }
    }
}