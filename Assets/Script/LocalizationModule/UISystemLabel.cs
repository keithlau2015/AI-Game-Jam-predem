using UnityEngine;
using UnityEngine.UI;

namespace LocalizationModule
{
    public class UISystemLabel : MonoBehaviour
    {
        public string key;
        private Text label;

        private void Awake()
        {
            if (!TryGetComponent(out label))
                return;

            LocalizationManager.singleton.OnLanguagePreferenceUpdate += UpdateLocalization;

            label.text = LocalizationManager.singleton.GetLocalization(key);
        }

        public void UpdateLabel(string key)
        {
            this.key = key;
            UpdateLocalization();
        }

        private void UpdateLocalization()
        {
            label.text = LocalizationManager.singleton.GetLocalization(key);
        }

        private void OnDestroy()
        {
            if (label == null)
                return;
            if (LocalizationManager.singleton == null)
                return;

            LocalizationManager.singleton.OnLanguagePreferenceUpdate -= UpdateLocalization;
        }
    }
}