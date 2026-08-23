using System.Collections.Generic;
using System.Linq;
using CombatUnitModule;
using Newtonsoft.Json;

namespace SaveLoadModule.DocumentDto.Participants
{
    public sealed class CombatUnitSlotSaveDto
    {
        public string key;
        public string unitUID;
        public string equipmentUID;
        public int slotIndex;
    }

    public sealed class CombatUnitSlotSaveParticipant : ISaveParticipant
    {
        public string SectionId => "combatUnitSlots";

        public string CaptureJson()
        {
            List<CombatUnitSlotSaveDto> dtos = CombatUnitEquipmentSlotData.map.Values
                .Where(s => s != null)
                .Select(s => new CombatUnitSlotSaveDto
                {
                    key = s.key?.ToString(),
                    unitUID = s.unitUID,
                    equipmentUID = s.equipmentUID,
                    slotIndex = s.slotIndex
                })
                .ToList();
            return JsonConvert.SerializeObject(dtos);
        }

        public void ClearRuntime()
        {
            CombatUnitEquipmentSlotData.map.Clear();
        }

        public void RestoreJson(string json)
        {
            List<CombatUnitSlotSaveDto> dtos = JsonConvert.DeserializeObject<List<CombatUnitSlotSaveDto>>(json);
            if (dtos == null)
                return;

            foreach (CombatUnitSlotSaveDto dto in dtos)
            {
                if (dto == null || string.IsNullOrEmpty(dto.key))
                    continue;
                CombatUnitEquipmentSlotData.FromSave(dto.key, dto.unitUID, dto.equipmentUID, dto.slotIndex);
            }

            // Re-link slot lists on units when present
            foreach (CombatUnitData unit in CombatUnitData.map.Values)
            {
                if (unit == null)
                    continue;
                unit.slotList = CombatUnitEquipmentSlotData.GetSlotsByUnitUID(unit.key?.ToString());
            }
        }
    }
}
