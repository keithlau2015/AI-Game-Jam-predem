using Model;
using System.Collections.Generic;
using System.Linq;

namespace Quest {
    public class QuestController {
        public static int GetCurrentQuestProgress(string questKey) {
            return QuestTaskModel.GetTaskByQuestKey(questKey).Where(task => task.isCompleted).ToList().Count;
        }

        public static int GetTotalQuestProgress(string questKey) {
            return QuestTaskModel.GetTaskByQuestKey(questKey).Count;
        }

        public static bool IsQuestCompleted(string questKey) {
            return GetCurrentQuestProgress(questKey) == GetTotalQuestProgress(questKey);
        }

        public static bool IsChapterCompleted(string chapterKey) {
            Queue<QuestModel> quests = QuestChapterModel.GetQuestQueueByChapterKey(chapterKey);
            foreach (QuestModel quest in quests) {
                if (!IsQuestCompleted(quest.key.ToString())) {
                    return false;
                }
            }
            return true;
        }
    }
}
