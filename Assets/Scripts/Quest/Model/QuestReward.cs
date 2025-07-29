namespace Model {
    public class QuestReward : Model<QuestReward> {
        public string itemKey;
        public int amount;

        public QuestReward() : base() {

        }

        public QuestReward(object key) : base(key) {
            
        }
    }
}