using Model;
using SaveLoadModule;

namespace DameageableModule {
    public class DamageablePartData : SaveableModel<DamageablePartData> {
        public string id;
        public string hpKey;
        public string defKey;

        public DamageableModel damageableModel {
            get {
                DamageableModel damageableModel = null;
                if(DamageableModel.map.TryGetValue(id, out damageableModel)) {
                    return damageableModel;
                }
                return null;
            }
        }
    }
}
