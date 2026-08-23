using System.Collections.Generic;
using System.Linq;
using ItemModule;
using Newtonsoft.Json;

namespace SaveLoadModule.DocumentDto.Participants
{
    public sealed class ItemSaveDto
    {
        public string key;
        public string id;
        public long count;
        public string ownerUID;
    }

    public sealed class ItemSaveParticipant : ISaveParticipant
    {
        public string SectionId => "items";

        public string CaptureJson()
        {
            List<ItemSaveDto> dtos = ItemData.map.Values
                .Where(i => i != null)
                .Select(i => new ItemSaveDto
                {
                    key = i.key?.ToString(),
                    id = i.id,
                    count = i.count,
                    ownerUID = i.ownerUID
                })
                .ToList();
            return JsonConvert.SerializeObject(dtos);
        }

        public void ClearRuntime()
        {
            ItemData.map.Clear();
            ItemData.itemGroupsByOwner.Clear();
        }

        public void RestoreJson(string json)
        {
            List<ItemSaveDto> dtos = JsonConvert.DeserializeObject<List<ItemSaveDto>>(json);
            if (dtos == null)
                return;

            foreach (ItemSaveDto dto in dtos)
            {
                if (dto == null || string.IsNullOrEmpty(dto.key) || string.IsNullOrEmpty(dto.id))
                    continue;
                ItemData.FromSave(dto.key, dto.id, dto.count, dto.ownerUID);
            }

            ItemData.RebuildOwnerGroups();
        }
    }
}
