using System.Collections.Generic;
using System.Linq;
using EquipmentModule;
using Newtonsoft.Json;

namespace SaveLoadModule.DocumentDto.Participants
{
    public sealed class EquipmentSaveDto
    {
        public string key;
        public string id;
        public string ownerUID;
    }

    public sealed class EquipmentSaveParticipant : ISaveParticipant
    {
        public string SectionId => "equipment";

        public string CaptureJson()
        {
            List<EquipmentSaveDto> dtos = EquipmentData.map.Values
                .Where(e => e != null)
                .Select(e => new EquipmentSaveDto
                {
                    key = e.key?.ToString(),
                    id = e.id?.ToString(),
                    ownerUID = e.ownerUID
                })
                .ToList();
            return JsonConvert.SerializeObject(dtos);
        }

        public void ClearRuntime()
        {
            EquipmentData.map.Clear();
            EquipmentData.mapByOwner.Clear();
        }

        public void RestoreJson(string json)
        {
            List<EquipmentSaveDto> dtos = JsonConvert.DeserializeObject<List<EquipmentSaveDto>>(json);
            if (dtos == null)
                return;

            foreach (EquipmentSaveDto dto in dtos)
            {
                if (dto == null || string.IsNullOrEmpty(dto.key) || string.IsNullOrEmpty(dto.id))
                    continue;
                EquipmentData.FromSave(dto.key, dto.id, dto.ownerUID);
            }
        }
    }
}
