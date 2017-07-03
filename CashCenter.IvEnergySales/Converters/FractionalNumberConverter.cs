using CashCenter.Common;
using System;
using System.Globalization;
using System.Windows.Data;

namespace CashCenter.IvEnergySales.Converters
{
    public class FractionalNumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((IFormattable)value).ToString("N2", null);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var strValue = value as string;
            if (string.IsNullOrEmpty(strValue))
                return 0;

            if (targetType == typeof(decimal))
                return StringUtils.ForseDecimalParse(strValue);

            if (targetType == typeof(double))
                return StringUtils.ForseDoubleParse(strValue);

            if (targetType == typeof(float))
                return StringUtils.ForseFloatParse(strValue);

            return 0;
        }
    }
}
