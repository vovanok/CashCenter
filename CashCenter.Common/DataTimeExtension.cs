using System;

namespace CashCenter.Common
{
    public static class DataTimeExtension
    {
        public static DateTime DayBegin(this DateTime dateTime)
        {
            return dateTime.Date;
        }

        public static DateTime DayEnd(this DateTime dateTime)
        {
            return dateTime.Date.AddDays(1).AddSeconds(-1);
        }
    }
}
