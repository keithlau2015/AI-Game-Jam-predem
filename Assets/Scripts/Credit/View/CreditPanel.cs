using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI
{
    public class CreditPanel : MonoBehaviour, IPreviousablePanel
    {
        [SerializeField]
        private GameObject anchor;
        [SerializeField]
        private Button closeBtn;
        [SerializeField]
        private Tweener_AnchorPosition tweener;
        [SerializeField]
        private CreditItem creditItemPrefab;

        public void Hide()
        {
            tweener.Stop();
            Destroy(gameObject);
        }

        public void Show()
        {
            
        }

        private void Awake()
        {
            if (tweener == null)
                TryGetComponent(out tweener);
        }

        private void Start()
        {
            if (tweener)
            {
                tweener.SetOnCompleteCB(() => Hide());
                tweener.Play();
            }

            if (closeBtn)
            {
                closeBtn.onClick.AddListener(Hide);
            }


            foreach (List<CreditModel> list in CreditModel.grpNameMap.Values)
            {
                foreach (CreditModel creditModel in list)
                {
                    GameObject item = Instantiate(creditItemPrefab.gameObject, anchor.transform);
                    CreditItem creditItem = item.GetComponent<CreditItem>();
                    if (creditItem == null)
                    {
                        Debug.LogError("CreditItem component not found on the prefab.");
                        continue;
                    }
                    creditItem.SetCreditItem(creditModel);
                }                
            }
        }

        private void OnDestroy()
        {
            closeBtn.onClick.RemoveAllListeners();
        }
    }
}