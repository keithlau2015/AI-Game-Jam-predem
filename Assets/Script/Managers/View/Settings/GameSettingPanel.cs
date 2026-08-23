using UnityEngine;
using UnityEngine.UI;

namespace GameUI {
    public class GameSettingPanel : CommonPopUpPanel
    {
        [SerializeField]
        private Button langPrefBtn, graphicPrefBtn, soundPrefBtn, controlPrefBtn, creditBtn, bugReportBtn;

        private void Start()
        {
            Show();
        }

        public override void Show()
        {
            langPrefBtn.onClick.AddListener(async () => {
                await UIManager.singleton.LoadUI<LangPrefSettingPanel>(typeof(LangPrefSettingPanel).Name);
            });

            graphicPrefBtn.onClick.AddListener(async () => {
                await UIManager.singleton.LoadUI<GraphicPrefSettingPanel>(typeof(GraphicPrefSettingPanel).Name);
            });

            soundPrefBtn.onClick.AddListener(async () => {
                await UIManager.singleton.LoadUI<AudioPrefSettingPanel>(typeof(AudioPrefSettingPanel).Name);
            });

            controlPrefBtn.onClick.AddListener(async () => {
                await UIManager.singleton.LoadUI<ControlPrefSettingPanel>(typeof(ControlPrefSettingPanel).Name);
            });

            creditBtn.onClick.AddListener(async () => {
                await UIManager.singleton.LoadUI<CreditPanel>(typeof(CreditPanel).Name);                
            });

            bugReportBtn.onClick.AddListener(async () => {
                ScreenCapture.CaptureScreenshot($"{Application.persistentDataPath}/bugReport.jpg");
                BugReportPanel bugReportPanel = await UIManager.singleton.LoadUI<BugReportPanel>(typeof(BugReportPanel).Name);
            });
            base.Show();
        }

        public override void Hide()
        {
            tweenAlpha.SetOnCompleteCB(() => Destroy(gameObject));
            base.Hide();
        }

        private void OnDestroy()
        {
            langPrefBtn.onClick.RemoveAllListeners();
            graphicPrefBtn.onClick.RemoveAllListeners();
            soundPrefBtn.onClick.RemoveAllListeners();
            controlPrefBtn.onClick.RemoveAllListeners();
            creditBtn.onClick.RemoveAllListeners();
            bugReportBtn.onClick.RemoveAllListeners();
        }
    }
}