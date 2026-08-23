using Newtonsoft.Json;
using UnityEngine;

namespace SaveLoadModule.DocumentDto.Participants
{
    public sealed class MetaSaveDto
    {
        public string lastLevelKey;
    }

    /// <summary>
    /// Lightweight prefs / campaign meta mirrored into the document (also stored on the document root).
    /// </summary>
    public sealed class MetaSaveParticipant : ISaveParticipant
    {
        public string SectionId => "meta";

        public string CaptureJson()
        {
            return JsonConvert.SerializeObject(new MetaSaveDto
            {
                lastLevelKey = PlayerPrefs.GetString("LastLevelKey", string.Empty)
            });
        }

        public void ClearRuntime()
        {
        }

        public void RestoreJson(string json)
        {
            MetaSaveDto dto = JsonConvert.DeserializeObject<MetaSaveDto>(json);
            if (dto == null)
                return;
            if (!string.IsNullOrEmpty(dto.lastLevelKey))
                PlayerPrefs.SetString("LastLevelKey", dto.lastLevelKey);
        }
    }
}
