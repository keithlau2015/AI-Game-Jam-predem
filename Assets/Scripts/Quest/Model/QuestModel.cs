namespace Model {
    public class QuestModel : Model<QuestModel> {
        public string nameKey { get; private set; }
        public string descriptionKey { get; private set; }
        public string iconKey { get; private set; }
        public string preQuestKey { get; private set; }
        
        public QuestModel() : base() {

        }

        public QuestModel(object key) : base(key) {

        }
    }
}