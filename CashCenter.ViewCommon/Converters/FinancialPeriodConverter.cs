using System;
using System.Globalization;
using System.Windows.Data;

namespace CashCenter.ViewCommon.Converters
{
    public class FinancialPeriodConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is int))
                return string.Empty;

            int intValue = (int)value;

            if (intValue <= 0)
                return string.Empty;

            int years = intValue / 12;
            int months = intValue % 12;

            return string.Format("{0} ({1}.{2})", intValue, months, years);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var strValue = value as string;
            if (strValue == null)
                return 0;

            var sections = strValue.Split(' ');
            if (sections.Length == 0)
                return 0;

            if (int.TryParse(sections[0], out int result))
                return result;

            return 0;
        }
    }
}
