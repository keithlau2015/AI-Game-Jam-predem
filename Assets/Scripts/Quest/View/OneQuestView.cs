using LocalizationModule;
using Model;
using UnityEngine;
using UnityEngine.UI;

namespace Quest {
    public class OneQuestView : MonoBehaviour {
        [SerializeField]
        private Text questName;
        [SerializeField]
        private Text questDescription;
        [SerializeField]
        private Text questCurrentProgress;
        [SerializeField]
        private Text questTotalProgress;
        [SerializeField]
        private Button questButton;
        [SerializeField]
        private Image questImage;
        [SerializeField]
        private Image questCompletedImage;

        private QuestModel quest;

        public void SetQuest(QuestModel quest) {
            this.quest = quest;
            questName.text = LocalizationController.singleton.GetLabel(quest.nameKey);
            questDescription.text = LocalizationController.singleton.GetLabel(quest.descriptionKey);
            questImage.sprite = LocalizationController.singleton.GetSprite(quest.iconKey);
            
            if(QuestController.IsQuestCompleted(quest.key.ToString())) {
                questCompletedImage.sprite = LocalizationController.singleton.GetSprite("Quest/Completed"); 
                questCompletedImage.gameObject.SetActive(true);
            } else {
                questCompletedImage.gameObject.SetActive(false);
            }
            
            UpdateQuestContext();
            questButton.onClick.AddListener(ShowQuestDetails);
        }

        private void UpdateQuestContext() {
            
            questCurrentProgress.text = QuestController.GetCurrentQuestProgress(quest.key.ToString()).ToString();
            questTotalProgress.text = QuestController.GetTotalQuestProgress(quest.key.ToString()).ToString();
        }

        private void ShowQuestDetails() {

        }
    }
}