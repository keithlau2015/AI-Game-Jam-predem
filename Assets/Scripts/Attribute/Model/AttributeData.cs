using System;

namespace AttributeSystem
{
    public class AttributeData : Model<AttributeData>
    {
        public enum EditMode : int
        {
            Add = 0,
            Multiply = 1,
            Replace = 2
        }
        public double minValue { get; set; }
        public double value { get; set; }
        public double maxValue { get; set; }

        //Non Serializable Properties
        public event Action onValueMax;
        public event Action onValuePreChange;
        public event Action<int, double, double, double> onValuePostChange;
        public event Action onValueMin;

        public AttributeData(double initValue, double maxInitValue = -1) : base()
        {
            this.value = initValue;
            if (maxInitValue == -1)
                this.maxValue = initValue;
            else
                this.maxValue = maxInitValue;
        }

        public double SetValue(double value, EditMode editMode)
        {
            onValuePreChange?.Invoke();
            int dir = 0;
            if (value < 0)
                dir = -1;
            else if (value > 0)
                dir = 1;

            double overflow = 0;

            double result = 0;
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
            return Math.Abs(overflow);
        }

        public void MaxValueGrow(double growValue, bool resotre, EditMode editMode)
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

        public void MinValueGrow(double growValue, EditMode editMode)
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

        public void Reset()
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