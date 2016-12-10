using System;
using System.Collections.Generic;
using FirebirdSql.Data.FirebirdClient;
using CashCenter.IvEnergySales.DataModel;
using CashCenter.IvEnergySales.DbQualification;
using CashCenter.IvEnergySales.Logging;
using System.Data.Common;
using System.Data;

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

                var command = GetDbCommandByQuery(string.Format(SqlConsts.SQL_GET_CUSTOMER_FORMAT, customerId));
                var dataReader = command.ExecuteReader(CommandBehavior.SingleRow);

                Customer result = null;
                if (dataReader.Read())
                {
                    string name = GetFieldFromReader<string>(dataReader, SqlConsts.CUSTOMER_NAME) ?? string.Empty;
                    string flat = GetFieldFromReader<string>(dataReader, SqlConsts.CUSTOMER_FLAT, true) ?? string.Empty;
                    string buildingNumber = GetFieldFromReader<string>(dataReader, SqlConsts.CUSTOMER_BUILDING_NUMBER) ?? string.Empty;
                    string streetName = GetFieldFromReader<string>(dataReader, SqlConsts.CUSTOMER_STREET_NAME) ?? string.Empty;
                    string localityName = GetFieldFromReader<string>(dataReader, SqlConsts.CUSTOMER_LOCALITY_NAME) ?? string.Empty;
                    CustomerCounters customerCounters = GetCustomerCountersForCurrentConnection(customerId);

                    result = new Customer(this, customerId, name, flat, buildingNumber, streetName, localityName);
                }

                dataReader.Close();
                dbConnection.Close();

                return result;
            }
            catch (Exception e)
            {
                Log.ErrorWithException($"Ошибка получения лицевого счета с номером {customerId}.", e);
                return null;
            }
        }

        private CustomerCounters GetCustomerCountersForCurrentConnection(int customerId)
        {
            try
            {
                var now = DateTime.Now;
                var beginOfMonth = new DateTime(now.Year, now.Month, 1);
                var endOfCurrentDay = new DateTime(now.Year, now.Month, now.Day + 1);

                var command = GetDbCommandByQuery(string.Format(SqlConsts.SQL_GET_COUNTERS_FORMAT, customerId, beginOfMonth, endOfCurrentDay));
                var dataReader = command.ExecuteReader(CommandBehavior.SingleRow);

                CustomerCounters result = null;

                if (dataReader.Read())
                {
                    int endDayValue = GetFieldFromReader<int>(dataReader, SqlConsts.COUNTERS_END_DAY_VALUE);
                    int endNightValue = GetFieldFromReader<int>(dataReader, SqlConsts.COUNTERS_END_NIGHT_VALUE);
                    bool isTwoTariff = GetFieldFromReader<int>(dataReader, SqlConsts.COUNTERS_IS_TWO_TARIFF) == 1;

                    result = new CustomerCounters(this, endDayValue, endNightValue, isTwoTariff);
                }

                dataReader.Close();

                return result;
            }
            catch(Exception e)
            {
                Log.ErrorWithException($"Ошибка получения последний показаний счетчиков для плательщика с номером лицевого счета {customerId}.", e);
                return null;
            }
        }

        private DbCommand GetDbCommandByQuery(string query)
        {
            if (dbConnection.State != ConnectionState.Open)
            {
                Log.Error("Ошибка получения комманды для текущего подключения к БД. Подключение не открыто.");
                return null;
            }

            var command = dbConnection.CreateCommand();
            command.Transaction = dbConnection.BeginTransaction();
            command.CommandText = query;
            return command;
        }

        private T GetFieldFromReader<T>(DbDataReader dataReader, string columnName, bool isMayBeNull = false)
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
            catch(Exception e)
            {
                Log.ErrorWithException($"Ошибка получения поля в колонке {columnName}.", e);
                return result;
            }
        }
    }
}
