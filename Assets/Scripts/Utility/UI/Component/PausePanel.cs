using UnityEngine;
using UnityEngine.UI;
namespace GameUI
{
    public class PausePanel : MonoBehaviour, IPreviousablePanel
    {
        [SerializeField]
        private Button exitBtn, resumeBtn;
        [SerializeField]
        private Tweener_Alpha alphaTween;

        private void Awake()
        {

        }

        public void Hide()
        {
            alphaTween.SetTween(1, 0);
            alphaTween.SetOnCompleteCB(() => this.gameObject.SetActive(false));
            alphaTween.Play();
        }

        public void Show()
        {
            this.gameObject.SetActive(true);
            alphaTween.SetTween(0, 1);
            alphaTween.SetOnCompleteCB(null);
            alphaTween.Play();
        }
    }
}