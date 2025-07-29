using System.Linq;

namespace DameageableModule {
    //If all parts are destroyed, the object is destroyed
    public class CombinedDamageable : Damageable {
        public override double GetCurrentHp() {
            if(damageableParts.Count == 0) {
                return 0;
            }

            return damageableParts.Sum(part => part.GetHp());
        }
    }
}