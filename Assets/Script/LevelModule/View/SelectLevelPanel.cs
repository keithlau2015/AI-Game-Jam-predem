
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Pool;
using GameUI;
using System;
using LocalizationModule;
using BugReportSystem;
using System.Linq;
using Model;

public class SelectLevelPanel : MonoBehaviour, IPreviousablePanel, LoopScrollPrefabSource, LoopScrollDataSource
{
    [SerializeField]
    private OneLevel oneLevelPrefab;
    [SerializeField]
    private int totalCount = -1;
    [SerializeField]
    private LoopVerticalScrollRect loopVerticalScrollRect;
    private ObjectPool<GameObject> pool;
    private Action onSelectCB;
    public GameObject GetObject(int index)
    {
        return pool.Get();
    }

    public void SetUp(Action onClickCB)
    {
        this.onSelectCB = onClickCB;
    }

    public void ProvideData(Transform transform, int idx)
    {
        if (idx > LevelModel.map.Count || idx < 0)
            return;

        LevelModel data = LevelModel.map.Values.ToList()[idx];
        OneLevel oneLevel = null;
        if (!transform.TryGetComponent(out oneLevel))
            return;

        oneLevel.SetUp(data, SelectLevel);
    }

    public void ReturnObject(Transform trans)
    {
        pool.Release(trans.gameObject);
    }

    private void SelectLevel(LevelModel model)
    {
        UIManager.singleton.ShowCommonPopUpTextPanel(
            true,
            new CommonPopTextPanel.CommonPopUpTextPanelConfig() { showGreenBtn = true, showRedBtn = true, greenBtnLabeID = "SYS_Apply", redBtbLabelID = "SYS_Revert" },
            LocalizationManager.singleton.GetLocalization("SYS_ConfirmApplySelectSave"),
            async () => {
                UIManager.singleton.ShowCommonPopUpTextPanel(false);
                LoadingManager.singleton.Show(true, 1);
                //IProgress<int> loadLevelProgress = await LoadingManager.singleton.AddTask(LoadingManager.PresentType.ShowPercentage, "Loading level...", 1);
                //LevelController.LoadLevel(model);
                //loadLevelProgress.Report(1);
                UIManager.singleton.RemoveTopPreviousPanel();
                onSelectCB?.Invoke();
                GameStateController.singleton.stateMachine.OnErrorOccur += OnStartBattleFailed;
                //GameStateController.singleton.stateMachine.SetState(new EnterLevelState(model.sceneIndex,GameStateController.singleton.stateMachine));
                GameStateController.singleton.stateMachine.LoadLevel(model.key.ToString());
                Destroy(this.gameObject);
                LoadingManager.singleton.Hide();
                UIManager.singleton.ClearAllUI();
            },
            () => { UIManager.singleton.ShowCommonPopUpTextPanel(false); Destroy(this.gameObject); }
        );
    }

    public void Show()
    {
        pool = new ObjectPool<GameObject>(
           () => Instantiate(oneLevelPrefab.gameObject),
           o => o.SetActive(true),
           o =>
           {
               o.transform.SetParent(transform);
               o.SetActive(false);
           });
        loopVerticalScrollRect.prefabSource = this;
        loopVerticalScrollRect.dataSource = this;
        loopVerticalScrollRect.totalCount = LevelModel.map.Count;
        loopVerticalScrollRect.RefillCells();
    }

    public void Hide()
    {
        Destroy(this.gameObject);
    }

    private void OnStartBattleFailed(string result)
    {
        if (BugReport.errorReportedRecords.Contains(result))
        {
            UIManager.singleton.ShowCommonPopUpTextPanel(true,
                new GameUI.CommonPopTextPanel.CommonPopUpTextPanelConfig()
                {
                    showGreenBtn = true,
                    showRedBtn = true,
                    greenBtnLabeID = "SYS_Confirm",
                    redBtbLabelID = "SYS_Cancel"
                },
                $"ERROR[{result}]",
                () => { UIManager.singleton.ShowCommonPopUpTextPanel(false); GameStateController.singleton.stateMachine.OnErrorOccur -= OnStartBattleFailed; },
                () => { UIManager.singleton.ShowCommonPopUpTextPanel(false); GameStateController.singleton.stateMachine.OnErrorOccur -= OnStartBattleFailed; }
            );
            return;
        }

        UIManager.singleton.ShowCommonPopUpTextPanel(true,
            new GameUI.CommonPopTextPanel.CommonPopUpTextPanelConfig()
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
                GameStateController.singleton.stateMachine.OnErrorOccur -= OnStartBattleFailed;
            },
            () => { UIManager.singleton.ShowCommonPopUpTextPanel(false); GameStateController.singleton.stateMachine.OnErrorOccur -= OnStartBattleFailed; }
        );
    }

}
