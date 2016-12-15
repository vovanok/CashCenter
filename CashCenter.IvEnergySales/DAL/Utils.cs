using CashCenter.IvEnergySales.Logging;
using System;
using System.Data;
using System.Data.Common;

namespace CashCenter.IvEnergySales.DAL
{
    public static class Utils
    {
        public static T GetFieldFromReader<T>(this DbDataReader dataReader, string columnName, bool isMayBeNull = false)
        {
            var result = default(T);

            try
            {
                int ordinal = dataReader.GetOrdinal(columnName);
                if (dataReader.IsDBNull(ordinal))
                {
                    if (!isMayBeNull)
                        Log.Warning($"Не найдено поле в колонке {columnName}.");

                    return result;
                }

                if (typeof(T) == typeof(int))
                    return (T)Convert.ChangeType(dataReader.GetInt32(ordinal), typeof(T));

                if (typeof(T) == typeof(string))
                    return (T)Convert.ChangeType(dataReader.GetString(ordinal), typeof(T));

                return result;
            }
            catch (Exception e)
            {
                Log.ErrorWithException($"Ошибка получения поля в колонке {columnName}.", e);
                return result;
            }
        }

        public static void AddParameter<T>(this DbCommand command, string paramName, T paramValue)
        {
	        var commandParameter = GetCommandParameter<T>(command, paramName);
            commandParameter.Value = paramValue;
            command.Parameters.Add(commandParameter);
        }

		public static void AddParameter<T>(this DbCommand command, string paramName)
		{
			var commandParameter = GetCommandParameter<T>(command, paramName);
			commandParameter.Direction = ParameterDirection.Output;
			command.Parameters.Add(commandParameter);
		}

	    private static DbParameter GetCommandParameter<T>(DbCommand command, string paramName)
	    {
			var commandParameter = command.CreateParameter();
			commandParameter.ParameterName = paramName;

			if (typeof(T) == typeof(int))
			{
				commandParameter.DbType = DbType.Int32;
			}
			else if (typeof(T) == typeof(string))
			{
				commandParameter.DbType = DbType.String;
			}
			else if (typeof(T) == typeof(decimal))
			{
				commandParameter.DbType = DbType.Decimal;
			}

		    return commandParameter;
	    }
	}
}
