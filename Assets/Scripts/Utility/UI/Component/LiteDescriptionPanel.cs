using LocalizationModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;

namespace GameUI
{
    public class LiteDescriptionPanel : MonoBehaviour
    {
        [SerializeField]
        private Text text;
        [SerializeField]
        private Canvas canvas;
        private PlayerControl inputActions;
        private RectTransform rectTransform;
        public void Show(string id)
        {
            if (rectTransform == null)
                if (!TryGetComponent(out rectTransform))
                    return;

            if (inputActions == null)
                inputActions = new PlayerControl();
            if(!inputActions.UI.Point.enabled)
            {
                inputActions.UI.Point.Enable();
                inputActions.UI.Point.performed += FollowCursor;
            }    
            text.text = LocalizationController.singleton.GetLabel(id);
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            text.text = string.Empty;
            gameObject.SetActive(false);
        }

        private void FollowCursor(CallbackContext ctx)
        {
            Vector2 cursorPos = ctx.ReadValue<Vector2>();

            Vector2 uiPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
               canvas.transform as RectTransform,
               cursorPos, canvas.worldCamera,
                out uiPos
            );

            transform.position = canvas.transform.TransformPoint(uiPos);
        }
    }
}