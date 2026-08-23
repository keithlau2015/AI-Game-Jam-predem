namespace Model
{
    public class LevelModel : Model<LevelModel>
    {
        public int sceneIndex { get; protected set; }
        public string name { get; protected set; }
        public string description { get; protected set; }
        public LevelModel(object key) : base(key) { }
        public LevelModel() : base() { }
    }
}