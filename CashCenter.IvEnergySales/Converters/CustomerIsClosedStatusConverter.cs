using System;
using System.Globalization;
using System.Windows.Data;

namespace CashCenter.IvEnergySales.Converters
{
    public class CustomerIsClosedStatusConverter : IValueConverter
    {
        private const string STATE_OPEN = "Открыт";
        private const string STATE_CLOSE = "Закрыт";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool?))
                return string.Empty;

            bool? boolValue = (bool?)value;

            if (boolValue == null)
                return string.Empty;

            if (boolValue.Value)
                return STATE_CLOSE;

            return STATE_OPEN;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var strValue = value as string;
            if (strValue == null)
                return null;

            switch (strValue)
            {
                case STATE_OPEN:
                    return false;
                case STATE_CLOSE:
                    return true;
                default:
                    return null;
            }
        }
    }
}
