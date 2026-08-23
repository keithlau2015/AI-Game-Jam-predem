using System;
using System.Collections.Generic;

namespace SaveLoadModule
{
    /// <summary>
    /// Base for runtime catalogs that the RegistrySnapshot backend can reflect into a save blob.
    /// </summary>
    public abstract class SaveableModel<T> : Model<T> where T : Model<T>
    {
        public SaveableModel()
        {
            this.key = Guid.NewGuid().ToString();
            map[this.key] = this as T;
        }

        public SaveableModel(object key) : base(key)
        {
        }

        /// <summary>
        /// Replaces the in-memory catalog with loaded instances.
        /// </summary>
        public static void Load(List<T> loadedData)
        {
            map.Clear();
            if (loadedData == null)
                return;

            for (int i = 0; i < loadedData.Count; i++)
            {
                T model = loadedData[i];
                if (model == null || model.key == null)
                    continue;
                map[model.key] = model;
            }
        }
    }
}
