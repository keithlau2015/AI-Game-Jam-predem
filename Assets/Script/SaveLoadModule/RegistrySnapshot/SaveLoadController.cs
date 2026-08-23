using Cysharp.Threading.Tasks;

namespace SaveLoadModule
{
    /// <summary>
    /// Compatibility facade over <see cref="SaveService"/> (keeps existing call sites working).
    /// </summary>
    public static class SaveLoadController
    {
        public static object currentSaveKey
        {
            get => SaveService.CurrentSaveKey;
            set => SaveService.CurrentSaveKey = value;
        }

        public static void LoadSave(string uid, out string errorCode)
        {
            SaveService.LoadSave(uid, out errorCode);
        }

        public static void CreateSave(string name)
        {
            SaveService.CreateSave(name).Forget();
        }

        public static void DeleteSave(string nameOrSlotId)
        {
            SaveService.DeleteSave(nameOrSlotId).Forget();
        }

        public static void AutoSave()
        {
            SaveService.AutoSave().Forget();
        }
    }
}
