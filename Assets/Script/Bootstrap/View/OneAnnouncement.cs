using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI
{
    public class OneAnnouncement : MonoBehaviour
    {
        [SerializeField]
        private Button button;
        [SerializeField]
        private Text titleLabel;

        private string title;
        private string content;

        public void SetUp(string title, string content, Action<string> onClickCB)
        {
            SetUpData(title, content);
            SetUpUI(onClickCB);
        }

        public void SetUpData(string title, string content)
        {
            this.title = title;
            this.content = content;
        }

        public void SetUpUI(Action<string> onClickCB)
        {
            if (button == null)
                return;

            if (titleLabel == null)
                return;

            titleLabel.text = title;
            button.onClick.AddListener(() => onClickCB?.Invoke(content));
        }
    }
}