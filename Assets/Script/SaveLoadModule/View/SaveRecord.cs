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

        private SaveSlotInfo _slot;

        public void SetUp(SaveSlotInfo slot)
        {
            _slot = slot;
            if (_slot == null)
                return;

            nameLabel.text = LocalizationManager.singleton.GetLocalization(_slot.DisplayName);
            createDateLabel.text = _slot.CreatedUtc.ToString("yyyy/MM/dd HH:mm:ss");
            selectBtn.onClick.RemoveAllListeners();
            deleteBtn.onClick.RemoveAllListeners();
            selectBtn.onClick.AddListener(Load);
            deleteBtn.onClick.AddListener(Delete);
            if (showFileSize)
            {
                sizeLabel.gameObject.SetActive(true);
                sizeLabel.text = FileManager.SizeSuffix(_slot.FileSizeBytes);
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
            if (_slot == null)
                return;

            UIManager.singleton.ShowCommonPopUpTextPanel(
                true,
                new CommonPopTextPanel.CommonPopUpTextPanelConfig() { showGreenBtn = true, showRedBtn = true, greenBtnLabeID = "SYS_Apply", redBtbLabelID = "SYS_Revert" },
                LocalizationManager.singleton.GetLocalization("SYS_ConfirmApplySelectSave"),
                async () => {
                    UIManager.singleton.ShowCommonPopUpTextPanel(false);
                    LoadingManager.singleton.Show(true, 2);
                    IProgress<int> loadAccountProgress = await LoadingManager.singleton.AddTask(LoadingManager.PresentType.ShowPercentage, "Loading SaveFile...", 1);
                    loadAccountProgress.Report(1);
                    UIManager.singleton.RemoveTopPreviousPanel();
                    SaveService.LoadSave(_slot.SlotId, out _);
                    LoadingManager.singleton.Hide();
                },
                () => { UIManager.singleton.ShowCommonPopUpTextPanel(false); Destroy(this.gameObject); }
            );
        }

        private async void Delete()
        {
            if (_slot == null)
                return;

            await SaveService.DeleteSave(_slot.SlotId);
            Destroy(gameObject);
        }
    }
}
