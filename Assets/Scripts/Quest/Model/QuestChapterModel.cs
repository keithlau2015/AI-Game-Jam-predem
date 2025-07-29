using System.Collections.Generic;

namespace Model {
    public class QuestChapterModel : Model<QuestChapterModel> {
        public string questKey { get; private set; }
        public string chapterKey { get; private set; }

        public QuestChapterModel() : base() {

        }
        
        public QuestChapterModel(object key) : base(key) {

        }

        public static Queue<QuestModel> GetQuestQueueByChapterKey(string chapterKey) {
            Queue<QuestModel> questQueue = new Queue<QuestModel>();
            foreach (var qc in QuestChapterModel.map.Values) {
                if (qc.chapterKey == chapterKey) {
                    QuestModel quest = null;
                    if (!QuestModel.map.TryGetValue(qc.questKey, out quest)) {
                        continue;
                    }
                    questQueue.Enqueue(quest);
                }
            }
            return questQueue;
        }
    }
}