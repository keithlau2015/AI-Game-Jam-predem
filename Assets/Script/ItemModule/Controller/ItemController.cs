using ItemModule;
using Model;
using SaveLoadModule;
using System.Collections.Generic;
using UnityEngine;

namespace ItemSystem
{
    /// <summary>
    /// Grant / consume helpers for ItemData stacks.
    /// </summary>
    public class ItemController
    {
        public static ItemData Grant(string ownerUID, string itemId, long count = 1)
        {
            if (string.IsNullOrEmpty(itemId) || count <= 0)
                return null;

            if (!ItemModel.map.TryGetValue(itemId, out ItemModel itemModel))
            {
                Debug.LogError($"[Item] Unknown item id '{itemId}'");
                return null;
            }

            long remaining = count;
            ItemData last = null;
            while (remaining > 0)
            {
                ItemData stack = ItemData.GetStackableItemDataByOwnerNID(ownerUID, itemId);
                if (stack == null)
                    stack = new ItemData(itemId, ownerUID ?? string.Empty);

                long room = itemModel.maxStack - stack.count;
                if (room <= 0)
                {
                    stack = new ItemData(itemId, ownerUID ?? string.Empty);
                    room = itemModel.maxStack;
                }

                long add = remaining < room ? remaining : room;
                stack.Stack(add);
                remaining -= add;
                last = stack;
            }

            return last;
        }

        public static bool Consume(string ownerUID, string itemId, long count = 1)
        {
            if (string.IsNullOrEmpty(itemId) || count <= 0)
                return false;

            List<ItemData> stacks = ItemData.GetItemsByOwnerNID(ownerUID, itemId);
            long total = 0;
            for (int i = 0; i < stacks.Count; i++)
                total += stacks[i].count;

            if (total < count)
                return false;

            long remaining = count;
            for (int i = 0; i < stacks.Count && remaining > 0; i++)
            {
                ItemData stack = stacks[i];
                long take = stack.count < remaining ? stack.count : remaining;
                stack.Stack(-take);
                remaining -= take;
            }

            return remaining == 0;
        }

        public static long GetCount(string ownerUID, string itemId)
        {
            List<ItemData> stacks = ItemData.GetItemsByOwnerNID(ownerUID, itemId);
            long total = 0;
            for (int i = 0; i < stacks.Count; i++)
                total += stacks[i].count;
            return total;
        }

        public static string CurrentOwnerUID
        {
            get
            {
                return SaveLoadController.currentSaveKey != null
                    ? SaveLoadController.currentSaveKey.ToString()
                    : string.Empty;
            }
        }
    }
}
