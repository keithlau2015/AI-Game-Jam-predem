using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using SaveLoadModule.DocumentDto;
using SaveLoadModule.RegistrySnapshot;
using UnityEngine;

namespace SaveLoadModule
{
    /// <summary>
    /// Game-facing facade. Pick a backend via <see cref="SaveLoadSettings"/> or <see cref="SetBackend"/>.
    /// </summary>
    public static class SaveService
    {
        private static ISaveBackend _backend;
        private static bool _initialized;
        private static SaveBackendKind _kind = SaveBackendKind.RegistrySnapshot;

        public static object CurrentSaveKey { get; set; }

        public static SaveBackendKind ActiveBackendKind => _kind;

        public static ISaveBackend Backend
        {
            get
            {
                EnsureInitialized();
                return _backend;
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void AutoBoot()
        {
            EnsureInitialized();
        }

        public static void EnsureInitialized()
        {
            if (_initialized && _backend != null)
                return;

            SaveLoadSettings settings = SaveLoadSettings.LoadOrDefault();
            SetBackend(settings.activeBackend);
        }

        public static void SetBackend(SaveBackendKind kind)
        {
            _kind = kind;
            _backend = kind == SaveBackendKind.DocumentDto
                ? (ISaveBackend)new DocumentSaveBackend()
                : new RegistrySaveBackend();
            _initialized = true;
            _backend.EnsureCatalogLoaded().Forget();
            Debug.Log($"[SaveService] Active backend: {kind}");
        }

        public static UniTask EnsureCatalogLoaded() => Backend.EnsureCatalogLoaded();

        public static IReadOnlyList<SaveSlotInfo> ListSlots() => Backend.ListSlots();

        public static async UniTask<string> CreateSave(string displayName)
        {
            string slotId = await Backend.CreateSave(displayName);
            if (!string.IsNullOrEmpty(slotId))
                CurrentSaveKey = slotId;
            return slotId;
        }

        public static void LoadSave(string slotId, out string errorCode)
        {
            Backend.LoadSave(slotId, out errorCode);
            if (string.IsNullOrEmpty(errorCode))
                CurrentSaveKey = slotId;
        }

        public static UniTask DeleteSave(string slotId) => Backend.DeleteSave(slotId);

        public static UniTask AutoSave() => Backend.AutoSave();
    }
}
