using System;
using System.Globalization;
using System.Windows.Data;

namespace CashCenter.IvEnergySales.Converters
{
    public class CustomerNumberToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is uint))
                return string.Empty;

            uint uintValue = (uint)value;

            if (uintValue == 0)
                return string.Empty;

            return uintValue.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var strValue = value as string;
            if (strValue == null)
                return 0;

            if (uint.TryParse(strValue, out uint result))
                return result;

            return 0;
        }
    }
}
