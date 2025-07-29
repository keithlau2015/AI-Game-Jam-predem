using GameUI;
using LocalizationModule;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace SaveLoadModule
{
    public class SaveRecord : MonoBehaviour
    {
        private static bool showFileSize;
        [SerializeField]
        private Text nameLabel;
        [SerializeField]
        private Text createDateLabel;
        [SerializeField]
        private Text sizeLabel;
        [SerializeField]
        private Button selectBtn;
        [SerializeField]
        private Button deleteBtn;

        private string saveUID;

        public void SetUp(string saveUID)
        {
            this.saveUID = saveUID;
            SaveDataModel saveDataModel = null;
            if (!SaveDataModel.map.TryGetValue(saveUID, out saveDataModel)) return;
            nameLabel.text = LocalizationController.singleton.GetLabel(saveDataModel?.Name);
            createDateLabel.text = TimeManager.singleton.UnixTimeStamp2DateTime(saveDataModel.CreateTime).ToString("yyyy/MM/dd HH:mm:ss");
            selectBtn.onClick.AddListener(Load);
            deleteBtn.onClick.AddListener(Delete);
            if (showFileSize)
            {
                sizeLabel.gameObject.SetActive(true);
                sizeLabel.text = FileManager.SizeSuffix(saveDataModel.FileSize);
            }
            else
                sizeLabel.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            selectBtn.onClick.RemoveAllListeners();
            deleteBtn.onClick.RemoveAllListeners();
        }

        private void Load()
        {
            UIManager.singleton.ShowCommonPopUpTextPanel(
                true,
                new CommonPopTextPanel.CommonPopUpTextPanelConfig() { showGreenBtn = true, showRedBtn = true, greenBtnLabeID = "SYS_Apply", redBtbLabelID = "SYS_Revert" },
                LocalizationController.singleton.GetLabel("SYS_ConfirmApplySelectSave"),
                async () => {
                    UIManager.singleton.ShowCommonPopUpTextPanel(false);
                    LoadingManager.singleton.Show(true, 2);
                    IProgress<int> loadAccountProgress = await LoadingManager.singleton.AddTask(LoadingManager.PresentType.ShowPercentage, "Loading SaveFile...", 1);
                    loadAccountProgress.Report(1);
                    UIManager.singleton.RemoveTopPreviousPanel();
                    SaveLoadController.LoadSave(saveUID, out _);
                    LoadingManager.singleton.Hide();
                },
                () => { UIManager.singleton.ShowCommonPopUpTextPanel(false); Destroy(this.gameObject); }
            );
        }

        private void Delete()
        {

        }
    }
}