using System;
using UnityEngine;
using UnityEngine.UI;
using GenericGameModule;
using LocalizationModule;

namespace GameUI
{
    public class OneSupportLang : MonoBehaviour
    {
        [SerializeField]
        private Image icon, toggle;
        [SerializeField]
        private Text label;
        [SerializeField]
        private Button button;

        private LocalizationModel.Language langID;

        public async void SetUp(LocalizationModel.Language langID)
        {
            this.langID = langID;
            toggle.gameObject.SetActive(langID == LocalizationManager.singleton.GetLanguagePref());
            button.onClick.AddListener(() => {
                if (langID == LocalizationManager.singleton.GetLanguagePref())
                    return;
                LocalizationManager.singleton.SetLanguage(langID);
            });
            label.text = Enum.GetName(typeof(LocalizationModel.Language), langID).ToUpper();
            icon.sprite = await GameAssetsBundleManager.LoadSprite($"lang_{Enum.GetName(typeof(LocalizationModel.Language), langID)}");
            LocalizationManager.singleton.OnLanguagePreferenceUpdate += OnLangPrefChange;
        }

        private void OnDestroy()
        {
            button.onClick.RemoveAllListeners();
            LocalizationManager.singleton.OnLanguagePreferenceUpdate -= OnLangPrefChange;
        }

        private void OnLangPrefChange()
        {
            if (langID == LocalizationManager.singleton.GetLanguagePref())
            {
                toggle.gameObject.SetActive(true);
            }
            else
            {
                toggle.gameObject.SetActive(false);
            }
        }
    }
}