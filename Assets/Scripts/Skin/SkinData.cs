using SaveLoadModule;
using System.Collections.Generic;
public class SkinData : SaveableModel<SkinData>
{
    public string id { get; private set; }
    public ushort materialIndex { get; private set; } = 0;
    public string ownerUID { get; private set; }

    public static SortedDictionary<string, SkinData> mapByOwner = new SortedDictionary<string, SkinData>();
    public SkinData(object key) : base(key)
    {

    }

    public SkinData() : base()
    {
        mapByOwner.Add(this.ownerUID, this);
    }

    public void SetSkin(ushort materialIndex)
    {
        this.materialIndex = materialIndex;
    }

    public SkinData Clone()
    {
        SkinData skinInstance = new SkinData();
        skinInstance.id = this.id;
        skinInstance.materialIndex = this.materialIndex;
        skinInstance.ownerUID = this.ownerUID;
        return skinInstance;
    }
}
