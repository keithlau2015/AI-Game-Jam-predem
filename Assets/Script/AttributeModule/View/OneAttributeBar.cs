using AttributeModule;
using Cysharp.Threading.Tasks;
using LocalizationModule;
using Model;
using System;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI
{
    public struct OneAttributeDS
    {
        public enum Type
        {
            ScrollBar = 0,
            Value = 1,
        }

        public OneAttributeDS(Type type = 0, bool showCount = false, bool showName = false, bool showFrame = false, bool showBackground = false, bool showIcon = false)
        {
            this.type = (int)type;
            this.showCount = showCount;
            this.showName = showName;
            this.showFrame = showFrame;
            this.showBackground = showBackground;
            this.showIcon = showIcon;
        }

        public int type;
        public bool showCount;
        public bool showName;
        public bool showFrame;
        public bool showBackground;
        public bool showIcon;
    }
    public class OneAttributeBar : MonoBehaviour
    {
        #region UI Component
        [SerializeField]
        private Image icon, frame, background;
        [SerializeField]
        private Text countLabel, nameLabel, descriptionLabel;
        [SerializeField]
        private Scrollbar scrollbar;
        #endregion

        #region Data
        public AttributeModel ds { get; private set; }
        private AttributeData ins;
        private OneAttributeDS config = new OneAttributeDS();
        public bool isInit { get; private set; } = false;
        #endregion

        public void Init(AttributeModel ds, AttributeData ins, OneAttributeDS config)
        {
            gameObject.SetActive(false);
            SetData(ds, ins, config);
            InitUI();
            gameObject.SetActive(true);
            isInit = true;
        }

        public void SetData(AttributeModel ds, AttributeData ins, OneAttributeDS config)
        {
            this.ds = ds;
            this.ins = ins;
            this.config = config;
        }

        public async void InitUI()
        {
            descriptionLabel.gameObject.SetActive(false);
            countLabel.gameObject.SetActive(false);
            nameLabel.gameObject.SetActive(false);
            icon.gameObject.SetActive(false);
            background.gameObject.SetActive(false);
            frame.gameObject.SetActive(false);
            scrollbar.gameObject.SetActive(false);

            descriptionLabel.text = LocalizationManager.singleton.GetLocalization(ds.descriptionID);

            if (config.type == (int)OneAttributeDS.Type.Value)
            {
                scrollbar.gameObject.SetActive(false);
                countLabel.gameObject.SetActive(true);
            }
            else if(config.type == (int)OneAttributeDS.Type.ScrollBar)
            {
                scrollbar.gameObject.SetActive(true);
                UpdateScrollBar(0, 0, 0, 0);
                //Set Count Centre
                countLabel.transform.SetParent(scrollbar.transform);
                RectTransform rectTransform = countLabel.GetComponent<RectTransform>();
                if (rectTransform == null)
                    return;
                rectTransform.anchoredPosition = scrollbar.GetComponent<RectTransform>().anchoredPosition;
            }

            if (config.showCount)
            {
                countLabel.text = this.ins.value.ToString();
                countLabel.gameObject.SetActive(true);
            }

            if (config.showName)
            {
                nameLabel.text = LocalizationManager.singleton.GetLocalization(ds.nameID);
                nameLabel.gameObject.SetActive(true);
            }

            if (config.showBackground)
            {
                Sprite backgroundSprite = await AssetsBundleManager.LoadSprite(ds.backgroundID);
                if (background != null)
                {
                    background.sprite = backgroundSprite;
                    background.gameObject.SetActive(true);
                }
            }

            if (config.showFrame)
            {
                Sprite frameSprite = await AssetsBundleManager.LoadSprite(ds.frameID);
                if (frame != null)
                {
                    frame.sprite = frameSprite;
                    frame.gameObject.SetActive(true);
                }
            }

            if (config.showIcon)
            {
                Sprite iconSprite = await AssetsBundleManager.LoadSprite(ds.iconID);
                if (icon != null)
                    icon.sprite = iconSprite;
            }

            ins.onValuePostChange += UpdateScrollBar;
        }

        private void UpdateScrollBar(int dir, BigInteger diff, BigInteger value, BigInteger maxValue)
        {
            if (ins.maxValue <= 0)
            {
                scrollbar.value = 0f;
                return;
            }

            float currentValue = (float)ins.value;
            float maxValueFloat = (float)ins.maxValue;
            scrollbar.value = Mathf.Clamp01(currentValue / maxValueFloat);
        }

        public void ShowDescription()
        {
            descriptionLabel.gameObject.SetActive(true);
        }

        private void OnDestroy()
        {
            ins.onValuePostChange -= UpdateScrollBar;
            scrollbar.onValueChanged.RemoveAllListeners();
            ds = null;
            ins = null;
        }
    }
}