using System.Collections.Generic;
using System.Linq;
using Model;
using Newtonsoft.Json;

namespace SaveLoadModule.DocumentDto.Participants
{
    public sealed class AchievementSaveDto
    {
        public string key;
        public string achievementKey;
    }

    public sealed class AchievementSaveParticipant : ISaveParticipant
    {
        public string SectionId => "achievements";

        public string CaptureJson()
        {
            List<AchievementSaveDto> dtos = AchievementHistoryModel.map.Values
                .Where(a => a != null)
                .Select(a => new AchievementSaveDto
                {
                    key = a.key?.ToString(),
                    achievementKey = a.achievementKey?.ToString()
                })
                .ToList();
            return JsonConvert.SerializeObject(dtos);
        }

        public void ClearRuntime()
        {
            AchievementHistoryModel.map.Clear();
            AchievementHistoryModel.ClearSideIndexes();
        }

        public void RestoreJson(string json)
        {
            List<AchievementSaveDto> dtos = JsonConvert.DeserializeObject<List<AchievementSaveDto>>(json);
            if (dtos == null)
                return;

            foreach (AchievementSaveDto dto in dtos)
            {
                if (dto == null || string.IsNullOrEmpty(dto.key))
                    continue;
                AchievementHistoryModel.FromSave(dto.key, dto.achievementKey);
            }
        }
    }
}
