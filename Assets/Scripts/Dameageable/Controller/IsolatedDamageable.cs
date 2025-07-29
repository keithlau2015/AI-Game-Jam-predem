using AttributeSystem;

namespace DameageableModule {
    //If all parts are destroyed, the object wont be destroyed
    public class IsolatedDamageable : Damageable {        
        private AttributeData hp;
        private AttributeData def;

        public override double GetCurrentHp() {
            return hp.value;
        }
    }
}
