using System.Collections.Generic;
using UnityEditor;

namespace SaveLoadModule
{
    public abstract class SaveableModel<T> : Model<T> where T : Model<T>
    {
        public SaveableModel() 
        { 
            this.key = GUID.Generate().ToString();
        }

        public SaveableModel(object key) : base(key)
        {
             
        }

        public static void Load(List<T> loadedData) {
            map.Clear();
            foreach(T model in loadedData) {
                if (!map.ContainsKey(model.key)) continue;
                map.Add(model.key, model);
            }
        }
    }
}