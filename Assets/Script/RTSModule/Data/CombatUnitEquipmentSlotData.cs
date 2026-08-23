using EquipmentModule;
using SaveLoadModule;
using System.Collections.Generic;
using System.Linq;
namespace CombatUnitModule
{
    public class CombatUnitEquipmentSlotData : SaveableModel<CombatUnitEquipmentSlotData>
    {
        public string unitUID { get; private set; }
        public string equipmentUID { get; private set; }
        public int slotIndex { get; private set; }

        public bool isEmpty { get { return string.IsNullOrEmpty(equipmentUID); } }
        public EquipmentData equipmentData
        {
            get
            {
                if (string.IsNullOrEmpty(equipmentUID)) return null;
                EquipmentData equipmentData = null;
                EquipmentData.map.TryGetValue(equipmentUID, out equipmentData);
                return equipmentData;
            }
        }
        public CombatUnitEquipmentSlotData(object key) : base(key)
        {

        }

        public CombatUnitEquipmentSlotData(string unitUID, string equipmentUID, int slotIndex) : base()
        {
            this.unitUID = unitUID;
            this.equipmentUID = equipmentUID;
            this.slotIndex = slotIndex;
        }

        public static CombatUnitEquipmentSlotData FromSave(object key, string unitUID, string equipmentUID, int slotIndex)
        {
            CombatUnitEquipmentSlotData slot = new CombatUnitEquipmentSlotData(key);
            slot.unitUID = unitUID;
            slot.equipmentUID = equipmentUID;
            slot.slotIndex = slotIndex;
            return slot;
        }

        public static List<CombatUnitEquipmentSlotData> GetSlotsByUnitUID(string unitUID)
        {
            return map.Values.Where(x => x.unitUID == unitUID).ToList();
        }

        public EquipmentData GetEquipmentData()
        {
            if (string.IsNullOrEmpty(this.equipmentUID)) return null;
            EquipmentData equipmentData = null;
            EquipmentData.map.TryGetValue(this.equipmentUID, out equipmentData);
            return equipmentData;
        }

        public CombatUnitData GetCombatUnitData()
        {
            if (string.IsNullOrEmpty(this.unitUID)) return null;
            CombatUnitData unitData = null;
            CombatUnitData.map.TryGetValue(this.unitUID, out unitData);
            return unitData;
        }
    }
}