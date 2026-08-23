using System;
using System.Numerics;

namespace AttributeModule
{
    public class AttributeData : Model<AttributeData>
    {
        public enum EditMode : int
        {
            Add = 0,
            Multiply = 1,
            Replace = 2
        }
        public BigInteger minValue { get; set; }
        public BigInteger value { get; set; }
        public BigInteger maxValue { get; set; }

        //Non Serializable Properties
        public event Action onValueMax;
        public event Action onValuePreChange;
        public event Action<int, BigInteger, BigInteger, BigInteger> onValuePostChange;
        public event Action onValueMin;

        public AttributeData(BigInteger initValue, BigInteger? maxInitValue = null) : base()
        {
            this.value = initValue;
            if (maxInitValue == null)
                this.maxValue = initValue;
            else
                this.maxValue = maxInitValue.Value;
        }

        public BigInteger SetValue(BigInteger value, EditMode editMode)
        {
            onValuePreChange?.Invoke();
            int dir = 0;
            if (value < 0)
                dir = -1;
            else if (value > 0)
                dir = 1;

            BigInteger overflow = 0;

            BigInteger result = 0;
            if (editMode == EditMode.Add)
            {
                result = this.value + value;
            }
            else if (editMode == EditMode.Multiply)
            {
                result = this.value * value;
            }
            else if (editMode == EditMode.Replace)
            {
                result = value;
            }

            if (result >= maxValue)
            {
                overflow = result - this.maxValue;
                this.value = this.maxValue;
                this.onValueMax?.Invoke();
            }
            else if (result <= minValue)
            {
                overflow = result + this.minValue;
                this.value = this.minValue;
                this.onValueMin?.Invoke();
            }
            else
            {
                this.value = result;
            }
            this.onValuePostChange?.Invoke(dir, value, this.value, this.maxValue);
            return overflow;
        }

        public void MaxValueGrow(BigInteger growValue, bool resotre, EditMode editMode)
        {
            if (editMode == EditMode.Add)
                this.maxValue += growValue;
            else if (editMode == EditMode.Multiply)
                this.maxValue *= growValue;
            if (resotre)
            {
                onValuePreChange?.Invoke();
                this.value = this.maxValue;
                onValuePostChange?.Invoke(1, growValue, this.value, maxValue);
                onValueMax?.Invoke();
            }
        }

        public void MinValueGrow(BigInteger growValue, EditMode editMode)
        {
            if (editMode == EditMode.Add)
                this.minValue += growValue;
            else if (editMode == EditMode.Multiply)
                this.minValue *= growValue;
        }

        public AttributeData Clone()
        {
            AttributeData attributeInstance = new AttributeData((int)this.value);
            attributeInstance.key = this.key;
            attributeInstance.minValue = this.minValue;
            attributeInstance.value = this.value;
            attributeInstance.maxValue = this.maxValue;
            return attributeInstance;
        }

        public void Clean()
        {
            onValueMax = null;
            onValuePreChange = null;
            onValuePostChange = null;
            onValueMin = null;
        }

        public bool IsMax()
        {
            return this.value == this.maxValue;
        }
    }
}