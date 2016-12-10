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

                var command = GetDbCommandByQuery(SqlHelper.GetSqlForGetReasons());
                var dataReader = command.ExecuteReader();

                var result = new List<PaymentReason>();
                while (dataReader.Read())
                {
                    int id = dataReader.GetFieldFromReader<int>(SqlHelper.REASON_ID);
                    string name = dataReader.GetFieldFromReader<string>(SqlHelper.REASON_NAME);
                    bool isCanPay = dataReader.GetFieldFromReader<int>(SqlHelper.REASON_CANPAY) == 1;

                    result.Add(new PaymentReason(id, name, isCanPay));
                }

                dataReader.Close();
                dbConnection.Close();

                return result;
            }
            catch (Exception e)
            {
                Log.ErrorWithException("Ошибка получения оснований для оплаты.", e);
                return new List<PaymentReason>();
            }
        }

        public Customer GetCustomer(int customerId)
        {
            try
            {
                dbConnection.Open();

                var now = DateTime.Now;
                var beginOfMonth = new DateTime(now.Year, now.Month, 1);
                var endOfCurrentDay = new DateTime(now.Year, now.Month, now.Day + 1);

                var command = GetDbCommandByQuery(string.Format(SqlHelper.GetSqlForGetCustomer(customerId, beginOfMonth, endOfCurrentDay), customerId));
                var dataReader = command.ExecuteReader(CommandBehavior.SingleRow);

                Customer result = null;
                if (dataReader.Read())
                {
                    string name = dataReader.GetFieldFromReader< string>(SqlHelper.CUSTOMER_NAME) ?? string.Empty;
                    string flat = dataReader.GetFieldFromReader<string>(SqlHelper.CUSTOMER_FLAT, true) ?? string.Empty;
                    string buildingNumber = dataReader.GetFieldFromReader<string>(SqlHelper.CUSTOMER_BUILDING_NUMBER) ?? string.Empty;
                    string streetName = dataReader.GetFieldFromReader<string>(SqlHelper.CUSTOMER_STREET_NAME) ?? string.Empty;
                    string localityName = dataReader.GetFieldFromReader<string>(SqlHelper.CUSTOMER_LOCALITY_NAME) ?? string.Empty;

                    int endDayValue = dataReader.GetFieldFromReader<int>(SqlHelper.CUSTOMER_COUNTERS_END_DAY_VALUE);
                    int endNightValue = dataReader.GetFieldFromReader<int>(SqlHelper.CUSTOMER_COUNTERS_END_NIGHT_VALUE, true);
                    bool isTwoTariff = dataReader.GetFieldFromReader<int>(SqlHelper.CUSTOMER_COUNTERS_IS_TWO_TARIFF) == 1;

                    var customerCounters = new CustomerCounters(this, endDayValue, endNightValue, isTwoTariff);
                    result = new Customer(this, customerId, name, flat, buildingNumber, streetName, localityName, customerCounters);
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
    }
}
