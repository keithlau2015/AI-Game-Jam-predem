using System;
using UnityEngine;
namespace LocalizationModule
{
    public class LocalizationController : Singleton<LocalizationController>
    {
        public event Action OnLanguagePreferenceUpdate;
        private LocalizationModel.Language curLanguage = LocalizationModel.Language.en;

        public string GetLabel(string localizationKey, params string[] args)
        {
            string result = localizationKey;
            if (LocalizationModel.map.ContainsKey(localizationKey.Trim()))
            {
                LocalizationModel model = (LocalizationModel)LocalizationModel.map[localizationKey.Trim()];
                result = model.GetContent((int)curLanguage);
                if (args.Length > 0 && args != null)
                    result = string.Format(result, args);
                result = result.Replace("\\n", "\n");
            }
            return result;
        }

        public Sprite GetSprite(string localizationKey)
        {
            Sprite result = null;
            if (LocalizationModel.map.ContainsKey(localizationKey.Trim()))
            {
                LocalizationModel model = (LocalizationModel)LocalizationModel.map[localizationKey.Trim()];
                string resultKey = model.GetContent((int)curLanguage);
                result = AssetsBundleManager.LoadSprite(resultKey).GetAwaiter().GetResult();
            }
            return result;
        }

        public void SetLanguage(LocalizationModel.Language language)
        {
            this.curLanguage = language;
            OnLanguagePreferenceUpdate?.Invoke();
        }

        public LocalizationModel.Language GetLanguagePref()
        {
            return this.curLanguage;
        }
    }
}