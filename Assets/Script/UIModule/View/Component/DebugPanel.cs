using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI
{
    public class DebugPanel : CommonPopUpPanel
    {
        [SerializeField]
        private Text contents;
        [SerializeField]
        private InputField inputField;
        [SerializeField]
        private Button request, tab;

        public override void Show()
        {
            base.Show();

            request.onClick.AddListener(() =>
            {
                DebugController.singleton.ExecuteCommand(inputField.text);
                inputField.text = "";
                contents.text = DebugController.log;
            });

            tab.onClick.AddListener(() =>
            {
                inputField.text = DebugController.singleton.Tab(inputField.text);
            });

            this.gameObject.SetActive(true);
        }

        public override void Hide()
        {
            this.tweenAlpha.SetOnCompleteCB(() => {
                this.gameObject.SetActive(false);
                this.tweenAlpha.SetCanvasGroupAlpha(1);
            });
            base.Hide();
        }
    }
}