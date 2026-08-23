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
        RegisterByOwner();
    }

    public static SkinData FromSave(object key, string id, ushort materialIndex, string ownerUID)
    {
        SkinData skin = new SkinData(key);
        skin.id = id;
        skin.materialIndex = materialIndex;
        skin.ownerUID = ownerUID ?? string.Empty;
        skin.RegisterByOwner();
        return skin;
    }

    public new static void Load(List<SkinData> loadedData)
    {
        map.Clear();
        mapByOwner.Clear();
        if (loadedData != null)
        {
            for (int i = 0; i < loadedData.Count; i++)
            {
                SkinData model = loadedData[i];
                if (model == null || model.key == null)
                    continue;
                map[model.key] = model;
                model.RegisterByOwner();
            }
        }
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

    private void RegisterByOwner()
    {
        string owner = ownerUID ?? string.Empty;
        mapByOwner[owner] = this;
    }
}
