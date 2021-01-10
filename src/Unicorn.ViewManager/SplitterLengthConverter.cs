using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;

namespace Unicorn.ViewManager
{
    public class SplitterLengthConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            TypeCode typeCode = Type.GetTypeCode(sourceType);
            if ((uint)(typeCode - 7) <= 8u || typeCode == TypeCode.String)
            {
                return true;
            }
            return false;
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType != typeof(InstanceDescriptor))
            {
                return destinationType == typeof(string);
            }
            return true;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null || !CanConvertFrom(value.GetType()))
            {
                throw GetConvertFromException(value);
            }
            if (value is string)
            {
                return FromString((string)value, culture);
            }
            double num = Convert.ToDouble(value, culture);
            if (double.IsNaN(num))
            {
                num = 1.0;
            }
            return new SplitterLength(num, SplitterUnitType.Stretch);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException("destinationType");
            }
            if (value != null && value is SplitterLength)
            {
                SplitterLength length = (SplitterLength)value;
                if (destinationType == typeof(string))
                {
                    return ToString(length, culture);
                }
                if (destinationType.IsEquivalentTo(typeof(InstanceDescriptor)))
                {
                    return new InstanceDescriptor(typeof(SplitterLength).GetConstructor(new Type[2]
                    {
                    typeof(double),
                    typeof(SplitterUnitType)
                    }), new object[2]
                    {
                    length.Value,
                    length.SplitterUnitType
                    });
                }
            }
            throw GetConvertToException(value, destinationType);
        }

        internal static SplitterLength FromString(string s, CultureInfo cultureInfo)
        {
            string text = s.Trim();
            double value = 1.0;
            SplitterUnitType unitType = SplitterUnitType.Stretch;
            if (text == "*")
            {
                unitType = SplitterUnitType.Fill;
            }
            else
            {
                value = Convert.ToDouble(text, cultureInfo);
            }
            return new SplitterLength(value, unitType);
        }

        internal static string ToString(SplitterLength length, CultureInfo cultureInfo)
        {
            if (length.SplitterUnitType == SplitterUnitType.Fill)
            {
                return "*";
            }
            return Convert.ToString(length.Value, cultureInfo);
        }
    }

}
