namespace Model
{
    public class EvtTriggerModel : Model<EvtTriggerModel>
    {
        public enum OperatorID
        {
            Equal = 0,
            Lesser = 1,
            Greater = 2,
            LesserrEqual = 3,
            GreaterEqual = 4,
        }

        public enum Event
        {
            UnitAttribute = 0,
            Captureable = 1,
            BattleDuration = 2,
            UnitSurvivor = 3,
            KeyPressed = 4,
            EnterArea = 5,
            EnterOpenWorld = 6,
            EvtTriggerCount = 7,
        }

        public enum UnitSurvivor
        {
            type = 0,
            id = 1,
            team = 2,
        }

        public int type { get; private set; }
        public int subType { get; private set; }
        public string condID { get; private set; }
        public int operatorID { get; private set; }
        public string value { get; private set; }

        public EvtTriggerModel(string id) : base(id)
        {

        }

        public EvtTriggerModel() : base() { }
    }
}