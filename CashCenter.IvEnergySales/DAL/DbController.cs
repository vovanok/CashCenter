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
                    string name = dataReader.GetFieldFromReader<string>(Sql.CUSTOMER_NAME) ?? string.Empty;
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

	    public PaymentKind GetPaymentKind(string paymentKindName)
	    {
			DbDataReader dataReader = null;

			try
		    {
				dbConnection.Open();

			    var command = GetDbCommandByQuery(Sql.GET_PAYMENT_KIND);
				command.AddParameter(Sql.PARAM_PAYMENT_KIND_NAME, paymentKindName);

			    dataReader = command.ExecuteReader(CommandBehavior.SingleRow);

			    PaymentKind resultPaymentKind = null;
			    if (dataReader.Read())
			    {
				    var id = dataReader.GetFieldFromReader<int>(Sql.PAYMENT_KIND_ID);
				    var name = dataReader.GetFieldFromReader<string>(Sql.PAYMENT_KIND_NAME);

					resultPaymentKind = new PaymentKind(id, name);
				}

			    return resultPaymentKind;
		    }
		    catch (Exception e)
		    {
				Log.ErrorWithException("Ошибка получения типа платежа.", e);
			    return null;
		    }
		    finally
		    {
				dataReader?.Close();
				dbConnection?.Close();
		    }
	    }

	    public PaymentKind AddPaymentKind(PaymentKind paymentKind)
	    {
		    try
		    {
				dbConnection.Open();

				var command = GetDbCommandByQuery(Sql.INSERT_PAYMENT_KIND);
			    command.AddParameter(Sql.PARAM_PAYMENT_KIND_NAME, paymentKind.Name);

				int id = (int)command.ExecuteScalar();

				command.Transaction.Commit();
				command.Dispose();

				return new PaymentKind(id, paymentKind.Name);
			}
			catch (Exception e)
		    {
				Log.ErrorWithException($"Ошибка добавления типа платежа.", e);
			    return null;
		    }
		    finally
		    {
			    dbConnection?.Close();
		    }
	    }

	    public PayJournal GetPayJournal(DateTime createDate, int paymentKindId)
	    {
			DbDataReader dataReader = null;

			try
			{
				dbConnection.Open();

				var command = GetDbCommandByQuery(Sql.GET_PAYJOURNAL);
				command.AddParameter(Sql.PARAM_CREATE_DATE, createDate.ToShortDateString());
				command.AddParameter(Sql.PARAM_PAYMENT_KIND_ID, paymentKindId);

				dataReader = command.ExecuteReader(CommandBehavior.SingleRow);

				PayJournal resultPayJournal = null;
				if (dataReader.Read())
				{
					var id = dataReader.GetFieldFromReader<int>(Sql.PAY_JOURNAL_ID);
					var name = dataReader.GetFieldFromReader<string>(Sql.PAY_JOURNAL_NAME);

					resultPayJournal = new PayJournal(id, name, createDate, paymentKindId);
				}

				return resultPayJournal;
			}
			catch (Exception e)
			{
				Log.ErrorWithException("Ошибка получения журнала платежа.", e);
				return null;
			}
			finally
			{
				dataReader?.Close();
				dbConnection?.Close();
			}
		}

	    public void UpdatePayJournal(decimal cost, int payJournalId)
	    {
			try
			{
				dbConnection.Open();

				var command = GetDbCommandByQuery(Sql.UPDATE_PAYJOURNAL);
				command.AddParameter(Sql.PARAM_PAYMENT_COST, cost);
				command.AddParameter(Sql.PARAM_PAY_JOURNAL_ID, payJournalId);

				command.ExecuteNonQuery();
				command.Transaction.Commit();
				command.Dispose();
			}
			catch (Exception e)
			{
				Log.ErrorWithException($"Ошибка обновления журнала платежа.", e);
			}
			finally
			{
				dbConnection?.Close();
			}
		}

	    public PayJournal AddPayJournal(PayJournal payJournal, decimal cost)
	    {
			try
			{
				dbConnection.Open();

				var command = GetDbCommandByQuery(Sql.INSERT_PAY_JOUNAL);
				command.AddParameter(Sql.PARAM_PAY_JOURNAL_NAME, payJournal.Name);
				command.AddParameter(Sql.PARAM_CREATE_DATE, payJournal.CreateDate.ToShortDateString());
				command.AddParameter(Sql.PARAM_PAYMENT_KIND_ID, payJournal.PaymentKindId);
				command.AddParameter(Sql.PARAM_PAYMENT_COST, cost);

				int id = (int)command.ExecuteScalar();

				command.Transaction.Commit();
				command.Dispose();

				return new PayJournal(id, payJournal.Name, payJournal.CreateDate, payJournal.PaymentKindId);
			}
			catch (Exception e)
			{
				Log.ErrorWithException($"Ошибка добавления журнала платежа.", e);
				return null;
			}
			finally
			{
				dbConnection?.Close();
			}
		}

	    public Pay AddPay(Pay pay)
	    {
		    try
		    {
			    dbConnection.Open();

			    var command = GetDbCommandByQuery(Sql.INSERT_PAY);
			    command.AddParameter(Sql.PARAM_CUSTOMER_ID, pay.CustomerId);
			    command.AddParameter(Sql.PARAM_PAY_JOURNAL_ID, pay.JournalId);
			    command.AddParameter(Sql.PARAM_REASON_ID, pay.ReasonId);
			    command.AddParameter(Sql.PARAM_PAYMENT_COST, pay.Cost);
			    command.AddParameter(Sql.PARAM_DESCRIPTION, pay.Description);

				int id = (int)command.ExecuteScalar();

				command.Transaction.Commit();
				command.Dispose();

				return new Pay(id, pay.CustomerId, pay.ReasonId, pay.JournalId,
					pay.Cost, pay.Description);
		    }
		    catch (Exception e)
		    {
			    Log.ErrorWithException($"Ошибка добавления журнала платежа.", e);
			    return null;
		    }
		    finally
		    {
			    dbConnection?.Close();
		    }
	    }

	    public int GetCustomerCounterId(int customerId)
	    {
			DbDataReader dataReader = null;

			try
			{
				dbConnection.Open();

				var command = GetDbCommandByQuery(Sql.SELECT_CUSTOMER_COUNTER);
				command.AddParameter(Sql.PARAM_CUSTOMER_ID, customerId);

				dataReader = command.ExecuteReader(CommandBehavior.SingleRow);

				int id = -1;
				if (dataReader.Read())
				{
					id = dataReader.GetFieldFromReader<int>(Sql.CUSTOMER_COUNTER_ID);
				}

				return id;
			}
			catch (Exception e)
			{
				Log.ErrorWithException("Ошибка получения счетчика плательщика.", e);
				return -1;
			}
			finally
			{
				dataReader?.Close();
				dbConnection?.Close();
			}
		}

	    public CounterValues AddCounterValues(CounterValues counterValues, DateTime createDate)
	    {
			try
			{
				dbConnection.Open();

				var command = GetDbCommandByQuery(Sql.INSERT_COUNTERVALUES);
				command.AddParameter(Sql.PARAM_CUSTOMER_ID, counterValues.CustomerId);
				command.AddParameter(Sql.PARAM_CUSTOMER_COUNTER_ID, counterValues.CustomerCounterId);
				command.AddParameter(Sql.PARAM_CREATE_DATE, createDate.ToShortDateString());
				command.AddParameter(Sql.PARAM_VALUE1, counterValues.Value1);
				command.AddParameter(Sql.PARAM_VALUE2, counterValues.Value2);

				int id = (int)command.ExecuteScalar();

				command.Transaction.Commit();
				command.Dispose();

				return new CounterValues(id, counterValues.CustomerId,
					counterValues.CustomerCounterId, counterValues.Value1,
					counterValues.Value2);
			}
			catch (Exception e)
			{
				Log.ErrorWithException($"Ошибка добавления журнала платежа.", e);
				return null;
			}
			finally
			{
				dbConnection?.Close();
			}
		}

	    public Meter AddMeters(Meter meter)
	    {
			try
			{
				dbConnection.Open();

				var command = GetDbCommandByQuery(Sql.INSERT_METERS);
				command.AddParameter(Sql.PARAM_CUSTOMER_ID, meter.CustomerId);
				command.AddParameter(Sql.PARAM_CUSTOMER_COUNTER_ID, meter.CustomerCounterId);
				command.AddParameter(Sql.PARAM_VALUE1, meter.Value1);
				command.AddParameter(Sql.PARAM_VALUE2, meter.Value2);
				command.AddParameter(Sql.PARAM_COUNTER_VALUES_ID, meter.CounterValuesId);

				int id = (int)command.ExecuteScalar();

				command.Transaction.Commit();
				command.Dispose();

				return new Meter(id, meter.CustomerId, meter.CustomerCounterId,
					meter.Value1, meter.Value2, meter.CounterValuesId);
			}
			catch (Exception e)
			{
				Log.ErrorWithException($"Ошибка добавления журнала платежа.", e);
				return null;
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
