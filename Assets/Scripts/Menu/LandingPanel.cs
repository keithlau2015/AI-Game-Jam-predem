using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System;
using LocalizationModule;

namespace GameUI {
    public class LandingPanel : MonoBehaviour
    {
        [SerializeField]
        private Text gameVersionNum;
        [SerializeField]
        private Button settingBtn, privacyPolicyBtn, startGameBtn;

        private void Start()
        {

            settingBtn.onClick.AddListener(async () => {
                await UIManager.singleton.LoadUI<GameSettingPanel>(typeof(GameSettingPanel).Name);
            });

            privacyPolicyBtn.onClick.AddListener(() => UIManager.singleton.ShowCommonPopUpTextPanel(true,
                new CommonPopTextPanel.CommonPopUpTextPanelConfig()
                {
                    showGreenBtn = true,
                    showRedBtn = false,
                    greenBtnLabeID = "SYS_Agree"
                },
                LocalizationController.singleton.GetLabel("SYS_PrivatePolicy")
            ));

            startGameBtn.onClick.AddListener(() =>
            {
                //load to lobby scene
                /*
                LoadingManager.singleton.Show(true, 1);
                IProgress<int> loadsceneprogress = await LoadingManager.singleton.AddTask(LoadingManager.PresentType.ShowPercentage, "loading to lobby", 100);
                await SceneManager.LoadSceneAsync(1).ToUniTask(Progress.Create<float>(x => {
                    loadsceneprogress.Report((int)(x * 100f));
                }));
                LoadingManager.singleton.Hide();
                */
                UIManager.singleton.LoadUI(typeof(MainMenuPanel).Name);
                Destroy(gameObject);
            });

            if (gameVersionNum != null)
                gameVersionNum.text = $"{Application.version}";
        }
    }
}