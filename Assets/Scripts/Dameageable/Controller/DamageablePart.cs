using AttributeSystem;
using Model;
using UnityEngine;

namespace DameageableModule {
    public class DamageablePart : MonoBehaviour {
        private DamageablePartData damageablePartData;
        private AttributeData hp;
        private AttributeData def;

        public double GetHp() {
            return hp.value;
        }

        public void Create(DamageableModel damageModel) {
            this.damageablePartData = new DamageablePartData();
            this.damageablePartData.id = damageModel.key.ToString();
            this.hp = new AttributeData(damageModel.hp);
            this.damageablePartData.hpKey = hp.key.ToString();
            this.def = new AttributeData(damageModel.def);
            this.damageablePartData.defKey = def.key.ToString();
        }

        public void Load(DamageablePartData damageablePartData) {
            this.damageablePartData = damageablePartData;
            AttributeData.map.TryGetValue(damageablePartData.hpKey, out hp);
            AttributeData.map.TryGetValue(damageablePartData.defKey, out def);
        }
        
        protected void OnTriggerEnter(Collider other) {
            Damager damager = null;
            if(!other.TryGetComponent(out damager)) {
                return;
            }
            double damage = damager.GetDamage(new System.Collections.Generic.Dictionary<string, AttributeData>()
            {
                { AttributeModel.AttributeType.HP.ToString() ,hp},
                { AttributeModel.AttributeType.DEF.ToString() ,def}
            });
            hp.value -= damage;
        }

        protected void OnTriggerExit(Collider other) {
            
        }
    }
}