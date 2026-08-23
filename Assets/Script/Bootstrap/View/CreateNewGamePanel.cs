using BugReportSystem;
using GameUI;
using SaveLoadModule;
using System;
using UnityEngine;
using UnityEngine.UI;

public class CreateNewGamePanel : MonoBehaviour, IPreviousablePanel
{
    [SerializeField]
    private InputField nameInputField;
    [SerializeField]
    private Button button;
    private string saveName;

    public void Hide()
    {
        Destroy(this.gameObject);
    }

    private void OnLoadFailed(string result)
    {
        if (BugReport.errorReportedRecords.Contains(result))
        {
            UIManager.singleton.ShowCommonPopUpTextPanel(true,
                new CommonPopTextPanel.CommonPopUpTextPanelConfig()
                {
                    showGreenBtn = true,
                    showRedBtn = true,
                    greenBtnLabeID = "SYS_Confirm",
                    redBtbLabelID = "SYS_Cancel"
                },
                $"ERROR[{result}]",
                () => { UIManager.singleton.ShowCommonPopUpTextPanel(false); GameStateController.singleton.stateMachine.OnErrorOccur -= OnLoadFailed; },
                () => { UIManager.singleton.ShowCommonPopUpTextPanel(false); GameStateController.singleton.stateMachine.OnErrorOccur -= OnLoadFailed; }
            );
            return;
        }

        UIManager.singleton.ShowCommonPopUpTextPanel(true,
            new CommonPopTextPanel.CommonPopUpTextPanelConfig()
            {
                showGreenBtn = true,
                showRedBtn = true,
                greenBtnLabeID = "SYS_Confirm",
                redBtbLabelID = "SYS_Cancel"
            },
            $"ERROR[{result}] Do you wanted to report this bug?",
            async () =>
            {
                ScreenCapture.CaptureScreenshot($"{Application.persistentDataPath}/bugReport.jpg");
                UIManager.singleton.ShowCommonPopUpTextPanel(false);

                BugReportPanel bugReportPanel = await UIManager.singleton.LoadUI<BugReportPanel>(typeof(BugReportPanel).Name);
                bugReportPanel.AutoFillBugReport(result);
                bugReportPanel.onSubmit += () =>
                {
                    UIManager.singleton.ShowCommonPopUpTextPanel(false);
                    BugReport.errorReportedRecords.Add(result);
                };
                GameStateController.singleton.stateMachine.OnErrorOccur -= OnLoadFailed;
            },
            () => { UIManager.singleton.ShowCommonPopUpTextPanel(false); GameStateController.singleton.stateMachine.OnErrorOccur -= OnLoadFailed; }
        );
    }

    public void Show()
    {
        button.onClick.AddListener(async () =>
        {
            LoadingManager.singleton.Show(true, 1);
            IProgress<int> createAccountProgress = await LoadingManager.singleton.AddTask(
                LoadingManager.PresentType.ShowPercentage, "Creating Account...", 1);

            string name = string.IsNullOrWhiteSpace(saveName) ? "New Game" : saveName;
            await SaveService.CreateSave(name);
            createAccountProgress.Report(1);

            GameStateController.singleton.stateMachine.OnErrorOccur += OnLoadFailed;
            bool started = GameStateController.singleton.stateMachine.LoadDefaultScene();
            if (!started)
            {
                Debug.LogWarning("[Bootstrap] No default scene to enter after new game. Add a scene to Build Settings.");
                GameStateController.singleton.stateMachine.OnErrorOccur -= OnLoadFailed;
            }

            Destroy(this.gameObject);
            LoadingManager.singleton.Hide();
            UIManager.singleton.ClearAllUI();
        });
        nameInputField.onValueChanged.AddListener(x => { saveName = x; });
    }

    private void OnDestroy()
    {
        button.onClick.RemoveAllListeners();
        nameInputField.onValueChanged.RemoveAllListeners();
    }
}
