using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUI;
using UnityEngine.UI;
using System;
using LocalizationModule;

public class GraphicPrefSettingPanel : CommonPopUpPanel
{
    [SerializeField]
    private Dropdown graphicQuality, resolution, windowMode, framerate, anisotropicFiltering, antiAliasing, shadowQuality, vSync;
    [SerializeField]
    private Button applyBtn, revertBtn;

    private GraphicManager.GraphicSetting tmpGraphicSetting = new GraphicManager.GraphicSetting();

    private void Start()
    {
        Show();
    }

    public override void Show()
    {
        //Default Value
        graphicQuality.value = QualitySettings.GetQualityLevel();
        resolution.value = GraphicManager.singleton.GetResolutionIndex(Screen.currentResolution);
        windowMode.value = (int)Screen.fullScreenMode;
        GraphicManager.FramerateType framerateIndex = GraphicManager.FramerateType.Unlimited;
        Enum.TryParse(Application.targetFrameRate.ToString(), out framerateIndex);
        framerate.value = (int)framerateIndex;
        anisotropicFiltering.value = (int)QualitySettings.anisotropicFiltering;
        antiAliasing.value = GraphicManager.singleton.GetAntiAliasingIndex();
        shadowQuality.value = (int)QualitySettings.shadows;
        vSync.value = QualitySettings.vSyncCount;

        graphicQuality.onValueChanged.AddListener(value => {
            tmpGraphicSetting.qualityLevel = value;
        });

        foreach(Resolution resolution in Screen.resolutions)
        {
            Dropdown.OptionData optionData = new Dropdown.OptionData($"{resolution.width} x {resolution.height}");
            this.resolution.options.Add(optionData);
        }
        resolution.onValueChanged.AddListener(value => {
            tmpGraphicSetting.height = Screen.resolutions[value].height;
            tmpGraphicSetting.width = Screen.resolutions[value].width;
            resolution.captionText.text = $"{Screen.resolutions[value].width} x {Screen.resolutions[value].height}";
        });


        foreach (FullScreenMode fullScreenMode in Enum.GetValues(typeof(FullScreenMode)))
        {
            string label = LocalizationController.singleton.GetLabel($"SYS_WindowMode_{(int)fullScreenMode}");
            Dropdown.OptionData optionData = new Dropdown.OptionData(label);
            windowMode.options.Add(optionData);
        }
        windowMode.onValueChanged.AddListener(value => {
            tmpGraphicSetting.windowMode = value;
            windowMode.captionText.text = LocalizationController.singleton.GetLabel($"SYS_WindowMode_{value}");
        });

        foreach (GraphicManager.FramerateType framerateType in Enum.GetValues(typeof(GraphicManager.FramerateType)))
        {
            string label = "";
            if (framerateType.Equals(GraphicManager.FramerateType.Unlimited))
            {
                label = LocalizationController.singleton.GetLabel("SYS_Unlimited");
            }
            else if (framerateType.Equals(GraphicManager.FramerateType._120))
            {
                label = "120";
            }
            else if (framerateType.Equals(GraphicManager.FramerateType._60))
            {
                label = "60";
            }
            else if (framerateType.Equals(GraphicManager.FramerateType._30))
            {
                label = "30";
            }
            else if (framerateType.Equals(GraphicManager.FramerateType._15))
            {
                label = "15";
            }
            Dropdown.OptionData optionData = new Dropdown.OptionData(label);
            framerate.options.Add(optionData);
        }
        framerate.onValueChanged.AddListener(value => {
            int tragetFramerate = -1;
            if(value == (int)GraphicManager.FramerateType.Unlimited)
            {
                tragetFramerate = -1;
            }
            else if(value == (int)GraphicManager.FramerateType._120)
            {
                tragetFramerate = 120;
            }
            else if(value == (int)GraphicManager.FramerateType._60)
            {
                tragetFramerate = 60;
            }
            else if(value == (int)GraphicManager.FramerateType._30)
            {
                tragetFramerate = 30;
            }
            else if(value == (int)GraphicManager.FramerateType._15)
            {
                tragetFramerate = 15;
            }

            tmpGraphicSetting.targetFrameRate = tragetFramerate;

            string label = "";
            if (tragetFramerate == -1)
            {
                label = LocalizationController.singleton.GetLabel("SYS_Unlimited");
            }
            else if (tragetFramerate == 120)
            {
                label = "120";
            }
            else if (tragetFramerate == 60)
            {
                label = "60";
            }
            else if (tragetFramerate == 30)
            {
                label = "30";
            }
            else if (tragetFramerate == 15)
            {
                label = "15";
            }
            framerate.captionText.text = label;
        });

        foreach (AnisotropicFiltering anisotropicFiltering in Enum.GetValues(typeof(AnisotropicFiltering)))
        {
            Dropdown.OptionData optionData = new Dropdown.OptionData(anisotropicFiltering.ToString());
            this.anisotropicFiltering.options.Add(optionData);
        }
        anisotropicFiltering.onValueChanged.AddListener(value => {
            tmpGraphicSetting.anisotropicFiltering = value;
            anisotropicFiltering.captionText.text = Enum.GetName(typeof(AnisotropicFiltering), value);
        });

        foreach (GraphicManager.AntiAliasingType type in Enum.GetValues(typeof(GraphicManager.AntiAliasingType)))
        {
            string label = "";
            if (type.Equals(GraphicManager.AntiAliasingType.NoMSAA))
            {
                label = LocalizationController.singleton.GetLabel("SYS_Close");
            }
            else if (type.Equals(GraphicManager.AntiAliasingType._2))
            {
                label = "2";
            }
            else if (type.Equals(GraphicManager.AntiAliasingType._4))
            {
                label = "4";
            }
            else if (type.Equals(GraphicManager.AntiAliasingType._8))
            {
                label = "8";
            }
            Dropdown.OptionData optionData = new Dropdown.OptionData(label);
            antiAliasing.options.Add(optionData);
        }
        antiAliasing.onValueChanged.AddListener(value => {
            int antiAliasing = -1;
            if (value == (int)GraphicManager.AntiAliasingType.NoMSAA)
            {
                antiAliasing = 0;
            }
            else if (value == (int)GraphicManager.AntiAliasingType._2)
            {
                antiAliasing = 2;
            }
            else if (value == (int)GraphicManager.AntiAliasingType._4)
            {
                antiAliasing = 4;
            }
            else if (value == (int)GraphicManager.AntiAliasingType._8)
            {
                antiAliasing = 8;
            }

            if (antiAliasing == -1)
                return;
            tmpGraphicSetting.antiAliasing = antiAliasing;
            string label = antiAliasing.ToString();
            if(value == 0)
                label = LocalizationController.singleton.GetLabel("SYS_Close");
            this.antiAliasing.captionText.text = label;
        });


        foreach (ShadowQuality shadowQuality in Enum.GetValues(typeof(ShadowQuality)))
        {
            string label = LocalizationController.singleton.GetLabel($"SYS_SQ_{(int)shadowQuality}");
            Dropdown.OptionData optionData = new Dropdown.OptionData(label);
            this.shadowQuality.options.Add(optionData);
        }
        shadowQuality.onValueChanged.AddListener(value => {
            tmpGraphicSetting.shadowQuality = value;
            shadowQuality.captionText.text = LocalizationController.singleton.GetLabel($"SYS_SQ_{value}");
        });

        for (int i = 0; i < 4; i++)
        {
            string label = i.ToString();
            if (i == 0)
                label = LocalizationController.singleton.GetLabel("SYS_Close");
            Dropdown.OptionData optionData = new Dropdown.OptionData(label);
            this.vSync.options.Add(optionData);
        }
        vSync.onValueChanged.AddListener(value => {
            tmpGraphicSetting.vSync = value;
            string label = value.ToString();
            if (value == 0)
                label = LocalizationController.singleton.GetLabel("SYS_Close");
            vSync.captionText.text = label;
        });

        applyBtn.onClick.AddListener(() => {
            ApplyChanges();
        });

        revertBtn.onClick.AddListener(() => {
            RevertChange();
        });

        base.Show();
    }

