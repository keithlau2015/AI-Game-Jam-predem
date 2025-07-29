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

            LocalizationController.singleton.OnLanguagePreferenceUpdate += UpdateLocalization;

            label.text = LocalizationController.singleton.GetLabel(key);
        }

        public void UpdateLabel(string key)
        {
            this.key = key;
            UpdateLocalization();
        }

        private void UpdateLocalization()
        {
            label.text = LocalizationController.singleton.GetLabel(key);
        }

        private void OnDestroy()
        {
            if (label == null)
                return;
            if (LocalizationController.singleton == null)
                return;

            LocalizationController.singleton.OnLanguagePreferenceUpdate -= UpdateLocalization;
        }
    }
}