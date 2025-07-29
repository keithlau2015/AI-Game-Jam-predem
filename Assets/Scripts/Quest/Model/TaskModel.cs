using ItemModule;

namespace Model {
    public class TaskModel : Model<TaskModel> {
        public string nameKey { get; private set; }
        public string descriptionKey { get; private set; }
        public string iconKey { get; private set; }
        
        public string taskItemKey { get; private set; }
        public int taskItemCount { get; private set; }

        public bool isCompleted { 
            get
            {
                ItemData itemData = null;
                if (!ItemData.map.TryGetValue(taskItemKey, out itemData))
                {
                    return false;
                }
                return itemData.count >= taskItemCount;
            } 
        }

        public TaskModel() : base() {

        }

        public TaskModel(object key) : base(key) {

        }
    }
}