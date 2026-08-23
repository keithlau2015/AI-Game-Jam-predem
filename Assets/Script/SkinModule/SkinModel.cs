using System.Collections.Generic;
namespace GenericGameModule
{
    public class SkinModel : Model<SkinModel>
    {
        public static SortedDictionary<string, List<SkinModel>> mapByEntity = new SortedDictionary<string, List<SkinModel>>();

        private string _entityID;
        public string entityID {
            get
            {
                return _entityID;
            }

            private set
            {
                _entityID = value;
                List<SkinModel> listByEntity = null;
                if (!mapByEntity.TryGetValue(_entityID, out listByEntity))
                {
                    listByEntity = new List<SkinModel>();
                    mapByEntity.Add(_entityID, listByEntity);
                }
                listByEntity.Add(this);
            }
        }
        public int sortIndex { get; private set; } = 0;
        /// <summary>
        /// Format: i_{id}
        /// </summary>
        public string iconID { get; private set; }

        #region 3D Model
        /// <summary>
        /// Format: m_{id}
        /// </summary>
        public string meshID { get; private set; }

        /// <summary>
        /// Format: t_{id}
        /// </summary>
        public string materialsIDs { get; private set; }

        public ushort defaultMaterialIndex { get; private set; }
        #endregion

        #region VFX
        public string spawnVFXPrefabKey { get; private set; }
        public string hurtVFXPrefabKey { get; private set; }
        public string destructVFXPrefabKey { get; private set; }
        #endregion

        #region SFX
        public string spawnSFXKey { get; private set; }
        public string hurtSFXKey { get; private set; }
        public string destructSFXKey { get; private set; }
        #endregion

        public SkinModel(object key) : base(key) { }

        public List<string> MaterialsIDList(char separator = '|')
        {
            return GenericMethod.ConvertStringToStringList(materialsIDs, separator);
        }
    }
}