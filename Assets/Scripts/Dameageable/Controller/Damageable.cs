using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DameageableModule {
    public abstract class Damageable : MonoBehaviour {
        private List<DamageablePart> _damageableParts;
        protected List<DamageablePart> damageableParts {
            get {
                if(_damageableParts == null) {
                    _damageableParts = GetComponentsInChildren<DamageablePart>().ToList();
                }
                return _damageableParts;
            }
        }

        public abstract double GetCurrentHp();

        protected void OnTriggerEnter(Collider other) {
            
        }

        protected void OnTriggerExit(Collider other) {
            
        }
    }
}