using GenericGameModule;
using System.Collections.Generic;

namespace LocalizationModule
{
    public class LocalizationModel : Model<LocalizationModel>
    {
        public enum Language
        {
            tw = 1,
            cn = 2,
            en = 3,
            jp = 4,
            kr = 5,
        }
        public string id 
        {
            get 
            {
                return key.ToString();
            }
            set
            {
                map.Remove(key);
                key = value;
                map.TryAdd(key, this);
            }
        }
        public string tw { get; private set; }
        public string cn { get; private set; }
        public string en { get; private set; }
        public string jp { get; private set; }
        public string kr { get; private set; }

        public LocalizationModel(object key) : base(key) { }

        public string GetContent(int lang)
        {
            string content = "";
            switch (lang)
            {
                case (int)Language.tw:
                    content = tw;
                    break;
                case (int)Language.cn:
                    content = cn;
                    break;
                case (int)Language.en:
                    content = en;
                    break;
                case (int)Language.jp:
                    content = jp;
                    break;
                case (int)Language.kr:
                    content = kr;
                    break;
            }

            if (string.IsNullOrEmpty(content))
                content = id;

            return content;
        }
    }
}