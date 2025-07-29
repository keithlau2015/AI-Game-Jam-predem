using System;
using UnityEngine;
using UnityEngine.EventSystems;
namespace GameUI
{
    public class DropSlot : MonoBehaviour, IDropHandler
    {
        public event Action onDropEvent;

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag != null)
            {
                eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
                onDropEvent?.Invoke();
            }
        }
    }
}