using SaveLoadModule;
using System;
using System.Collections.Generic;

public class EvtRecordData : SaveableModel<EvtRecordData>
{
    public string id { get; set; }

    private int _value;
    public int value {
        get {
            return _value;
        }

        set { 
            this._value = value;
            onValueChanged?.Invoke(this);
        }
    }

    public static SortedDictionary<string, EvtRecordData> mapByEvtName = new SortedDictionary<string, EvtRecordData>();
    public event Action<EvtRecordData> onValueChanged;

    //Create
    public EvtRecordData(string id) : base()
    {
        this.id = id;
        if(mapByEvtName.ContainsKey(id))
        {
            return;
        }

        mapByEvtName.Add(id, this);
    }

    //Load
    public EvtRecordData(object key) : base(key)
    {
        RegisterByEvtName();
    }

    public static EvtRecordData FromSave(object key, string id, int value)
    {
        EvtRecordData data = new EvtRecordData(key);
        data.id = id;
        data._value = value;
        data.RegisterByEvtName();
        return data;
    }

    public new static void Load(List<EvtRecordData> loadedData)
    {
        map.Clear();
        mapByEvtName.Clear();
        if (loadedData != null)
        {
            for (int i = 0; i < loadedData.Count; i++)
            {
                EvtRecordData model = loadedData[i];
                if (model == null || model.key == null)
                    continue;
                map[model.key] = model;
                model.RegisterByEvtName();
            }
        }
    }

    private void RegisterByEvtName()
    {
        if (string.IsNullOrEmpty(id))
            return;
        mapByEvtName[id] = this;
    }
}