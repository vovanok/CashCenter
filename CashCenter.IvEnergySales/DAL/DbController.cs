using System;
using System.Collections.Generic;
using FirebirdSql.Data.FirebirdClient;
using CashCenter.IvEnergySales.DataModel;
using CashCenter.IvEnergySales.DbQualification;
using CashCenter.IvEnergySales.Logging;
using System.Data.Common;
using System.Data;
using System.Text;

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
            DbDataReader dataReader = null;

            try
            {
                dbConnection.Open();

                var command = GetDbCommandByQuery(Sql.GET_REASONS);
                dataReader = command.ExecuteReader();

                var result = new List<PaymentReason>();
                while (dataReader.Read())
                {
                    int id = dataReader.GetFieldFromReader<int>(Sql.REASON_ID);
                    string name = dataReader.GetFieldFromReader<string>(Sql.REASON_NAME);
                    bool isCanPay = dataReader.GetFieldFromReader<int>(Sql.REASON_CANPAY) == 1;

                    result.Add(new PaymentReason(id, name, isCanPay));
                }

                return result;
            }
            catch (Exception e)
            {
                Log.ErrorWithException("Ошибка получения оснований для оплаты.", e);
                return new List<PaymentReason>();
            }
            finally
            {
                dataReader?.Close();
                dbConnection?.Close();
            }
        }

        public Customer GetCustomer(int customerId)
        {
            DbDataReader dataReader = null;

            try
            {
                dbConnection.Open();

                var now = DateTime.Now;
                var beginOfMonth = new DateTime(now.Year, now.Month, 1);
                var endOfCurrentDay = new DateTime(now.Year, now.Month, now.Day + 1);

                var command = GetDbCommandByQuery(Sql.GET_CUSTOMER);
                command.AddParameter(Sql.PARAM_CUSTOMER_ID, customerId);
                command.AddParameter(Sql.PARAM_START_DATE, beginOfMonth.ToShortDateString());
                command.AddParameter(Sql.PARAM_END_DATE, endOfCurrentDay.ToShortDateString());

                dataReader = command.ExecuteReader(CommandBehavior.SingleRow);

                Customer result = null;
                if (dataReader.Read())
                {
                    string name = dataReader.GetFieldFromReader< string>(Sql.CUSTOMER_NAME) ?? string.Empty;
                    string flat = dataReader.GetFieldFromReader<string>(Sql.CUSTOMER_FLAT, true) ?? string.Empty;
                    string buildingNumber = dataReader.GetFieldFromReader<string>(Sql.CUSTOMER_BUILDING_NUMBER) ?? string.Empty;
                    string streetName = dataReader.GetFieldFromReader<string>(Sql.CUSTOMER_STREET_NAME) ?? string.Empty;
                    string localityName = dataReader.GetFieldFromReader<string>(Sql.CUSTOMER_LOCALITY_NAME) ?? string.Empty;

                    int endDayValue = dataReader.GetFieldFromReader<int>(Sql.CUSTOMER_COUNTERS_END_DAY_VALUE);
                    int endNightValue = dataReader.GetFieldFromReader<int>(Sql.CUSTOMER_COUNTERS_END_NIGHT_VALUE, true);
                    bool isTwoTariff = dataReader.GetFieldFromReader<int>(Sql.CUSTOMER_COUNTERS_IS_TWO_TARIFF) == 1;

                    var customerCounters = new CustomerCounters(this, endDayValue, endNightValue, isTwoTariff);
                    result = new Customer(this, customerId, name, flat, buildingNumber, streetName, localityName, customerCounters);
                }

                return result;
            }
            catch (Exception e)
            {
                Log.ErrorWithException($"Ошибка получения лицевого счета с номером {customerId}.", e);
                return null;
            }
            finally
            {
                dataReader?.Close();
                dbConnection?.Close();
            }
        }

        public void Pay(Pay pay, int value1, int value2)
        {
            try
            {
                dbConnection.Open();

                var sbQuery = new StringBuilder();

                sbQuery.AppendLine("execute block as begin");
                sbQuery.AppendLine(Sql.ADD_PAYMENT_KIND_IF_NOTEXIST);
                sbQuery.AppendLine(Sql.ADD_OR_UPDATE_PAYJOURNAL);
                sbQuery.AppendLine(Sql.INSERT_PAY);
                sbQuery.AppendLine(Sql.ADD_COUNTERVALUES);
                sbQuery.AppendLine(Sql.ADD_METERS);
                sbQuery.AppendLine("end");

                var command = GetDbCommandByQuery(sbQuery.ToString());
                command.AddParameter(Sql.PARAM_CUSTOMER_ID, pay.Customer.Id);
                command.AddParameter(Sql.PARAM_PAYMENT_KIND_ID, pay.Journal.PaymentKind.Id);
                command.AddParameter(Sql.PARAM_PAYMENT_KIND_NAME, pay.Journal.PaymentKind.Name);
                command.AddParameter(Sql.PARAM_CREATE_DATE, pay.Journal.CreateDate.ToShortDateString());
                command.AddParameter(Sql.PARAM_PAYMENT_COST, pay.Cost);
                command.AddParameter(Sql.PARAM_PAY_JOURNAL_NAME, pay.Journal.Name);
                command.AddParameter(Sql.PARAM_REASON_ID, pay.ReasonId);
                command.AddParameter(Sql.PARAM_DESCRIPTION, pay.Description);
                command.AddParameter(Sql.PARAM_VALUE1, value1);
                command.AddParameter(Sql.PARAM_VALUE2, value2);

                command.ExecuteNonQuery();
            }
            catch(Exception e)
            {
                Log.ErrorWithException($"Ошибка записи информации о платеже.", e);
            }
            finally
            {
                dbConnection?.Close();
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
