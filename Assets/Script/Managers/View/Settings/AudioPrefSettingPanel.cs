using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUI;
using UnityEngine.UI;

public class AudioPrefSettingPanel : CommonPopUpPanel
{
    [SerializeField]
    private Slider masterVolumeSlider, bgmVolumeSlider, ambientVolumeSlider, sfxVolumeSlider;
    [SerializeField]
    private Button muteMasterBtn, muteBGMBtn, muteAmbientBtn, muteSFXBtn;
    [SerializeField]
    private GameObject muteMasterIcon, muteBGMIcon, muteAmbientIcon, muteSFXIcon;
    [SerializeField]
    private Text masterVolumeLabel, bgmVolumeLabel, ambientVolumeLabel, sfxVolumeLabel;

    private void Start()
    {
        Show();
    }

    public override void Show()
    {
        masterVolumeLabel.text = $"{Mathf.CeilToInt((SoundManager.singleton.GetMasterVolume() / 1) * 100)}%";
        bgmVolumeLabel.text = $"{Mathf.CeilToInt((SoundManager.singleton.GetBGM() / 1) * 100)}%";
        ambientVolumeLabel.text = $"{Mathf.CeilToInt((SoundManager.singleton.GetAmbient() / 1) * 100)}%";
        sfxVolumeLabel.text = $"{Mathf.CeilToInt((SoundManager.singleton.GetSoundEffect() / 1) * 100)}%";

        masterVolumeSlider.value = SoundManager.singleton.GetMasterVolume();
        bgmVolumeSlider.value = SoundManager.singleton.GetBGM();
        ambientVolumeSlider.value = SoundManager.singleton.GetAmbient();
        sfxVolumeSlider.value = SoundManager.singleton.GetSoundEffect();

        masterVolumeSlider.onValueChanged.AddListener(value => {
            masterVolumeLabel.text = $"{Mathf.CeilToInt((value / 1) * 100)}%";
            SoundManager.singleton.SetMasterVolume(value);
        });

        bgmVolumeSlider.onValueChanged.AddListener(value => {
            bgmVolumeLabel.text = $"{Mathf.CeilToInt((value / 1) * 100)}%";
            SoundManager.singleton.SetBGMVolume(value);
        });

        ambientVolumeSlider.onValueChanged.AddListener(value => {
            ambientVolumeLabel.text = $"{Mathf.CeilToInt((value / 1) * 100)}%";
            SoundManager.singleton.SetAmbientVolume(value);
        });

        sfxVolumeSlider.onValueChanged.AddListener(value => {
            sfxVolumeLabel.text = $"{Mathf.CeilToInt((value / 1) * 100)}%";
            SoundManager.singleton.SetSoundEffectVolume(value);
        });

        muteMasterIcon.SetActive(SoundManager.singleton.IsMasterMute);
        muteBGMIcon.SetActive(SoundManager.singleton.IsBGMMute);
        muteAmbientIcon.SetActive(SoundManager.singleton.IsAmbientMute);
        muteSFXIcon.SetActive(SoundManager.singleton.IsSFXMute);

        muteMasterBtn.onClick.AddListener(() => {
            if (!SoundManager.singleton.IsMasterMute)
                SoundManager.singleton.MuteAll();
            else
                SoundManager.singleton.UnMuteAll();

            muteMasterIcon.SetActive(SoundManager.singleton.IsMasterMute);
            muteBGMIcon.SetActive(SoundManager.singleton.IsBGMMute);
            muteAmbientIcon.SetActive(SoundManager.singleton.IsAmbientMute);
            muteSFXIcon.SetActive(SoundManager.singleton.IsSFXMute);
        });

        muteBGMBtn.onClick.AddListener(() => {
            if (!SoundManager.singleton.IsBGMMute)
                SoundManager.singleton.MuteBGM();
            else
                SoundManager.singleton.UnMuteBGM();

            muteBGMIcon.SetActive(SoundManager.singleton.IsBGMMute);

            if(!SoundManager.singleton.IsMasterMute)
                muteMasterIcon.SetActive(SoundManager.singleton.IsMasterMute);
        });

        muteAmbientBtn.onClick.AddListener(() => {
            if (!SoundManager.singleton.IsAmbientMute)
                SoundManager.singleton.MuteAmbient();
            else
                SoundManager.singleton.UnmuteAmbient();

            muteAmbientIcon.SetActive(SoundManager.singleton.IsAmbientMute);

            if (!SoundManager.singleton.IsMasterMute)
                muteMasterIcon.SetActive(SoundManager.singleton.IsMasterMute);
        });

        muteSFXBtn.onClick.AddListener(() => {
            if (!SoundManager.singleton.IsSFXMute)
                SoundManager.singleton.MuteSFX();
            else
                SoundManager.singleton.UnMuteSFX();

            muteSFXIcon.SetActive(SoundManager.singleton.IsSFXMute);

            if (!SoundManager.singleton.IsMasterMute)
                muteMasterIcon.SetActive(SoundManager.singleton.IsMasterMute);
        });
        base.Show();
    }

    public override void Hide()
    {
        tweenAlpha.SetOnCompleteCB(() => { Destroy(this.gameObject); });
        base.Hide();
    }

    private void OnDestroy()
    {
        masterVolumeSlider.onValueChanged.RemoveAllListeners();
        bgmVolumeSlider.onValueChanged.RemoveAllListeners();
        ambientVolumeSlider.onValueChanged.RemoveAllListeners();
        sfxVolumeSlider.onValueChanged.RemoveAllListeners();

        muteMasterBtn.onClick.RemoveAllListeners();
        muteBGMBtn.onClick.RemoveAllListeners();
        muteAmbientBtn.onClick.RemoveAllListeners();
        muteSFXBtn.onClick.RemoveAllListeners();
    }
}
