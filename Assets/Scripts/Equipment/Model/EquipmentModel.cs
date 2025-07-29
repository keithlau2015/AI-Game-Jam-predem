namespace GenericGameModule
{
    public class EquipmentModel : Model<EquipmentModel>
    {
        public string item { get; private set; }
        public int type { get; private set; }

        public EquipmentModel(string id) : base(id)
        {

        }
    }
}