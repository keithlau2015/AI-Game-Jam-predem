using System;
namespace LocalizationModule
{
    public class LocalizationManager : Singleton<LocalizationManager>
    {
        public event Action OnLanguagePreferenceUpdate;
        private LocalizationModel.Language curLanguage = LocalizationModel.Language.en;

        public string GetLocalization(string localizationKey, params string[] args)
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