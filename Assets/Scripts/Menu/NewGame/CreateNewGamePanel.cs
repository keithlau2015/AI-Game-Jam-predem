using GameUI;
using SaveLoadModule;
using System;
using System.Collections;
using System.Collections.Generic;
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
        Destroy(gameObject);
    }

    public void SetUp(Action startGameCB)
    {
        button.onClick.AddListener(async() => {
            LoadingManager.singleton.Show(true, 2);
            IProgress<int> createAccountProgress = await LoadingManager.singleton.AddTask(LoadingManager.PresentType.ShowPercentage, "Creating Account...", 1);
            SaveDataModel save = new SaveDataModel(saveName);
            startGameCB?.Invoke();
            createAccountProgress.Report(1);
            Destroy(this.gameObject);
            UIManager.singleton.RemoveTopPreviousPanel();
            LoadingManager.singleton.Hide();
        });
        nameInputField.onValueChanged.AddListener((x) => { saveName = x; });
    }

    public void Show()
    {
        
    }

    private void OnDestroy()
    {
        button.onClick.RemoveAllListeners();
        nameInputField.onValueChanged.RemoveAllListeners();
    }
}
