namespace Model {
    public class GameEvtModel : Model<GameEvtModel> {

        public enum Objective {
            itemRetrieved,
            objectDestroyed,
            objectCreated,
            objectDamaged,
            objectHealed,
            objectBuffed,
            objectDebuffed,
        }

        public int objective { get; private set; }
        public int objectiveKey { get; private set; }
        public int objectiveValue { get; private set; }

        public string nameKey { get; private set; }
        public string descriptionKey { get; private set; }
        public string iconKey { get; private set; }
    }
}