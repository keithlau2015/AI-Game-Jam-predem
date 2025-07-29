using System;
using System.Collections.Generic;
using GenericGameModule;
using SaveLoadModule;

namespace ItemModule
{
    public class ItemData : SaveableModel<ItemData>
    {
        public string id { get; private set; }
        public long count { get; private set; }
        public string ownerUID { get; private set; } = string.Empty;

        public ItemData(string id)
        {
            this.key = Guid.NewGuid().ToString();
            this.id = id;

        }

        public void Stack(long count)
        {
            ItemModel itemModel = null;
            if (!ItemModel.map.TryGetValue(this.id, out itemModel))
                return;

            long overflow = this.count + count - itemModel.maxStack;
            if (overflow > 0)
            {

            }
            else
                this.count += count;
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