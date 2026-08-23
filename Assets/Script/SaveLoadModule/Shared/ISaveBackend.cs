using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace SaveLoadModule
{
    /// <summary>
    /// Common save/load contract so UI and gameplay talk to one facade.
    /// </summary>
    public interface ISaveBackend
    {
        SaveBackendKind Kind { get; }

        UniTask EnsureCatalogLoaded();

        IReadOnlyList<SaveSlotInfo> ListSlots();

        UniTask<string> CreateSave(string displayName);

        void LoadSave(string slotId, out string errorCode);

        UniTask DeleteSave(string slotId);

        UniTask AutoSave();
    }
}
