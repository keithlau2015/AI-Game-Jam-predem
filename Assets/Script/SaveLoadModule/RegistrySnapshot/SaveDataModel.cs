using System;
using System.Collections.Generic;
using UnityEngine;

namespace SaveLoadModule
{
    /// <summary>
    /// Registry-snapshot blob: one slot = one file containing reflective maps of SaveableModel types.
    /// </summary>
    public class SaveDataModel : SaveableModel<SaveDataModel>
    {
        public const int CurrentVersion = 1;
        public const string FilePrefix = "reg_";

        public SaveDataModel(object key) : base(key)
        {
        }

        public SaveDataModel() : base()
        {
        }

        public SaveDataModel(string name) : base()
        {
            this.Name = name;
            this.CreateTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            this.UpdatedUnixMs = this.CreateTime;
            this.Version = CurrentVersion;
            this.LastLevelKey = PlayerPrefs.GetString("LastLevelKey", string.Empty);
            PredictFileSize();
        }

        public int Version { get; set; } = CurrentVersion;
        public string Name { get; set; }
        public long CreateTime { get; set; }
        public long UpdatedUnixMs { get; set; }
        public Int64 FileSize { get; set; }
        public string LastLevelKey { get; set; }

        public SortedDictionary<string, SortedDictionary<object, object>> data =
            new SortedDictionary<string, SortedDictionary<object, object>>();

        public string FileName => $"{FilePrefix}{key}";

        public SaveSlotInfo ToSlotInfo()
        {
            return new SaveSlotInfo
            {
                SlotId = key?.ToString(),
                DisplayName = Name,
                CreatedUnixMs = CreateTime,
                UpdatedUnixMs = UpdatedUnixMs > 0 ? UpdatedUnixMs : CreateTime,
                FileSizeBytes = FileSize,
                LastLevelKey = LastLevelKey,
                Backend = SaveBackendKind.RegistrySnapshot,
                FileName = FileName
            };
        }

        public async void PredictFileSize()
        {
            this.FileSize = await FileManager.PredictObjectSaveSize(new object[] { this });
        }

        public void Touch()
        {
            UpdatedUnixMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            LastLevelKey = PlayerPrefs.GetString("LastLevelKey", string.Empty);
            Version = CurrentVersion;
        }
    }
}
