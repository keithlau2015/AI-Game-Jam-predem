using ItemModule;

namespace Model
{
    public class EquipmentModel : Model<EquipmentModel>
    {
        public string item { get; private set; }
        public int type { get; private set; }
        public int maxHp { get; private set; }
        public int hp { get; private set; }
        public int atk { get; private set; }
        public int def { get; private set; }
        public int hit { get; private set; }
        public int dodge { get; private set; }
        public int cri { get; private set; }
        public int criDmg { get; private set; }
        public int spd { get; private set; }
        public int inspectRange { get; private set; }
        public int counterInspectRange { get; private set; }
        public int maxShield { get; private set; }
        public int shield { get; private set; }
        public int shieldRegenSpd { get; private set; }
        public int armorEfficiency { get; private set; }

        public EquipmentModel(string id) : base(id)
        {

        }

        public ItemModel GetItemModel()
        {
            ItemModel itemModel = null;
            ItemModel.map.TryGetValue(this.item, out itemModel);
            return itemModel;
        }
    }
}