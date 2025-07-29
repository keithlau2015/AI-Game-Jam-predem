using LocalizationModule;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI
{
    public class CommonPopTextPanel : CommonPopUpPanel
    {
        public struct CommonPopUpTextPanelConfig
        {
            public bool showGreenBtn;
            public bool showRedBtn;
            public string greenBtnLabeID;
            public string redBtbLabelID;
        }

        [SerializeField]
        private Text contentLabel, greenBtnLabel, redBtnLabel;
        [SerializeField]
        private Button greenButton, redButton;

        private CommonPopUpTextPanelConfig config;

        public bool isInit { get; private set; } = false;

        public void Show(CommonPopUpTextPanelConfig config, string content, Action onGreenBtnClickCB = null, Action onRedBtbClickCB = null)
        {
            this.config = config;
            if (contentLabel != null)
            {
                contentLabel.text = content;
            }

            greenButton.gameObject.SetActive(config.showGreenBtn);
            if (greenButton != null && config.showGreenBtn)
            {
                if (onGreenBtnClickCB == null)
                    onGreenBtnClickCB = Hide;

                greenButton.onClick.AddListener(() => {
                    onGreenBtnClickCB?.Invoke();
                });
            }

            redButton.gameObject.SetActive(config.showRedBtn);
            if (redButton != null && config.showRedBtn)
            {
                if (onRedBtbClickCB == null)
                    onRedBtbClickCB = Hide;

                redButton.onClick.AddListener(() => {
                    onRedBtbClickCB?.Invoke();
                });
            }

            if (greenBtnLabel != null && config.showGreenBtn)
                greenBtnLabel.text = LocalizationController.singleton.GetLabel(config.greenBtnLabeID);

            if (redBtnLabel != null && config.showRedBtn)
                redBtnLabel.text = LocalizationController.singleton.GetLabel(config.redBtbLabelID);

            isInit = true;

            Show();
            this.gameObject.SetActive(true);
        }

        public override void Hide()
        {
            this.tweenAlpha.SetOnCompleteCB(() => {
                this.gameObject.SetActive(false);
                this.tweenAlpha.SetCanvasGroupAlpha(1);
            });
            greenButton.onClick.RemoveAllListeners();
            redButton.onClick.RemoveAllListeners();
            base.Hide();
        }
    }
}