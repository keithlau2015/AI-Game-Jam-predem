using AttributeSystem;
using FormulaModule;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace DameageableModule {
    public class Damager : MonoBehaviour {
        private Dictionary<string, AttributeData> data = new Dictionary<string, AttributeData>();
        private FormulaController formulaController;

        public void SetUp(Dictionary<string, AttributeData> data, FormulaController formulaController) {
            this.data.AddRange(data);
            this.formulaController = formulaController;
        }

        public double GetDamage(Dictionary<string, AttributeData> data) {
            this.data.AddRange(data);
            double damage = formulaController.CalculateDamage(this.data);
            return damage;
        }

        private void OnTriggerEnter(Collider other) {
            
        }

        private void OnTriggerExit(Collider other) {

        }
    }
}