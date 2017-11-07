using CashCenter.Common;
using System;
using System.IO;
using System.Linq;
using System.Data.OleDb;
using System.Collections.Generic;
using System.Reflection;
using System.Data.Common;

namespace CashCenter.DataMigration.Dbf
{
    public static class Dbf
    {
        private class AttributePropertyPair
        {
            public BaseDbfColumnAttribute Attribute { get; private set; }
            public PropertyInfo Property { get; private set; }

            public AttributePropertyPair(BaseDbfColumnAttribute attribute, PropertyInfo property)
            {
                Attribute = attribute;
                Property = property;
            }
        }

        public static void SaveToFile<T>(IEnumerable<T> records, string filename)
        {
            if (records == null || string.IsNullOrEmpty(filename))
                return;

            if (File.Exists(filename))
                File.Delete(filename);

            var fileInfo = new FileInfo(filename);
            var connectionString = string.Format(Config.DbfConnectionStringFormat, fileInfo.Directory.FullName);
            OleDbConnection dbfConnection = new OleDbConnection(connectionString);
            string dbfName = fileInfo.Name;

            var dbfColumns = GetObjectDbfColumnsAttributes<T>();

            try
            {
                dbfConnection.Open();

                // Create DB
                var command = dbfConnection.CreateCommand();
                string columnsForCreate = string.Join(", ", dbfColumns.Select(column => $"[{column.Attribute.Name}] {column.Attribute.DbfType}"));
                command.CommandText = $"create table {dbfName} ({columnsForCreate})";
                command.ExecuteNonQuery();
                command.Dispose();

                foreach (var record in records)
                {
                    //Store data
                    command = dbfConnection.CreateCommand();
                    string columnsForInsert = string.Join(", ", dbfColumns.Select(column => column.Attribute.GetStringForQuery(column.Property.GetValue(record))));
                    command.CommandText = $"insert into {dbfName} values ({columnsForInsert})";
                    command.ExecuteNonQuery();
                    command.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw new SystemException($"Ошибка создания DBF {filename}", ex);
            }
            finally
            {
                dbfConnection?.Close();
            }
        }

        public static List<T> LoadFromFile<T>(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                return new List<T>();

            var fileInfo = new FileInfo(filename);
            var connectionString = string.Format(Config.DbfConnectionStringFormat, fileInfo.Directory.FullName);
            OleDbConnection dbfConnection = new OleDbConnection(connectionString);
            string dbfName = fileInfo.Name;

            var dbfColumns = GetObjectDbfColumnsAttributes<T>();

            try
            {
                dbfConnection.Open();

                // Get all data from DBF by query
                var command = dbfConnection.CreateCommand();
                command.CommandText = $"select * from {dbfName}";
                DbDataReader dataReader = command.ExecuteReader();

                var resultRecords = new List<T>();
                while (dataReader.Read())
                {
                    var newItem = Activator.CreateInstance<T>();
                    foreach (var column in dbfColumns)
                    {
                        int ordinal = dataReader.GetOrdinal(column.Attribute.Name);
                        var columnValue = Utils.GetDefault(column.Attribute.DotnetType);
                        if (!dataReader.IsDBNull(ordinal))
                        {
                            columnValue = dataReader.GetValue(ordinal);
                        }

                        // Set value to object
                        column.Property.SetValue(newItem, column.Attribute.ConvertDotnetTypeTo(column.Property.PropertyType, columnValue));
                    }

                    resultRecords.Add(newItem);
                }

                return resultRecords;
            }
            catch (Exception ex)
            {
                throw new SystemException($"Ошибка чтения DBF {dbfName}", ex);
            }
            finally
            {
                dbfConnection?.Close();
            }
        }

        private static List<AttributePropertyPair> GetObjectDbfColumnsAttributes<T>()
        {
            var attrToPropMap = new List<AttributePropertyPair>();
            foreach (var property in typeof(T).GetProperties())
            {
                var dbfColumnAttr = property.GetCustomAttribute<BaseDbfColumnAttribute>();
                if (dbfColumnAttr != null)
                {
                    attrToPropMap.Add(new AttributePropertyPair(dbfColumnAttr, property));
                }
            }

            return attrToPropMap;
        }
    }
}
