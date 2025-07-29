namespace Model {
    public class DamageableModel : Model<DamageableModel> {
        public int hp { get; private set; }
        public int def { get; private set; }
        public int multiplier { get; private set; } //-1 parts destroyed wont affect other parts
        
        public string nameKey { get; private set; }
        public string descriptionKey { get; private set; }
        public string iconKey { get; private set; }
        
        public DamageableModel() : base() {

        }

        public DamageableModel(object key) : base(key) {

        }
    }
}