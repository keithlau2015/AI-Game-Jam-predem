using BugReportSystem;
using LocalizationModule;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI
{
    public class BugReportPanel : CommonPopUpPanel
    {
        [SerializeField]
        private Text hintText;
        [SerializeField]
        private InputField summary, detail;
        [SerializeField]
        private Button submit;

        public event Action onSubmit;

        private void Start()
        {
            hintText.text = LocalizationManager.singleton.GetLocalization($"Hint_{ErrorCode.BugReportFailed_SummaryEmpty}");
            Show();
        }

        public override void Show()
        {
            summary.onValueChanged.AddListener((value) =>
            {
                if (string.IsNullOrEmpty(value))
                {
                    hintText.gameObject.SetActive(true);
                    hintText.text = LocalizationManager.singleton.GetLocalization($"Hint_{ErrorCode.BugReportFailed_SummaryEmpty}");
                    submit.interactable = false;
                }
                else
                {
                    hintText.gameObject.SetActive(false);
                    submit.interactable = true;
                }
            });
            submit.onClick.AddListener(SubmitReport);
            base.Show();
        }

        public override void Hide()
        {
            tweenAlpha.SetOnCompleteCB(() => Destroy(gameObject));
            base.Hide();
        }

        private async void SubmitReport()
        {
            BugReportConfig config = BugReportConfig.Load();
            if (config == null || !config.HasTrelloCredentials)
            {
                Debug.LogError("[BugReport] Missing Resources/BugReportConfig with Trello credentials.");
                UIManager.singleton.ShowCommonPopUpTextPanel(
                    true,
                    new CommonPopTextPanel.CommonPopUpTextPanelConfig()
                    {
                        showGreenBtn = true,
                        showRedBtn = false,
                        greenBtnLabeID = "SYS_Confirm"
                    },
                    "Bug report is not configured. Add Resources/BugReportConfig.",
                    () => UIManager.singleton.ShowCommonPopUpTextPanel(false),
                    null);
                return;
            }

            submit.interactable = false;
            GameObject miniLoading = LoadingManager.singleton.ShowMini(this.transform);
            BugReport bugReport = new BugReport(new Dictionary<BugReport.SupportItemIndex, bool>
            {
                { BugReport.SupportItemIndex.Trello, true }
            });

            bugReport.allItems[BugReport.SupportItemIndex.Trello].parameters["API_KEY"] = config.trelloApiKey.Trim();
            bugReport.allItems[BugReport.SupportItemIndex.Trello].parameters["API_TOKEN"] = config.trelloApiToken.Trim();
            bugReport.allItems[BugReport.SupportItemIndex.Trello].parameters["BOARD_NAME"] = config.trelloBoardId.Trim();
            bugReport.allItems[BugReport.SupportItemIndex.Trello].parameters["DEFAULT_LIST_NAME"] = config.trelloDefaultListName;

            bugReport.title = summary.text;
            bugReport.summary = detail.text;
            bugReport.sysInfo = GetSystemInfo();
            bugReport.sendTime = TimeManager.singleton.GetCurrentDatetime();

            string attachmentPath = $"{Application.persistentDataPath}/bugReport.jpg";
            bugReport.attachment = System.IO.File.Exists(attachmentPath)
                ? System.IO.File.ReadAllBytes(attachmentPath)
                : Array.Empty<byte>();

            bool result = await bugReport.SendReport();
            if (result)
            {
                onSubmit?.Invoke();
                LoadingManager.singleton.HideMini(miniLoading);
                Hide();
            }
            else
            {
                LoadingManager.singleton.HideMini(miniLoading);
                submit.interactable = true;
            }
        }

        private string GetSystemInfo()
        {
            return $"Platform: {SystemInfo.deviceType}\\n" +
                $"OS: {SystemInfo.operatingSystem}\\n" +
                $"REM: {SystemInfo.systemMemorySize / 1024} GB\\n" +
                $"CPU: {SystemInfo.processorType}, Core: {SystemInfo.processorCount / 2}\\n" +
                $"GPU: {SystemInfo.graphicsDeviceName}, REM: {SystemInfo.graphicsMemorySize / 1024} GB\\n" +
                $"Support Audio: {SystemInfo.supportsAudio}";
        }

        public void AutoFillBugReport(string errorCode)
        {
            summary.text = $"[Error]{errorCode}";
        }
    }
}
