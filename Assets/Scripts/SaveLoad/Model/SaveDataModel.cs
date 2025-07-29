using GenericGameModule;
using System;
using System.Collections.Generic;

namespace SaveLoadModule
{
    public class SaveDataModel : Model<SaveDataModel>
    {
        public SaveDataModel(object key) : base(key)
        {

        }

        public SaveDataModel() : base()
        {

        }

        public SaveDataModel(string name) : base()
        {
            this.key = Guid.NewGuid().ToString();
            this.Name = name;
            this.CreateTime = TimeManager.singleton.GetCurrentUnixtimestamp();
            PredictFileSize();
        }

        public string Name { get; private set; }
        public long CreateTime { get; private set; }
        public Int64 FileSize { get; private set; }

        public SortedDictionary<string, SortedDictionary<object, object>> data = new SortedDictionary<string, SortedDictionary<object, object>>();

        public async void PredictFileSize()
        {
            this.FileSize = await FileManager.PredictObjectSaveSize(new object[] { this });
        }
    }
}