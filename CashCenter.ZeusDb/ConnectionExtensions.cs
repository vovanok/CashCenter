using CashCenter.Common;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;

namespace CashCenter.ZeusDb
{
    public class DbFieldAttribute : Attribute
    {
        public string DbName { get; private set; }

        public DbFieldAttribute(string dbName)
        {
            DbName = dbName;
        }
    }

    public static class ConnectionExtensions
    {
        private static IEnumerable<TItemType> GetItems<TItemType>(this DbConnection connection, string sqlQuery) where TItemType : new()
        {
            DbDataReader dataReader = null;

            try
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.Transaction = connection.BeginTransaction();
                command.CommandText = sqlQuery;

                dataReader = command.ExecuteReader();

                var result = new List<TItemType>();

                var namePropertyMap = new Dictionary<string, PropertyInfo>();
                var propetiesInfo = typeof(TItemType).GetProperties();
                foreach (var propertyInfo in propetiesInfo)
                {
                    var dbFieldAttribute = propertyInfo.GetCustomAttributes(true).FirstOrDefault(attribute => attribute is DbFieldAttribute) as DbFieldAttribute;
                    if (dbFieldAttribute == null)
                        continue;

                    if (!namePropertyMap.ContainsKey(dbFieldAttribute.DbName))
                        namePropertyMap.Add(dbFieldAttribute.DbName, propertyInfo);
                }

                while (dataReader.Read())
                {
                    var currentItem = new TItemType();
                    foreach (var nameProperty in namePropertyMap)
                    {
                        //dataReader.GetFieldValue<>
                        //nameProperty.Value.SetValue(currentItem, dataReader.GetFieldFromReader<>(nameProperty.Key)); //nameProperty.Value.PropertyType
                    }

                    //int id = dataReader.GetFieldFromReader<int>(Sql.CUSTOMER_ID);
                    //string name = dataReader.GetFieldFromReader<string>(Sql.CUSTOMER_NAME) ?? string.Empty;
                    //string flat = dataReader.GetFieldFromReader<string>(Sql.CUSTOMER_FLAT, true) ?? string.Empty;
                    //string buildingNumber = dataReader.GetFieldFromReader<string>(Sql.CUSTOMER_BUILDING_NUMBER) ?? string.Empty;
                    //string streetName = dataReader.GetFieldFromReader<string>(Sql.CUSTOMER_STREET_NAME) ?? string.Empty;
                    //string localityName = dataReader.GetFieldFromReader<string>(Sql.CUSTOMER_LOCALITY_NAME) ?? string.Empty;

                    //result.Add(new Customer(id, DepartamentDef?.Code ?? string.Empty, name, flat, buildingNumber, streetName, localityName));
                }

                return result;
            }
            catch (Exception e)
            {
                Log.ErrorWithException("Ошибка получения данных.", e);
                return null;
            }
            finally
            {
                dataReader?.Close();
                connection?.Close();
            }
        }
    }
}
