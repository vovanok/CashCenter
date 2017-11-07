using CashCenter.Common;
using System;
using System.Globalization;

namespace CashCenter.DataMigration.Dbf
{
    [AttributeUsage(AttributeTargets.Property)]
    public abstract class BaseDbfColumnAttribute : Attribute
    {
        public string Name { get; private set; }
        public abstract string DbfType { get; }
        public abstract Type DotnetType { get; }

        public BaseDbfColumnAttribute(string name)
        {
            Name = name;
        }

        public abstract string GetStringForQuery(object value);
        public abstract object ConvertDotnetTypeTo(Type targetType, object value);
    }

    public class NumericDbfColumnAttribute : BaseDbfColumnAttribute
    {
        public override string DbfType => "Numeric";
        public override Type DotnetType => typeof(double);

        private string customNumericFormat;

        public NumericDbfColumnAttribute(string columnName)
            : this(columnName, null)
        {
        }

        public NumericDbfColumnAttribute(string columnName, string customNumericFormat)
            : base(columnName)
        {
            this.customNumericFormat = customNumericFormat;
        }

        public override string GetStringForQuery(object value)
        {
            if (value is int)
            {
                return string.IsNullOrEmpty(customNumericFormat)
                    ? ((int)value).ToString(CultureInfo.InvariantCulture)
                    : ((int)value).ToString(customNumericFormat, CultureInfo.InvariantCulture);
            }
            
            if (value is decimal)
            {
                return string.IsNullOrEmpty(customNumericFormat)
                    ? ((decimal)value).ToString(CultureInfo.InvariantCulture)
                    : ((decimal)value).ToString(customNumericFormat, CultureInfo.InvariantCulture);
            }

            if (value is float)
            {
                return string.IsNullOrEmpty(customNumericFormat)
                    ? ((float)value).ToString(CultureInfo.InvariantCulture)
                    : ((float)value).ToString(customNumericFormat, CultureInfo.InvariantCulture);
            }
                    
            if (value is double)
            {
                return string.IsNullOrEmpty(customNumericFormat)
                    ? ((double)value).ToString(CultureInfo.InvariantCulture)
                    : ((double)value).ToString(customNumericFormat, CultureInfo.InvariantCulture);
            }

            return 0.ToString();
        }

        public override object ConvertDotnetTypeTo(Type targetType, object value)
        {
            if (targetType == null)
                return null;

            if (value == null)
                return Utils.GetDefault(targetType);

            return Convert.ChangeType(value, targetType);
        }
    }

    public class MoneyDbfColumnAttribute : NumericDbfColumnAttribute
    {
        public MoneyDbfColumnAttribute(string columnName)
            : base(columnName, "0.00")
        {
        }
    }

    public class CharacterDbfColumnAttribute : BaseDbfColumnAttribute
    {
        public override string DbfType => $"Character({size})";
        public override Type DotnetType => typeof(string);

        private int size;

        public CharacterDbfColumnAttribute(string columnName)
            : this(columnName, byte.MaxValue - 1)
        {
        }

        public CharacterDbfColumnAttribute(string columnName, int size)
            : base(columnName)
        {
            this.size = size >= 0 ? size : 0;
        }

        public override string GetStringForQuery(object value)
        {
            if (value == null)
                return "NULL";
            
            var valueStr = value.ToString();
            return $"'{valueStr.Substring(0, Math.Min(valueStr.Length, size))}'";
        }

        public override object ConvertDotnetTypeTo(Type targetType, object value)
        {
            if (targetType == null || value == null)
                return null;

            if (targetType == typeof(string))
                return value.ToString();

            return Utils.GetDefault(targetType);
        }
    }

    public class DateDbfColumnAttribute : BaseDbfColumnAttribute
    {
        public override string DbfType => string.IsNullOrEmpty(customDatetimeFormat) ? "Date" : "Character";
        public override Type DotnetType => typeof(string);

        private string customDatetimeFormat;

        public DateDbfColumnAttribute(string columnName)
            : this(columnName, null)
        {
        }

        public DateDbfColumnAttribute(string columnName, string customDatetimeFormat)
            : base(columnName)
        {
            this.customDatetimeFormat = customDatetimeFormat;
        }

        public override string GetStringForQuery(object value)
        {
            if (value is DateTime)
            {
                if ((DateTime)value == DateTime.MinValue)
                    return "NULL";

                if (!string.IsNullOrEmpty(customDatetimeFormat))
                    return $"'{((DateTime)value).ToString(customDatetimeFormat)}'";
            }

            return $"'{value}'";
        }

        public override object ConvertDotnetTypeTo(Type targetType, object value)
        {
            if (targetType == null)
                return null;

            if (value == null)
                return Utils.GetDefault(targetType);

            if (targetType == typeof(string))
                return value.ToString();

            if (targetType == typeof(DateTime))
            {
                if (value is DateTime)
                    return value;

                if (value is string)
                {
                    if (DateTime.TryParse(value as string, out DateTime result))
                        return result;
                }
            }

            return DateTime.MinValue;
        }
    }

    public class BooleanDbfColumnAttribute : CharacterDbfColumnAttribute
    {
        public BooleanDbfColumnAttribute(string columnName)
            : base(columnName)
        {
        }

        public override string GetStringForQuery(object value)
        {
            if (!(value is bool))
                return base.GetStringForQuery(value);

            return ((bool)value) ? "1" : "0";
        }

        public override object ConvertDotnetTypeTo(Type targetType, object value)
        {
            if (targetType == null)
                return null;

            if (value == null)
                return Utils.GetDefault(targetType);

            if (targetType == typeof(bool))
            {
                if (value is bool)
                    return value;

                if (value is string)
                {
                    if (bool.TryParse(value as string, out bool result))
                        return result;
                }
            }

            return false;
        }
    }
}