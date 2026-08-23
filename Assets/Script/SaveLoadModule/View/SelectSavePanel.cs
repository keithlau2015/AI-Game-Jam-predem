using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Pool;
using GameUI;
using System;
using System.Collections.Generic;
using System.Linq;
using LocalizationModule;
using SaveLoadModule;
using BugReportSystem;
using Model;

public class SelectSavePanel : MonoBehaviour, IPreviousablePanel, LoopScrollPrefabSource, LoopScrollDataSource
{
    [SerializeField]
    private OneSave oneSavePrefab;
    [SerializeField]
    private int totalCount = -1;
    [SerializeField]
    private LoopVerticalScrollRect loopVerticalScrollRect;
    private ObjectPool<GameObject> pool;
    private Action onSelectCB;
    private IReadOnlyList<SaveSlotInfo> _slots = Array.Empty<SaveSlotInfo>();

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
        if (_slots == null || idx < 0 || idx >= _slots.Count)
            return;

        if (!transform.TryGetComponent(out OneSave oneSave))
            return;

        oneSave.SetUp(_slots[idx], SelectSave);
    }

    public void ReturnObject(Transform trans)
    {
        pool.Release(trans.gameObject);
    }

    private void SelectSave(SaveSlotInfo slot)
    {
        if (slot == null)
            return;

        UIManager.singleton.ShowCommonPopUpTextPanel(
            true,
            new CommonPopTextPanel.CommonPopUpTextPanelConfig() { showGreenBtn = true, showRedBtn = true, greenBtnLabeID = "SYS_Apply", redBtbLabelID = "SYS_Revert" },
            LocalizationManager.singleton.GetLocalization("SYS_ConfirmApplySelectSave"),
            async() => {
                UIManager.singleton.ShowCommonPopUpTextPanel(false);
                LoadingManager.singleton.Show(true, 1);
                IProgress<int> loadAccountProgress = await LoadingManager.singleton.AddTask(LoadingManager.PresentType.ShowPercentage, "Loading SaveFile...", 1);
                string errorCode = "";
                SaveService.LoadSave(slot.SlotId, out errorCode);
                if (!string.IsNullOrEmpty(errorCode))
                {
                    loadAccountProgress.Report(1);
                    LoadingManager.singleton.Hide();
                    OnStartBattleFailed(errorCode);
                    return;
                }

                loadAccountProgress.Report(1);
                UIManager.singleton.RemoveTopPreviousPanel();
                onSelectCB?.Invoke();
                GameStateController.singleton.stateMachine.OnErrorOccur += OnStartBattleFailed;

                string levelKey = ResolveEnterLevelKey(slot);
                bool started = !string.IsNullOrEmpty(levelKey)
                    && GameStateController.singleton.stateMachine.LoadLevel(levelKey);
                if (!started)
                    started = GameStateController.singleton.stateMachine.LoadDefaultScene();
                if (!started)
                {
                    Debug.LogWarning("[SaveLoad] Could not enter a scene after load. Add LevelModel data or a scene in Build Settings.");
                    GameStateController.singleton.stateMachine.OnErrorOccur -= OnStartBattleFailed;
                }

                Destroy(this.gameObject);
                LoadingManager.singleton.Hide();
                UIManager.singleton.ClearAllUI();
            },
            () => { UIManager.singleton.ShowCommonPopUpTextPanel(false); Destroy(this.gameObject); }
        );
    }

    public async void Show()
    {
        await SaveService.EnsureCatalogLoaded();
        _slots = SaveService.ListSlots();

        pool = new ObjectPool<GameObject>(
           () => Instantiate(oneSavePrefab.gameObject),
           o => o.SetActive(true),
           o =>
           {
               o.transform.SetParent(transform);
               o.SetActive(false);
           });
        loopVerticalScrollRect.prefabSource = this;
        loopVerticalScrollRect.dataSource = this;
        loopVerticalScrollRect.totalCount = _slots.Count;
        loopVerticalScrollRect.RefillCells();
    }

    public void Hide()
    {
        Destroy(this.gameObject);
    }

    private static string ResolveEnterLevelKey(SaveSlotInfo slot)
    {
        if (slot != null && !string.IsNullOrEmpty(slot.LastLevelKey)
            && LevelModel.map != null && LevelModel.map.ContainsKey(slot.LastLevelKey))
            return slot.LastLevelKey;

        string last = PlayerPrefs.GetString("LastLevelKey", string.Empty);
        if (!string.IsNullOrEmpty(last) && LevelModel.map != null && LevelModel.map.ContainsKey(last))
            return last;

        if (LevelModel.map != null && LevelModel.map.Count > 0)
            return LevelModel.map.Keys.First().ToString();

        return string.Empty;
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