    private void RevertChange()
    {
        tmpGraphicSetting = null;
        tmpGraphicSetting = new GraphicManager.GraphicSetting();
        Hide();
    }

    private void ApplyChanges()
    {
        GraphicManager.singleton.SetAllGraphicConfig(tmpGraphicSetting);
        tmpGraphicSetting = null;
        tmpGraphicSetting = new GraphicManager.GraphicSetting();
        Hide();
    }

    public override void Hide()
    {
        if (!GraphicManager.singleton.graphicSettingConfig.Equals(tmpGraphicSetting))
        {
            UIManager.singleton.ShowCommonPopUpTextPanel(
                true,
                new CommonPopTextPanel.CommonPopUpTextPanelConfig() { showGreenBtn = true, showRedBtn = true, greenBtnLabeID = "SYS_Apply", redBtbLabelID = "SYS_Revert" },
                LocalizationController.singleton.GetLabel("SYS_ConfirmApplySetting"),
                () => { ApplyChanges(); UIManager.singleton.ShowCommonPopUpTextPanel(false); tweenAlpha.SetOnCompleteCB(() => Destroy(this.gameObject)); base.Hide(); },
                () => { RevertChange(); UIManager.singleton.ShowCommonPopUpTextPanel(false); }
            );
        }
        else
        {
            tweenAlpha.SetOnCompleteCB(() => Destroy(this.gameObject));
            base.Hide();
        }
    }

    private void OnDestroy()
    {
        resolution.onValueChanged.RemoveAllListeners();
        windowMode.onValueChanged.RemoveAllListeners();
        framerate.onValueChanged.RemoveAllListeners();
        anisotropicFiltering.onValueChanged.RemoveAllListeners();
        antiAliasing.onValueChanged.RemoveAllListeners();
        shadowQuality.onValueChanged.RemoveAllListeners();
        vSync.onValueChanged.RemoveAllListeners();
        applyBtn.onClick.RemoveAllListeners();
        revertBtn.onClick.RemoveAllListeners();
    }
}
