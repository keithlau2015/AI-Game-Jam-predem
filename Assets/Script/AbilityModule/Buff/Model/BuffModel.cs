namespace Model
{
    public class BuffModel : Model<BuffModel>
    {
        public int type { get; private set; }
        public string name { get; private set; }
        public string description { get; private set; }
        public string icon { get; private set; }

        /// <summary>Duration in seconds. &lt;= 0 means permanent until removed.</summary>
        public float duration { get; private set; }

        /// <summary>AttributeModel.AttributeType as int. -1 = no attribute change.</summary>
        public int attributeType { get; private set; } = -1;

        /// <summary>Delta applied with AttributeData.EditMode.Add (string for BigInteger).</summary>
        public string attributeDelta { get; private set; } = "0";

        /// <summary>Max stacks for StackableBuff. &lt;= 1 means non-stackable.</summary>
        public int maxStack { get; private set; } = 1;

        public BuffModel(object key) : base(key) { }
        public BuffModel() : base() { }
    }
}
