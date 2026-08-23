using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace SaveLoadModule.DocumentDto.Participants
{
    public sealed class SkinSaveDto
    {
        public string key;
        public string id;
        public ushort materialIndex;
        public string ownerUID;
    }

    public sealed class SkinSaveParticipant : ISaveParticipant
    {
        public string SectionId => "skins";

        public string CaptureJson()
        {
            List<SkinSaveDto> dtos = SkinData.map.Values
                .Where(s => s != null)
                .Select(s => new SkinSaveDto
                {
                    key = s.key?.ToString(),
                    id = s.id,
                    materialIndex = s.materialIndex,
                    ownerUID = s.ownerUID
                })
                .ToList();
            return JsonConvert.SerializeObject(dtos);
        }

        public void ClearRuntime()
        {
            SkinData.map.Clear();
            SkinData.mapByOwner.Clear();
        }

        public void RestoreJson(string json)
        {
            List<SkinSaveDto> dtos = JsonConvert.DeserializeObject<List<SkinSaveDto>>(json);
            if (dtos == null)
                return;

            foreach (SkinSaveDto dto in dtos)
            {
                if (dto == null || string.IsNullOrEmpty(dto.key))
                    continue;
                SkinData.FromSave(dto.key, dto.id, dto.materialIndex, dto.ownerUID);
            }
        }
    }
}
