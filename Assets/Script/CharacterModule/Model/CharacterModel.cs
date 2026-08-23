namespace Model
{
    public class CharacterModel : Model<CharacterModel>
    {
        public string iconID { get; private set; }
        public string nameID { get; private set; }
        public string descriptionID { get; private set;}

        public CharacterModel(string id) : base(id)
        {
            map.Add(id, this);
        }
    }
}