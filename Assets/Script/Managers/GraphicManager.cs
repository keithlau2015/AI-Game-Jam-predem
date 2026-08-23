using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GraphicManager : Singleton<GraphicManager>
{
    public enum FramerateType : int
    {
        Unlimited = 0,
        _120 = 1,
        _60 = 2,
        _30 = 3,
        _15 = 4,
    }

    public enum AntiAliasingType : int
    {
        NoMSAA = 0,
        _2 = 1,
        _4 = 2,
        _8 = 3,
    }

    public class GraphicSetting
    {
        public int height;
        public int width;
        public int windowMode;
        public int qualityLevel;
        public int anisotropicFiltering;
        public int antiAliasing;
        public float LODDistance;
        public int shadowQuality;
        public int vSync;
        public int targetFrameRate;

        /*
        public GraphicSetting()
        {

            this.height = Screen.currentResolution.height;
            this.width = Screen.currentResolution.width;
            this.windowMode = (int)FullScreenMode.MaximizedWindow;
            this.qualityLevel = QualitySettings.GetQualityLevel();
            this.anisotropicFiltering = (int)QualitySettings.anisotropicFiltering;
            this.antiAliasing = QualitySettings.antiAliasing;
            this.LODDistance = QualitySettings.lodBias;
            this.shadowQuality = (int)QualitySettings.shadows;
            this.vSync = QualitySettings.vSyncCount;
            this.targetFrameRate = Application.targetFrameRate;
        }
        */

        public override bool Equals(object obj)
        {
            bool result = false;
            if (!obj.GetType().IsEquivalentTo(typeof(GraphicSetting)))
                return result;

            GraphicSetting other = (GraphicSetting)obj;
            result = (other.height == this.height) && (other.width == this.width) && (other.windowMode == this.windowMode) && (other.qualityLevel == this.qualityLevel) && (other.anisotropicFiltering == this.anisotropicFiltering) && (other.antiAliasing == this.antiAliasing) && (other.LODDistance == this.LODDistance) && (other.shadowQuality == this.shadowQuality) && (other.vSync == this.vSync) && (other.targetFrameRate == this.targetFrameRate);
            return result;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public GraphicSetting graphicSettingConfig { get; private set; }

    public async void SetUp(List<GraphicSetting> graphicSettings)
    {
        if (graphicSettings == null || graphicSettings.Count == 0)
            graphicSettingConfig = new GraphicSetting();
        else
            this.graphicSettingConfig = graphicSettings[0];

        SetAllGraphicConfig(graphicSettingConfig);
        await new WaitUntil(() => { return this.graphicSettingConfig != null; });
    }

#if UNITY_ANDROID || UNITY_IOS

#else
    public void SetWindowMode(FullScreenMode fullScreenMode)
    {
        Screen.fullScreenMode = fullScreenMode;
        this.graphicSettingConfig.windowMode = (int)fullScreenMode;
    }

    public void SetResolution(int height, int width, bool isFullScreen)
    {
        Screen.SetResolution(height, width, isFullScreen);
        this.graphicSettingConfig.height = height;
        this.graphicSettingConfig.width = width;
    }

    private bool IsWindowModeFullScreen(int windowMode)
    {
        if (windowMode == (int)FullScreenMode.FullScreenWindow)
            return true;

        return false;
    }

    public void SetVSync(int vSyncCount)
    {
        if (vSyncCount < 0 && vSyncCount > 4)
            return;

        QualitySettings.vSyncCount = vSyncCount;
        this.graphicSettingConfig.vSync = vSyncCount;
    }
#endif

    public void SetQualityLevel(int levelIndex)
    {
        QualitySettings.SetQualityLevel(levelIndex);
        this.graphicSettingConfig.qualityLevel = levelIndex;
    }

    public void SetLODDistance(float distance)
    {
        QualitySettings.lodBias = distance;
        this.graphicSettingConfig.LODDistance = distance;
    }

    public void SetTargetFrameRate(int frameRate)
    {
        Application.targetFrameRate = frameRate;
        this.graphicSettingConfig.targetFrameRate = frameRate;
    }

    public void SetTextureDetails(AnisotropicFiltering anisotropicFiltering)
    {
        QualitySettings.anisotropicFiltering = anisotropicFiltering;
        this.graphicSettingConfig.anisotropicFiltering = (int)anisotropicFiltering;
    }

    public void SetAllGraphicConfig(GraphicSetting graphicSetting)
    {
        if (graphicSetting.qualityLevel > 0 && graphicSetting.qualityLevel < QualitySettings.names.Length)
            QualitySettings.SetQualityLevel(graphicSetting.qualityLevel);
        if (graphicSetting.LODDistance > 0)
            QualitySettings.lodBias = graphicSetting.LODDistance;
        if (graphicSetting.targetFrameRate > 0)
            Application.targetFrameRate = graphicSetting.targetFrameRate;
        if(graphicSetting.anisotropicFiltering > 0 && graphicSetting.anisotropicFiltering < Enum.GetValues(typeof(AnisotropicFiltering)).Length)
            QualitySettings.anisotropicFiltering = (AnisotropicFiltering)graphicSetting.anisotropicFiltering;
        if (graphicSetting.antiAliasing == 0 || graphicSetting.antiAliasing == 2 || graphicSetting.antiAliasing == 4 || graphicSetting.antiAliasing == 8)
            QualitySettings.antiAliasing = graphicSetting.antiAliasing;
        if (graphicSetting.shadowQuality > 0)
            QualitySettings.shadows = (ShadowQuality)graphicSetting.shadowQuality;
#if UNITY_ANDROID || UNITY_IOS

#else
        if (graphicSetting.height > 0 && graphicSetting.width > 0)
            Screen.SetResolution(graphicSetting.height, graphicSetting.width, IsWindowModeFullScreen(graphicSetting.windowMode));
        if (graphicSetting.windowMode > 0 && graphicSetting.windowMode < 4)
            Screen.fullScreenMode = (FullScreenMode)graphicSetting.windowMode;
#endif
        this.graphicSettingConfig = graphicSetting;
    }

    public int GetResolutionIndex(Resolution resolution)
    {
        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            if (resolution.Equals(Screen.currentResolution))
                return i;
        }

        return -1;
    }

    public int GetAntiAliasingIndex()
    {
        if(QualitySettings.antiAliasing == 0)
        {
            return 0;
        }
        else if (QualitySettings.antiAliasing == 2)
        {
            return 1;
        }
        else if (QualitySettings.antiAliasing == 4)
        {
            return 2;
        }
        else if (QualitySettings.antiAliasing == 8)
        {
            return 3;
        }

        return -1;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        //SaveFile Config
        FileManager.SaveFile<GraphicSetting>(new GraphicSetting[] { graphicSettingConfig }, FileManager.FileType.Save, "GraphicPreference");
    }
}
