using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI
{
    public abstract class CommonPopUpPanel : MonoBehaviour, IPreviousablePanel
    {
        [SerializeField]
        private Button[] closeButtons;
        [SerializeField]
        protected Tweener_Scale tweenScale;
        [SerializeField]
        protected Tweener_Alpha tweenAlpha;

        public virtual void Hide()
        {
            foreach (Button button in closeButtons)
            {
                button.interactable = false;
            }
            this.tweenAlpha.SetTween(1, 0);
            this.tweenAlpha.Play();
        }

        public virtual void Show()
        {
            this.tweenScale.SetTween(from: new Vector3(0.4f, 0.4f, 0.4f), to: new Vector3(1, 1, 1));
            this.tweenScale.Play();

            foreach (Button button in this.closeButtons)
            {
                button.interactable = true;
                button.onClick.AddListener(this.Hide);
            }
        }

        private void OnDestroy()
        {
            foreach (Button button in this.closeButtons)
            {
                button.onClick.RemoveAllListeners();
            }
        }
    }
}