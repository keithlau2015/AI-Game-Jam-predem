using System;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;

public class TupleConverter
{
    public class TwoParam<T1, T2> : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var key = Convert.ToString(value).Trim('(').Trim(')');
            var parts = Regex.Split(key, (", "));
            var item1 = (T1)TypeDescriptor.GetConverter(typeof(T1)).ConvertFromInvariantString(parts[0])!;
            var item2 = (T2)TypeDescriptor.GetConverter(typeof(T2)).ConvertFromInvariantString(parts[1])!;
            return new ValueTuple<T1, T2>(item1, item2);
        }
    }
    
    public class ThreeParam<T1, T2, T3> : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var key = Convert.ToString(value).Trim('(').Trim(')');
            var parts = Regex.Split(key, (", "));
            var item1 = (T1)TypeDescriptor.GetConverter(typeof(T1)).ConvertFromInvariantString(parts[0])!;
            var item2 = (T2)TypeDescriptor.GetConverter(typeof(T2)).ConvertFromInvariantString(parts[1])!;
            var item3 = (T3)TypeDescriptor.GetConverter(typeof(T3)).ConvertFromInvariantString(parts[2])!;

            return new ValueTuple<T1, T2, T3>(item1, item2, item3);
        }
    }
    
}
