using System;
using System.Collections.Generic;
using UnityEngine;

namespace SaveLoadModule.DocumentDto
{
    /// <summary>
    /// Shipped-game style save root: one document per slot, sections filled by <see cref="ISaveParticipant"/>.
    /// </summary>
    [Serializable]
    public class SaveGameDocument
    {
        public const int CurrentVersion = 1;
        public const string FilePrefix = "dto_";

        public int version = CurrentVersion;
        public string slotId;
        public string displayName;
        public long createdUnixMs;
        public long updatedUnixMs;
        public string lastLevelKey;
        public long fileSizeBytes;

        /// <summary>sectionId → JSON payload</summary>
        public Dictionary<string, string> sections = new Dictionary<string, string>();

        public string FileName => $"{FilePrefix}{slotId}";

        public static SaveGameDocument CreateNew(string displayName)
        {
            long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            return new SaveGameDocument
            {
                version = CurrentVersion,
                slotId = Guid.NewGuid().ToString("N"),
                displayName = displayName,
                createdUnixMs = now,
                updatedUnixMs = now,
                lastLevelKey = PlayerPrefs.GetString("LastLevelKey", string.Empty),
                sections = new Dictionary<string, string>()
            };
        }

        public SaveSlotInfo ToSlotInfo()
        {
            return new SaveSlotInfo
            {
                SlotId = slotId,
                DisplayName = displayName,
                CreatedUnixMs = createdUnixMs,
                UpdatedUnixMs = updatedUnixMs > 0 ? updatedUnixMs : createdUnixMs,
                FileSizeBytes = fileSizeBytes,
                LastLevelKey = lastLevelKey,
                Backend = SaveBackendKind.DocumentDto,
                FileName = FileName
            };
        }

        public void Touch()
        {
            updatedUnixMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            lastLevelKey = PlayerPrefs.GetString("LastLevelKey", string.Empty);
            version = CurrentVersion;
        }
    }
}
