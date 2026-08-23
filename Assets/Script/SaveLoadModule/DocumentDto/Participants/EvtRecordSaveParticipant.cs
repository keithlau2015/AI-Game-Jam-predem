using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace SaveLoadModule.DocumentDto.Participants
{
    public sealed class EvtRecordSaveDto
    {
        public string key;
        public string id;
        public int value;
    }

    public sealed class EvtRecordSaveParticipant : ISaveParticipant
    {
        public string SectionId => "evtRecords";

        public string CaptureJson()
        {
            List<EvtRecordSaveDto> dtos = EvtRecordData.map.Values
                .Where(e => e != null)
                .Select(e => new EvtRecordSaveDto
                {
                    key = e.key?.ToString(),
                    id = e.id,
                    value = e.value
                })
                .ToList();
            return JsonConvert.SerializeObject(dtos);
        }

        public void ClearRuntime()
        {
            EvtRecordData.map.Clear();
            EvtRecordData.mapByEvtName.Clear();
        }

        public void RestoreJson(string json)
        {
            List<EvtRecordSaveDto> dtos = JsonConvert.DeserializeObject<List<EvtRecordSaveDto>>(json);
            if (dtos == null)
                return;

            foreach (EvtRecordSaveDto dto in dtos)
            {
                if (dto == null || string.IsNullOrEmpty(dto.key))
                    continue;
                EvtRecordData.FromSave(dto.key, dto.id, dto.value);
            }
        }
    }
}
