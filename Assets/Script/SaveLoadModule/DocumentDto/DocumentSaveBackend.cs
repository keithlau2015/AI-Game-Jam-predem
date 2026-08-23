using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace SaveLoadModule.DocumentDto
{
    /// <summary>
    /// Usual shipped-game save: capture/restore through DTO sections, one document file per slot.
    /// </summary>
    public sealed class DocumentSaveBackend : ISaveBackend
    {
        private bool _catalogLoaded;
        private readonly List<SaveSlotInfo> _slots = new List<SaveSlotInfo>();
        private readonly Dictionary<string, SaveGameDocument> _catalog = new Dictionary<string, SaveGameDocument>();
        private readonly List<ISaveParticipant> _participants = new List<ISaveParticipant>();

        public SaveBackendKind Kind => SaveBackendKind.DocumentDto;

        public DocumentSaveBackend()
        {
            DocumentSaveBootstrap.RegisterDefaultParticipants(_participants);
        }

        public void RegisterParticipant(ISaveParticipant participant)
        {
            if (participant == null || _participants.Any(p => p.SectionId == participant.SectionId))
                return;
            _participants.Add(participant);
        }

        public async UniTask EnsureCatalogLoaded()
        {
            if (_catalogLoaded)
                return;

            _catalog.Clear();
            _slots.Clear();
            FileManager.EnsureInit();

            string dir = FileManager.filePath[(int)FileManager.FileType.Save];
            string[] files = FileManager.GetAllFileInDirectory(dir);
            if (files != null)
            {
                foreach (string fileName in files)
                {
                    if (string.IsNullOrEmpty(fileName) || !fileName.StartsWith(SaveGameDocument.FilePrefix, StringComparison.Ordinal))
                        continue;

                    List<SaveGameDocument> loaded =
                        await FileManager.LoadFile<SaveGameDocument>(FileManager.FileType.Save, fileName);
                    if (loaded == null || loaded.Count == 0 || loaded[0] == null)
                        continue;

                    SaveGameDocument doc = loaded[0];
                    if (string.IsNullOrEmpty(doc.slotId))
                        doc.slotId = fileName.Substring(SaveGameDocument.FilePrefix.Length);

                    _catalog[doc.slotId] = doc;
                }
            }

            RebuildSlotCache();
            _catalogLoaded = true;
        }

        public IReadOnlyList<SaveSlotInfo> ListSlots()
        {
            EnsureCatalogLoaded().GetAwaiter().GetResult();
            return _slots;
        }

        public async UniTask<string> CreateSave(string displayName)
        {
            await EnsureCatalogLoaded();

            SaveGameDocument doc = SaveGameDocument.CreateNew(displayName);
            CaptureInto(doc);
            doc.fileSizeBytes = EstimateSize(doc);
            FileManager.SaveFile(doc, FileManager.FileType.Save, doc.FileName);
            await UniTask.Yield();

            _catalog[doc.slotId] = doc;
            RebuildSlotCache();
            return doc.slotId;
        }

        public void LoadSave(string slotId, out string errorCode)
        {
            errorCode = string.Empty;
            EnsureCatalogLoaded().GetAwaiter().GetResult();

            if (!_catalog.TryGetValue(slotId, out SaveGameDocument doc) || doc == null)
            {
                errorCode = ErrorCode.SaveFileCorrupted;
                return;
            }

            try
            {
                foreach (ISaveParticipant participant in _participants)
                    participant.ClearRuntime();

                foreach (ISaveParticipant participant in _participants)
                {
                    if (doc.sections != null && doc.sections.TryGetValue(participant.SectionId, out string json)
                        && !string.IsNullOrEmpty(json))
                    {
                        participant.RestoreJson(json);
                    }
                }

                if (!string.IsNullOrEmpty(doc.lastLevelKey))
                    PlayerPrefs.SetString("LastLevelKey", doc.lastLevelKey);
            }
            catch (Exception e)
            {
                Debug.LogError($"[DocumentSave] Load failed: {e}");
                errorCode = ErrorCode.SaveFileCorrupted;
            }
        }

        public async UniTask DeleteSave(string slotId)
        {
            await EnsureCatalogLoaded();
            if (_catalog.TryGetValue(slotId, out SaveGameDocument doc) && doc != null)
            {
                FileManager.DeleteFile(FileManager.FileType.Save, doc.FileName);
                _catalog.Remove(slotId);
            }
            else
            {
                FileManager.DeleteFile(FileManager.FileType.Save, $"{SaveGameDocument.FilePrefix}{slotId}");
            }

            RebuildSlotCache();
        }

        public async UniTask AutoSave()
        {
            int autoCount = _catalog.Values.Count(d =>
                d != null && !string.IsNullOrEmpty(d.displayName)
                && d.displayName.StartsWith("AutoSave_", StringComparison.Ordinal));
            await CreateSave($"AutoSave_{autoCount}");
        }

        private void CaptureInto(SaveGameDocument doc)
        {
            doc.Touch();
            doc.sections.Clear();
            foreach (ISaveParticipant participant in _participants)
            {
                try
                {
                    doc.sections[participant.SectionId] = participant.CaptureJson() ?? string.Empty;
                }
                catch (Exception e)
                {
                    Debug.LogError($"[DocumentSave] Capture {participant.SectionId} failed: {e}");
                }
            }
        }

        private void RebuildSlotCache()
        {
            _slots.Clear();
            foreach (SaveGameDocument doc in _catalog.Values.OrderByDescending(d => d.updatedUnixMs > 0 ? d.updatedUnixMs : d.createdUnixMs))
            {
                if (doc == null)
                    continue;
                _slots.Add(doc.ToSlotInfo());
            }
        }

        private static long EstimateSize(SaveGameDocument doc)
        {
            try
            {
                string json = JsonConvert.SerializeObject(doc);
                return json != null ? json.Length : 0;
            }
            catch
            {
                return 0;
            }
        }
    }
}
