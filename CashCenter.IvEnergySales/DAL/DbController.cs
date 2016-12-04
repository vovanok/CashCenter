using System;
using System.Collections.Generic;
using FirebirdSql.Data.FirebirdClient;
using CashCenter.IvEnergySales.DataModel;
using CashCenter.IvEnergySales.DbQualification;
using CashCenter.IvEnergySales.Logging;
using System.Data.Common;

namespace CashCenter.IvEnergySales.DAL
{
    public class DbController
    {
        private const string DB_CONNECTION_STRING_FORMAT = "Server=localhost;User=SYSDBA;Password=masterkey;Database={0}";

        private DbConnection dbConnection;

        public DbModel Model { get; private set; }

        public DbController(DbModel dbModel)
        {
            this.Model = dbModel;
            this.dbConnection = new FbConnection(string.Format(DB_CONNECTION_STRING_FORMAT, dbModel.Path));
        }

        public List<PaymentReason> GetPaymentReasons()
        {
            try
            {
                dbConnection.Open();

                var command = GetDbCommandByQuery("select * from reason");
                var dataReader = command.ExecuteReader();

                var result = new List<PaymentReason>();
                while (dataReader.Read())
                {
                    int id = GetFieldFromReader<int>(dataReader, "ID");
                    string name = GetFieldFromReader<string>(dataReader, "NAME");
                    bool isCanPay = GetFieldFromReader<int>(dataReader, "CANPAY") == 1;

                    result.Add(new PaymentReason(id, name, isCanPay));
                }

                dataReader.Close();
                dbConnection.Close();

                return result;
            }
            catch (Exception e)
            {
                Log.Error($"Ошибка получения оснований для оплаты.\n{e.Message}\nStack trace:\n{e.StackTrace}");
                return new List<PaymentReason>();
            }
        }

        public Customer GetCustomer(int customerId)
        {
            try
            {
                dbConnection.Open();

                var command = GetDbCommandByQuery($"select ID, NAME from customer where id={customerId} and AGGR$STATE_ID <> 4");
                var dataReader = command.ExecuteReader();

                Customer result = null;
                while (dataReader.Read())
                {
                    int id = GetFieldFromReader<int>(dataReader, "ID");
                    string name = GetFieldFromReader<string>(dataReader, "NAME");

                    result = new Customer(id, name);
                }

                dataReader.Close();
                dbConnection.Close();

                return result;
            }
            catch (Exception e)
            {
                Log.Error($"Ошибка получения лицевого счета с номером {customerId}.\n{e.Message}\nStack trace:\n{e.StackTrace}");
                return null;
            }
        }

        private DbCommand GetDbCommandByQuery(string query)
        {
            var command = dbConnection.CreateCommand();
            command.Transaction = dbConnection.BeginTransaction();
            command.CommandText = query;
            return command;
        }

        private T GetFieldFromReader<T>(DbDataReader dataReader, string columnName)
        {
            var result = default(T);

            try
            {
                int ordinal = dataReader.GetOrdinal(columnName);
                if (dataReader.IsDBNull(ordinal))
                {
                    Log.Warning($"Не найдено поле в колонке {columnName}.");
                    return result;
                }

                if (typeof(T) == typeof(int))
                    return (T)Convert.ChangeType(dataReader.GetInt32(ordinal), typeof(T));

                if (typeof(T) == typeof(string))
                    return (T)Convert.ChangeType(dataReader.GetString(ordinal), typeof(T));

                return result;
            }
            catch(Exception e)
            {
                Log.Error($"Ошибка получения поля в колонке {columnName}.\n{e.Message}\nStack trace:\n{e.StackTrace}");
                return result;
            }
        }
    }
}
