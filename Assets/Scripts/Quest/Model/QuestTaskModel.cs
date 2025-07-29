using System.Collections.Generic;

namespace Model {
    public class QuestTaskModel : Model<QuestTaskModel> {
        public string questKey { get; private set; }
        public string taskKey { get; private set; }
        
        public static List<TaskModel> GetTaskByQuestKey(string questKey) {
            List<TaskModel> taskList = new List<TaskModel>();
            foreach (var qt in QuestTaskModel.map.Values) {
                if (qt.questKey == questKey) {
                    TaskModel task = null;
                    if (!TaskModel.map.TryGetValue(qt.taskKey, out task)) {
                        continue;
                    }
                    taskList.Add(task);
                }
            }
            return taskList;
        }

        public QuestTaskModel() : base() {

        }

        public QuestTaskModel(object key) : base(key) {

        }
    }
}