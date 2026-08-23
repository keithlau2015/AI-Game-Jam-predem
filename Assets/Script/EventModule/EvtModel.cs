namespace Model
{
    public class EvtModel : Model<EvtModel>
    {
        public enum EventType
        {
            InstantWin = 1,
            InstantLose = 2,
            PlayAVG = 3,
            SpawnUnit = 4,
            Buff = 5,
            CreateEvt = 6,
            EnterBattle = 7,
            StopEvt = 8,
        }

        public int type { get; private set; }
        public int subType { get; private set; }
        public string value { get; private set; }
        public int triggerCount { get; private set; } //-1 means infinite trigger
        public EvtModel(string id) : base(id)
        {

        }

        public EvtModel() : base() { }
    }
}