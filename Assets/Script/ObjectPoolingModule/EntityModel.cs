using System.Collections.Generic;
namespace Model
{
    public class EntityModel : Model<EntityModel>
    {
        public ushort defaultSkinIndex { get; private set; }
        private string _prefabKey;
        public string prefabKey {
            get
            {
                return _prefabKey;
            } 
            private set
            {
                _prefabKey = value;
                List<EntityModel> list = null;
                if(!mapByPrefabKey.TryGetValue(value, out list))
                {
                    list = new List<EntityModel>();
                    mapByPrefabKey.Add(value, list);
                }
                list.Add(this);
            }
        }

        public EntityModel(object key) : base(key) { }

        public static SortedDictionary<string, List<EntityModel>> mapByPrefabKey = new SortedDictionary<string, List<EntityModel>>();
    }
}