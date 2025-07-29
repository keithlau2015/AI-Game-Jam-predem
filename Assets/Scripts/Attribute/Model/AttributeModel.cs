using System;
using System.Collections.Generic;

namespace AttributeSystem
{
    public class AttributeModel : Model<AttributeModel>
    {
        public enum AttributeType
        {
            HP,
            MP,
            ATK,
            DEF,
            CRI,
            CRI_DMG,
            DODGE,
            ATK_SPD,
            SPD,
            CAP_SPD,
            CD,
            RANGE,
            EXP,
            LV,
            INSPECT_RANGE,
            COUNTER_INSPECT_RANGE,
            SHIELD,
            SHIELD_REGEN_SPD,
            HIT,
            SHIELD_EFFICIENCY
        }

        public int type;
        public double defaultValue;
        public double defaultMinValue;
        public double defaultMaxValue;
        public string iconID;
        public string backgroundID;
        public string frameID;  
        public string nameID;
        public string descriptionID;
    }
}