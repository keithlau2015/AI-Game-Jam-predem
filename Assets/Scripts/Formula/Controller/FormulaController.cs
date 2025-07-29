using AttributeSystem;
using System.Collections.Generic;

namespace FormulaModule {
    public abstract class FormulaController {
        public abstract double CalculateDamage(Dictionary<string, AttributeData> attributes);
    }
}