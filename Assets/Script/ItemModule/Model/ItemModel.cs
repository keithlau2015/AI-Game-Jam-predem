using ObjetPoolModule;

namespace Model
{
    public class ItemModel : Model<ItemModel>
    {        
        public long maxStack { get; private set; }
        public string entityID { get; private set; }
        public string nameID { get; private set; }
        public string descriptionID { get; private set; }
        public string iconID { get; private set; }
        public string backgroundID { get; private set; }
        public string frameID { get; private set; }

        public ItemModel(string id) : base(id)
        {

        }

        public bool IsEntity()
        {
            return string.IsNullOrEmpty(entityID);
        }

        public ObjectPool GetObjectPool()
        {
            if (string.IsNullOrEmpty(entityID))
            {
                return null;
            }
            ObjectPool pool = null;
            ObjectPoolManager.singleton.pools.TryGetValue(entityID, out pool);
            return pool;
        }
    }
}