using BugReportSystem;
using GameUI;
using SaveLoadModule;
using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuPanel : MonoBehaviour
{
    [SerializeField]
    private Button newGameBtn, loadGameBtn, settingBtn, quitBtn;
#if UNITY_EDITOR
    [SerializeField]
    private Button loadLevelBtn;
#endif

    private void Awake()
    {
        newGameBtn.onClick.AddListener(NewGame);
        loadGameBtn.onClick.AddListener(LoadGame);

        settingBtn.onClick.AddListener(Setting);
        quitBtn.onClick.AddListener(GameStateController.singleton.ExitGame);

#if UNITY_EDITOR
        loadLevelBtn.onClick.AddListener(LoadLevel);
#endif
    }

    private async void NewGame()
    {
        LoadingManager.singleton.Show(true, 1);
        IProgress<int> createNewSaveProgress = await LoadingManager.singleton.AddTask(LoadingManager.PresentType.ShowPercentage, "Create New Save...", 1);
        CreateNewGamePanel createNewGamePanel = await UIManager.singleton.LoadUI<CreateNewGamePanel>(typeof(CreateNewGamePanel).Name);
        createNewGamePanel.Show();
        createNewSaveProgress.Report(1);
        LoadingManager.singleton.Hide();
    }

    private async void LoadGame()
    {
        SelectSavePanel selectSavePanel = await UIManager.singleton.LoadUI<SelectSavePanel>(typeof(SelectSavePanel).Name);
        selectSavePanel.SetUp(() => Destroy(this.gameObject));
        selectSavePanel.Show();
    }

    private async void Setting()
    {
        await UIManager.singleton.LoadUI<GameSettingPanel>(typeof(GameSettingPanel).Name);
    }

#if UNITY_EDITOR
    private async void LoadLevel()
    {
        SelectLevelPanel selectLevelPanel = await UIManager.singleton.LoadUI<SelectLevelPanel>(typeof(SelectLevelPanel).Name);
        selectLevelPanel.SetUp(() => Destroy(this.gameObject));
        selectLevelPanel.Show();
    }
#endif

    private void OnDestroy()
    {
        newGameBtn.onClick.RemoveAllListeners();
        loadGameBtn.onClick.RemoveAllListeners();
        settingBtn.onClick.RemoveAllListeners();
        quitBtn.onClick.RemoveAllListeners();
#if UNITY_EDITOR
        loadLevelBtn.onClick.RemoveAllListeners();
#endif
    }
}
