using UnityEngine;

namespace SaveLoadModule
{
    /// <summary>
    /// Optional Resources asset: Resources/SaveLoadSettings.
    /// Controls which backend the template uses at runtime.
    /// </summary>
    [CreateAssetMenu(fileName = "SaveLoadSettings", menuName = "Null Template/Save Load Settings")]
    public class SaveLoadSettings : ScriptableObject
    {
        [Tooltip("RegistrySnapshot keeps the original SaveableModel design. DocumentDto is the usual shipped SaveGame DTO pattern.")]
        public SaveBackendKind activeBackend = SaveBackendKind.RegistrySnapshot;

        public static SaveLoadSettings LoadOrDefault()
        {
            SaveLoadSettings settings = Resources.Load<SaveLoadSettings>("SaveLoadSettings");
            if (settings != null)
                return settings;

            settings = CreateInstance<SaveLoadSettings>();
            settings.activeBackend = SaveBackendKind.RegistrySnapshot;
            return settings;
        }
    }
}
