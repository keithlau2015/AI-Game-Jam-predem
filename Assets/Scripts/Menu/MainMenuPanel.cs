using ItemModule;
using SaveLoadModule;
using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuPanel : MonoBehaviour
{
    [SerializeField]
    private Button newGameBtn, loadGameBtn, settingBtn, quitBtn;
    private void Awake()
    {
        newGameBtn.onClick.AddListener(NewGame);
        loadGameBtn.onClick.AddListener(LoadGame);
        settingBtn.onClick.AddListener(Setting);
        quitBtn.onClick.AddListener(GameManager.ExitGame);
    }

    private async void NewGame()
    {
        /*
        CreateAccountPanel createAccountPanel = await UIManager.singleton.LoadUI<CreateAccountPanel>(typeof(CreateAccountPanel).Name);
        createAccountPanel.SetUp(() => Destroy(this.gameObject));
        createAccountPanel.Show();
        */
        LoadingManager.singleton.Show(true, 2);
        IProgress<int> createAccountProgress = await LoadingManager.singleton.AddTask(LoadingManager.PresentType.ShowPercentage, "Creating Account...", 1);
        //AccountManager.singleton.currentSelectAcctIns = AccountManager.singleton.CreateNewAccount($"New Game {AccountManager.singleton.accountMap.Count+1}", $"Player_{AccountManager.singleton.accountMap.Count}", "");
        SaveLoadController.CreateSave("New Game");
        createAccountProgress.Report(1);
        Destroy(this.gameObject);
        UIManager.singleton.RemoveTopPreviousPanel();
        LoadingManager.singleton.Hide();
    }

    private async void LoadGame()
    {
        SaveLoadMenu saveLoadMenu = await UIManager.singleton.LoadUI<SaveLoadMenu>(typeof(SaveLoadMenu).Name);
        saveLoadMenu.SetUp(() => Destroy(this.gameObject));
        saveLoadMenu.Show();
    }

    private void Setting()
    {
        //UIManager.singleton.LoadUI("SettingPanel");
        
    }

    private void OnDestroy()
    {
        newGameBtn.onClick.RemoveAllListeners();
        loadGameBtn.onClick.RemoveAllListeners();
        settingBtn.onClick.RemoveAllListeners();
        quitBtn.onClick.RemoveAllListeners();
    }
}
