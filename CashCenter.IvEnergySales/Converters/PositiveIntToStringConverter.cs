using System;
using System.Globalization;
using System.Windows.Data;

namespace CashCenter.IvEnergySales.Converters
{
    public class PositiveIntToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is uint)
            {
                uint uintValue = (uint)value;

                if (uintValue == 0)
                    return string.Empty;

                return uintValue.ToString();
            }

            if (value is int)
            {
                int intValue = (int)value;
                if (intValue == 0)
                    return string.Empty;

                return intValue.ToString();
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var strValue = value as string;
            if (strValue == null)
                return 0;

            if (uint.TryParse(strValue, out uint uintResult))
                return uintResult;

            if (int.TryParse(strValue, out int intResult))
                return intResult;

            return 0;
        }
    }
}
