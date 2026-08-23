namespace Model
{
    public class SkillModel : Model<SkillModel>
    {
        public enum TargetType
        {
            auto = 1,
            direction = 2,
            assignable = 3,
        }

        public enum SkillType
        {
            buff=1,
            placement=2,
        }

        public int type { get; protected set; } //技能類型
        public int maxTarget { get; protected set; } //最多可選定對象
        public int targetType { get; protected set; }
        public float targetRangeXValue { get; protected set; }
        public float targetRangeYValue { get; protected set; }
        public string value { get; protected set; }
        public string rangeVisualEntityKey { get; protected set; }
        public int formula { get; private set; } //技能公式
        public int cd { get; protected set; }
        public int allowParallel { get; protected set; }
        public string name { get; protected set; }
        public string description { get; protected set; }
        public string icon { get; protected set; }
        public bool isParallel
        {
            get
            {
                return allowParallel == 1;
            }
        }

        public SkillModel(object key) : base(key)
        {

        }
    }
}
