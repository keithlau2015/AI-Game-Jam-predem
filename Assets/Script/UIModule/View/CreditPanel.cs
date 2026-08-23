using UnityEngine;
using UnityEngine.UI;

namespace GameUI
{
    /// <summary>
    /// Minimal credits panel stub for settings menu wiring.
    /// Replace content per project as needed.
    /// </summary>
    public class CreditPanel : MonoBehaviour, IPreviousablePanel
    {
        [SerializeField]
        private Button closeBtn;
        [SerializeField]
        private Tweener_AnchorPosition tweener;

        public void Hide()
        {
            if (tweener != null)
                tweener.Stop();
            Destroy(gameObject);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        private void Awake()
        {
            if (tweener == null)
                TryGetComponent(out tweener);
        }

        private void Start()
        {
            if (tweener != null)
            {
                tweener.SetOnCompleteCB(() => Hide());
                tweener.Play();
            }

            if (closeBtn != null)
                closeBtn.onClick.AddListener(Hide);
        }

        private void OnDestroy()
        {
            if (closeBtn != null)
                closeBtn.onClick.RemoveAllListeners();
        }
    }
}
