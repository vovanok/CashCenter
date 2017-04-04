using System.Data;
using System.Data.Common;

namespace CashCenter.Common
{
    public static class DbUtils
    {
        public static T GetFieldFromReader<T>(this DbDataReader dataReader, string columnName)
        {
            int ordinal = dataReader.GetOrdinal(columnName);
            if (dataReader.IsDBNull(ordinal))
                return default(T);

            return dataReader.GetFieldValue<T>(ordinal);
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

            var parameterType = typeof(T);
            if (parameterType == typeof(int) || parameterType == typeof(int?))
			{
				commandParameter.DbType = DbType.Int32;
			}
			else if (parameterType == typeof(string))
			{
				commandParameter.DbType = DbType.String;
			}
			else if (parameterType == typeof(decimal) || parameterType == typeof(decimal?))
			{
				commandParameter.DbType = DbType.Decimal;
			}

		    return commandParameter;
	    }
	}
}
