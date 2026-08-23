using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI
{
    public class PromptDot : MonoBehaviour
    {
        public enum Mode : int
        {
            icon = 0,
            num = 1,
        }

        [SerializeField]
        private GameObject icon;
        [SerializeField]
        private Text numLabel;

        private PromptDotController controller;
        private Mode mode;
        
        public virtual void SetUp(PromptDotController controller, Mode mode)
        {
            this.mode = mode;
            if (controller != null)
                this.controller = controller;
        }

        private void OnCountChange(int count)
        {
            if(mode == Mode.icon)
            {
                if (numLabel.gameObject.activeInHierarchy)
                    numLabel.gameObject.SetActive(false);

                icon.gameObject.SetActive(true);
            }
            else if(mode == Mode.num)
            {
                if(icon.gameObject.activeInHierarchy)
                    icon.gameObject.SetActive(false);
                numLabel.gameObject.SetActive(true);
                numLabel.text = count.ToString();
            }
        }

        private void OnDestroy()
        {
            if (controller != null)
            {
                controller.onValueChangeEvent -= OnCountChange;
                controller = null;
            }
        }
    }
}