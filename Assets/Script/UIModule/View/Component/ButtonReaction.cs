using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameUI.LittleElements
{
    [RequireComponent(typeof(Button))]
    public class ButtonReaction : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
    {
        [SerializeField]
        private Vector3 iconMagnifier = new Vector3(0, 0, 0);
        [SerializeField]
        private Color selectedColor;
        private Color originColor;
        [SerializeField]
        private Image icon, colorSwitchTarget;
        [SerializeField]
        private Sprite onClickSprite;
        private Sprite originSprite;
        public bool isSelected { get; private set; }

        private void Awake()
        {
            if (icon == null)
                TryGetComponent(out icon);

            if (colorSwitchTarget == null)
                return;

            originSprite = icon.sprite;
            originColor = colorSwitchTarget.color;
        }

        public void OnSelected()
        {
            if (isSelected)
                return;

            isSelected = true;
            if (onClickSprite != null)
                icon.sprite = onClickSprite;
            if (colorSwitchTarget != null)
                colorSwitchTarget.color = selectedColor;
            if (iconMagnifier.x == 0 && iconMagnifier.y == 0 && iconMagnifier.z == 0)
                return;
            this.icon.transform.localScale += iconMagnifier;
        }

        public void OnDeSelected()
        {
            if (!isSelected)
                return;

            isSelected = false;
            if (originSprite != null)
                icon.sprite = originSprite;
            if (colorSwitchTarget != null)
                colorSwitchTarget.color = originColor;
            if (iconMagnifier.x == 0 && iconMagnifier.y == 0 && iconMagnifier.z == 0)
                return;
            this.icon.transform.localScale -= iconMagnifier;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            OnDeSelected();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnSelected();
        }
    }
}