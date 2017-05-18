using System;
using System.Data.Common;

namespace CashCenter.Common
{
    public static class CommonExtensions
    {
        public static T GetFieldFromReader<T>(this DbDataReader dataReader, string columnName)
        {
            int ordinal = dataReader.GetOrdinal(columnName);
            if (dataReader.IsDBNull(ordinal))
                return default(T);

            return dataReader.GetFieldValue<T>(ordinal);
        }

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
