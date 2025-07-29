namespace AchievementSystem
{
    public class AchievementModel : Model<AchievementModel>
    {
        public string observeModel { get; protected set; }
        public string observeField { get; protected set; }
        public string achieveKey { get; protected set; }
        public string achieveValue { get; protected set; }
        public object preAchievementKey { get; protected set; }
        public AchievementModel(object key) : base(key) { }
    }
}