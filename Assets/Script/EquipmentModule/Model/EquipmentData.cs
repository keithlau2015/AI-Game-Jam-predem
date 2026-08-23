using Model;
using SaveLoadModule;
using System.Collections.Generic;

namespace EquipmentModule
{
    public class EquipmentData : SaveableModel<EquipmentData>
    {
        public object id { get; private set; }
        public string ownerUID { get; private set; }

        public static SortedDictionary<string, List<EquipmentData>> mapByOwner = new SortedDictionary<string, List<EquipmentData>>();

        //Create
        public EquipmentData(string id, string ownerUID) : base()
        {
            this.id = id;
            this.ownerUID = ownerUID;
            RegisterInOwnerGroups();
        }

        //Load
        public EquipmentData(object key) : base(key)
        {
        }

        public static EquipmentData FromSave(object key, string id, string ownerUID)
        {
            EquipmentData data = new EquipmentData(key);
            data.id = id;
            data.ownerUID = ownerUID ?? string.Empty;
            data.RegisterInOwnerGroups();
            return data;
        }

        public new static void Load(List<EquipmentData> loadedData)
        {
            map.Clear();
            mapByOwner.Clear();
            if (loadedData != null)
            {
                for (int i = 0; i < loadedData.Count; i++)
                {
                    EquipmentData model = loadedData[i];
                    if (model == null || model.key == null)
                        continue;
                    map[model.key] = model;
                    model.RegisterInOwnerGroups();
                }
            }
        }

        private void RegisterInOwnerGroups()
        {
            string owner = ownerUID ?? string.Empty;
            if (!mapByOwner.TryGetValue(owner, out List<EquipmentData> listByOwner))
            {
                listByOwner = new List<EquipmentData>();
                mapByOwner.Add(owner, listByOwner);
            }

            if (!listByOwner.Contains(this))
                listByOwner.Add(this);
        }

        public void Equip(string userUID)
        {
            if (!userUID.Equals(ownerUID))
                throw new EquipmentException(string.Format(ErrorCode.InvaildUserEquip, userUID));

            RegisterInOwnerGroups();
        }

        public EquipmentModel GetEquipmentModel()
        {
            EquipmentModel equipmentModel = null;
            EquipmentModel.map.TryGetValue(this.id, out equipmentModel);
            return equipmentModel;
        }

        public EquipmentData Clone()
        {
            EquipmentData equipmentInstance = new EquipmentData(this.id.ToString(), this.ownerUID);            
            return equipmentInstance;
        }
    }
}