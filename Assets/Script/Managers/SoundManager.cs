using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    private const int MAX_BGM_SOURCE = 1;
    private const int MAX_SFX_SOURCE = 10;
    private const int MAX_AMBIENT_SOURCE = 5;
    
    public enum Type : int
    {
        SFX = 0,
        BGM = 1,
        Ambient = 2,
    }

    public class SoundSetting
    {
        public float master;
        public float soundEffect;
        public float bgm;
        public float ambient;
        public bool muteSFX;
        public bool muteBGM;
        public bool muteAmbient;

        public SoundSetting()
        {
            this.master = 1f;
            this.soundEffect = 0.5f;
            this.bgm = 0.4f;
            this.ambient = 0.6f;
            this.muteSFX = false;
            this.muteBGM = false;
            this.muteAmbient = false;
        }
    }

    public bool isInit { get; private set; }

    public event Action<float> onMasterVolumeChange;
    public event Action<float> onSFXVolumeChange;
    public event Action<float> onBGMVolumeChange;
    public event Action<float> onAmbientVolumeChange;
    public event Action<bool> onMuteMaster;
    public event Action<bool> onMuteSFX;
    public event Action<bool> onMuteBGM;
    public event Action<bool> onMuteAmbient;

    private Dictionary<int, List<AudioSource>> audioSourceByTag = new Dictionary<int, List<AudioSource>>();

    private SoundSetting soundSettingConfig;
    public bool IsMasterMute { get { return soundSettingConfig.muteSFX && soundSettingConfig.muteBGM && soundSettingConfig.muteAmbient; } }
    public bool IsBGMMute { get { return soundSettingConfig.muteBGM; } }
    public bool IsAmbientMute { get { return soundSettingConfig.muteAmbient; } }
    public bool IsSFXMute { get { return soundSettingConfig.muteSFX; } }
    public void SetUp(List<SoundSetting> soundSetting = null)
    {
        if (soundSetting == null || soundSetting.Count == 0)
            soundSettingConfig = new SoundSetting();
        else
            soundSettingConfig = soundSetting[0];

        audioSourceByTag.Clear();

        GameObject bgmParent = new GameObject("BGM");
        bgmParent.transform.SetParent(transform);
        GameObject sfxParent = new GameObject("SFX");
        sfxParent.transform.SetParent(transform);
        GameObject ambientParent = new GameObject("Ambient");
        ambientParent.transform.SetParent(transform);

        for (int i = 0; i < MAX_BGM_SOURCE; i++)
        {
            CreateAudioSource((int)Type.BGM).transform.SetParent(bgmParent.transform);
        }

        for (int i = 0; i < MAX_SFX_SOURCE; i++)
        {
            CreateAudioSource((int)Type.SFX).transform.SetParent(sfxParent.transform);
        }

        for (int i = 0; i < MAX_AMBIENT_SOURCE; i++)
        {
            CreateAudioSource((int)Type.Ambient).transform.SetParent(ambientParent.transform);
        }

        if (soundSettingConfig.muteSFX)
            MuteSFX();
        if (soundSettingConfig.muteBGM)
            MuteBGM();

        this.isInit = true;
    }

    #region Set
    public void SetMasterVolume(float newVolume)
    {
        if (!isInit)
            return;

        if (newVolume > 1)
            newVolume = 1;

        onMasterVolumeChange?.Invoke(newVolume);
        soundSettingConfig.master = newVolume;
    }

    public void SetSoundEffectVolume(float newVolume)
    {
        if (!isInit)
            return;

        if (newVolume > 1)
            newVolume = 1;

        onSFXVolumeChange?.Invoke(newVolume);
        soundSettingConfig.soundEffect = newVolume;
    }

    public void SetBGMVolume(float newVolume)
    {
        if (!isInit)
            return;

        if (newVolume > 1)
            newVolume = 1;

        onBGMVolumeChange?.Invoke(newVolume);
        soundSettingConfig.bgm = newVolume;
    }

    public void SetAmbientVolume(float newVolume)
    {
        if (!isInit)
            return;

        if (newVolume > 1)
            newVolume = 1;

        onAmbientVolumeChange?.Invoke(newVolume);
        soundSettingConfig.ambient = newVolume;
    }

    public void MuteAll()
    {
        if (!isInit)
            return;

        foreach (List<AudioSource> audioSourceList in audioSourceByTag.Values)
        {
            foreach(AudioSource audioSource in audioSourceList)
            {
                audioSource.mute = true;
            }
        }
        this.onMuteMaster?.Invoke(true);
        soundSettingConfig.muteBGM = true;
        soundSettingConfig.muteSFX = true;
        soundSettingConfig.muteAmbient = true;
    }

    public void UnMuteAll()
    {
        if (!isInit)
            return;

        foreach (List<AudioSource> audioSourceList in audioSourceByTag.Values)
        {
            foreach (AudioSource audioSource in audioSourceList)
            {
                audioSource.mute = false;
            }
        }
        this.onMuteMaster?.Invoke(false);
        soundSettingConfig.muteBGM = false;
        soundSettingConfig.muteSFX = false;
        soundSettingConfig.muteAmbient = false;
    }

    public void MuteBGM()
    {
        if (!isInit)
            return;

        List<AudioSource> audioSourceList = null;
        if (!audioSourceByTag.TryGetValue((int)Type.BGM, out audioSourceList))
            return;

        if (audioSourceList == null || audioSourceList.Count == 0)
            return;

        foreach(AudioSource audioSource in audioSourceList)
        {
            audioSource.mute = true;
        }

        this.onMuteBGM?.Invoke(true);
        soundSettingConfig.muteBGM = true;
    }
    public void UnMuteBGM()
    {
        if (!isInit)
            return;

        List<AudioSource> audioSourceList = null;
        if (!audioSourceByTag.TryGetValue((int)Type.BGM, out audioSourceList))
            return;

        if (audioSourceList == null || audioSourceList.Count == 0)
            return;

        foreach (AudioSource audioSource in audioSourceList)
        {
            audioSource.mute = false;
        }
        this.onMuteBGM?.Invoke(false);
        soundSettingConfig.muteBGM = false;
    }

    public void MuteSFX()
    {
        if (!isInit)
            return;

        List<AudioSource> audioSourceList = null;
        if (!audioSourceByTag.TryGetValue((int)Type.SFX, out audioSourceList))
            return;

        foreach (AudioSource audioSource in audioSourceList)
        {
            audioSource.mute = true;
        }
        onMuteSFX?.Invoke(true);
        soundSettingConfig.muteSFX = true;
    }

    public void UnMuteSFX()
    {
        if (!isInit)
            return;

        List<AudioSource> audioSourceList = null;
        if (!audioSourceByTag.TryGetValue((int)Type.SFX, out audioSourceList))
            return;

        foreach (AudioSource audioSource in audioSourceList)
        {
            audioSource.mute = false;
        }
        onMuteSFX?.Invoke(false);
        soundSettingConfig.muteSFX = false;
    }

    public void MuteAmbient()
    {
        if (!isInit)
            return;

        List<AudioSource> audioSourceList = null;
        if (!audioSourceByTag.TryGetValue((int)Type.Ambient, out audioSourceList))
            return;

        foreach (AudioSource audioSource in audioSourceList)
        {
            audioSource.mute = true;
        }
        onMuteAmbient?.Invoke(true);
        soundSettingConfig.muteAmbient = true;
    }

    public void UnmuteAmbient()
    {
        if (!isInit)
            return;

        List<AudioSource> audioSourceList = null;
        if (!audioSourceByTag.TryGetValue((int)Type.Ambient, out audioSourceList))
            return;

        foreach (AudioSource audioSource in audioSourceList)
        {
            audioSource.mute = true;
        }
        onMuteAmbient?.Invoke(false);
        soundSettingConfig.muteAmbient = false;
    }
    #endregion

    #region Get
    private float GetVolumeByTag(int tag)
    {
        float result = soundSettingConfig.master;
        switch (tag)
        {
            case (int)Type.BGM:
                result = GetBGM();
                break;
            case (int)Type.SFX:
                result = GetSoundEffect();
                break;
            case (int)Type.Ambient:
                result = GetAmbient();
                break;
        }
        return result;
    }
    public float GetMasterVolume()
    {
        return soundSettingConfig.master;
    }

    public float GetSoundEffect()
    {
        return soundSettingConfig.soundEffect * soundSettingConfig.master;
    }

    public float GetBGM()
    {
        return soundSettingConfig.bgm * soundSettingConfig.master;
    }

    public float GetAmbient()
    {
        return soundSettingConfig.ambient * soundSettingConfig.ambient;
    }
    #endregion

    #region Play
    public async void Play(int tag, string id)
    {
        if (!isInit)
            return;

        List<AudioSource> audioSourceList = null;
        if (!audioSourceByTag.TryGetValue(tag, out audioSourceList))
            return;

        AudioClip audioClip = await GameAssetsBundleManager.LoadAudio(id);

        AudioSource availableAudioSource = null;
        foreach (AudioSource audioSource in audioSourceList)
        {
            if (audioSource.isPlaying)
                continue;

            availableAudioSource = audioSource;
        }

        availableAudioSource.volume = GetVolumeByTag(tag);
        availableAudioSource.clip = audioClip;
        availableAudioSource.Play();
    }
    #endregion

    private AudioSource CreateAudioSource(int type)
    {
        GameObject audioSourceGO = new GameObject($"AudioSource_{type}");
        AudioSource audioSource = audioSourceGO.AddComponent<AudioSource>();
        List<AudioSource> list = null;
        if(!audioSourceByTag.TryGetValue(type, out list))
        {
            list = new List<AudioSource>();
            audioSourceByTag.Add(type, list);
        }
        list.Add(audioSource);
        return audioSource;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        onBGMVolumeChange = null;
        onMasterVolumeChange = null;
        onMuteBGM = null;
        onMuteMaster = null;
        onMuteSFX = null;
        onSFXVolumeChange = null;

        //SaveFile Config
        FileManager.SaveFile<SoundSetting>(new SoundSetting[] { soundSettingConfig }, FileManager.FileType.Save, "SoundPreference");
    }
}