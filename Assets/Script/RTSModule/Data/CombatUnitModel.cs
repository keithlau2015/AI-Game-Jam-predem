
namespace Model
{
    public class CombatUnitModel : Model<CombatUnitModel>
    {
        public int type { get; private set; }
        public string nameID { get; private set; }
        public string descriptionID { get; private set; }
        public string characterID { get; private set; }
        public string entityID { get; private set; }
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
        public string structureID { get; private set; }
        public CombatUnitModel(object key) : base(key)
        {

        }
    }
}