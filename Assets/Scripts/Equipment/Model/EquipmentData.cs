using SaveLoadModule;

public class EquipmentData : SaveableModel<EquipmentData>
{
    public object id { get; private set; }
    
    //Create
    public EquipmentData(string id)
    {
        this.id = id;
    }

    //Load
    public EquipmentData(object key) : base(key)
    {

    }

    public EquipmentData Clone()
    {
        EquipmentData equipmentInstance = new EquipmentData(this.id);
        return equipmentInstance;
    }
}