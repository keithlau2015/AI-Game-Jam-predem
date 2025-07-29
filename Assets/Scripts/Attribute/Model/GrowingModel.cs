namespace AttributeSystem
{
    public class GrowingModel
    {
        public enum Operator : int
        {
            Additive = 0,
            Multiply = 1
        }

        public int ID { get; set; }
        public int OwnerID { get; set; }
        public int Level { get; set; }
        public bool isFullRecover { get; set; }
        public int OperatorIndex { get; set; }
        public long Value { get; set; }
    }
}