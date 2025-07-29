namespace Model {
    public class ChapterModel : Model<ChapterModel> {
        public string nameKey { get; private set; }
        public string descriptionKey { get; private set; }
        public string iconKey { get; private set; }
        
        public ChapterModel() : base() {

        }

        public ChapterModel(object key) : base(key) {

        }
    }
}