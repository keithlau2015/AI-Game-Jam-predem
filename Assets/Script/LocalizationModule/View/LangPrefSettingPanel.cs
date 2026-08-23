using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GenericGameModule;
using LocalizationModule;

namespace GameUI
{
    public class LangPrefSettingPanel : CommonPopUpPanel
    {
        [SerializeField]
        private Transform langPrefOptionAnchor;
        [SerializeField]
        private OneSupportLang oneSupportLangPref;

        private void Start()
        {
            Show();
        }

        public override void Show()
        {
            foreach (LocalizationModel.Language supportLang in Enum.GetValues(typeof(LocalizationModel.Language)))
            {
                OneSupportLang oneSupportLang = Instantiate(oneSupportLangPref, langPrefOptionAnchor);
                oneSupportLang.SetUp(supportLang);
            }
            base.Show();
        }

        public override void Hide()
        {
            tweenAlpha.SetOnCompleteCB(() => { Destroy(gameObject); });
            base.Hide();
        }
    }
}