using System;
using System.ComponentModel;
using System.Globalization;

namespace Unicorn.ViewManager
{
    [TypeConverter(typeof(SplitterLengthConverter))]
    public struct SplitterLength : IEquatable<SplitterLength>
    {
        private double unitValue;

        private SplitterUnitType unitType;

        public SplitterUnitType SplitterUnitType => unitType;

        public double Value => unitValue;

        public bool IsFill => SplitterUnitType == SplitterUnitType.Fill;

        public bool IsStretch => SplitterUnitType == SplitterUnitType.Stretch;

        public SplitterLength(double value)
        {
            this = new SplitterLength(value, SplitterUnitType.Stretch);
        }

        public SplitterLength(double value, SplitterUnitType unitType)
        {
            unitValue = value;
            this.unitType = unitType;
        }

        public static bool operator ==(SplitterLength obj1, SplitterLength obj2)
        {
            if (obj1.SplitterUnitType == obj2.SplitterUnitType)
            {
                return obj1.Value == obj2.Value;
            }
            return false;
        }

        public static bool operator !=(SplitterLength obj1, SplitterLength obj2)
        {
            return !(obj1 == obj2);
        }

        public override bool Equals(object obj)
        {
            if (obj is SplitterLength)
            {
                return this == (SplitterLength)obj;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (int)((int)unitValue + unitType);
        }

        public bool Equals(SplitterLength other)
        {
            return this == other;
        }

        public override string ToString()
        {
            return SplitterLengthConverter.ToString(this, CultureInfo.InvariantCulture);
        }
    }

}
