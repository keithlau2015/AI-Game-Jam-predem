using System.Collections.Generic;
using System.Linq;

namespace Model
{
    public class CombatUnitEquipmentSlotModel : Model<CombatUnitEquipmentSlotModel>
    {
        public enum SlotType
        {
            Weapon,
            Shield,
            Engine,
            Structure,
            Special
        }

        public string unitId { get; private set; }
        public int type { get; private set; }
        public string defaultEquipmentId { get; private set; } // This is the default equipment ID for this slot type, can be null or empty if no default equipment is set.
        public CombatUnitEquipmentSlotModel(object key) : base(key)
        {

        }

        public static List<CombatUnitEquipmentSlotModel> GetModelListByUnit(string unitKey)
        {
            return map.Values.ToList().FindAll(slot => slot.unitId.Equals(unitKey)).ToList();
        }
    }
}