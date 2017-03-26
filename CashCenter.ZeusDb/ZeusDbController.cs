using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using FirebirdSql.Data.FirebirdClient;
using CashCenter.ZeusDb.Entities;
using CashCenter.Common;
using CashCenter.Common.DbQualification;

namespace CashCenter.ZeusDb
{
    public partial class ZeusDbController
    {
        private DbConnection dbConnection;

        public DepartmentDef DepartamentDef { get; private set; }

        public ZeusDbController(DepartmentDef departamentDef)
        {
            this.DepartamentDef = departamentDef;
            this.dbConnection = new FbConnection(string.Format(Config.DbConnectionStringFormat, departamentDef.Url, departamentDef.Path));
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
                    bool isCanPay = dataReader.GetFieldFromReader<string>(Sql.REASON_CANPAY) == "1";

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

        public List<Customer> GetCustomers()
        {
            DbDataReader dataReader = null;

            try
            {
                dbConnection.Open();

                var command = GetDbCommandByQuery(Sql.GET_CUSTOMERS);
                dataReader = command.ExecuteReader();

                var result = new List<Customer>();
                while (dataReader.Read())
                {
                    int id = dataReader.GetFieldFromReader<int>(Sql.CUSTOMER_ID);
                    string name = dataReader.GetFieldFromReader<string>(Sql.CUSTOMER_NAME) ?? string.Empty;
                    string flat = dataReader.GetFieldFromReader<string>(Sql.CUSTOMER_FLAT) ?? string.Empty;
                    string buildingNumber = dataReader.GetFieldFromReader<string>(Sql.CUSTOMER_BUILDING_NUMBER) ?? string.Empty;
                    string streetName = dataReader.GetFieldFromReader<string>(Sql.CUSTOMER_STREET_NAME) ?? string.Empty;
                    string localityName = dataReader.GetFieldFromReader<string>(Sql.CUSTOMER_LOCALITY_NAME) ?? string.Empty;

                    result.Add(new Customer(id, DepartamentDef?.Code ?? string.Empty, name, flat, buildingNumber, streetName, localityName));
                }

                return result;
            }
            catch (Exception e)
            {
                Log.ErrorWithException("Ошибка получения физических лиц.", e);
                return new List<Customer>();
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

                var command = GetDbCommandByQuery(Sql.GET_CUSTOMER);
                command.AddParameter(Sql.PARAM_CUSTOMER_ID, customerId);

                dataReader = command.ExecuteReader(CommandBehavior.SingleRow);

                Customer result = null;
                if (dataReader.Read())
                {
                    string name = dataReader.GetFieldFromReader<string>(Sql.CUSTOMER_NAME) ?? string.Empty;
                    string flat = dataReader.GetFieldFromReader<string>(Sql.CUSTOMER_FLAT) ?? string.Empty;
                    string buildingNumber = dataReader.GetFieldFromReader<string>(Sql.CUSTOMER_BUILDING_NUMBER) ?? string.Empty;
                    string streetName = dataReader.GetFieldFromReader<string>(Sql.CUSTOMER_STREET_NAME) ?? string.Empty;
                    string localityName = dataReader.GetFieldFromReader<string>(Sql.CUSTOMER_LOCALITY_NAME) ?? string.Empty;

                    result = new Customer(customerId, DepartamentDef?.Code ?? string.Empty, name, flat, buildingNumber, streetName, localityName);
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

        public CustomerCounters GetCustomerCounterValues(int customerId, DateTime beginDate, DateTime endDate)
        {
            DbDataReader dataReader = null;

            try
            {
                dbConnection.Open();

                var command = GetDbCommandByQuery(Sql.GET_CUSTOMER_COUNTER_VALUES);
                command.AddParameter(Sql.PARAM_CUSTOMER_ID, customerId);
                command.AddParameter(Sql.PARAM_START_DATE, beginDate.ToShortDateString());
                command.AddParameter(Sql.PARAM_END_DATE, endDate.ToShortDateString());

                dataReader = command.ExecuteReader(CommandBehavior.SingleRow);

                CustomerCounters result = null;
                if (dataReader.Read())
                {
                    string counterName = dataReader.GetFieldFromReader<string>(Sql.COUNTER_NAME);
                    int endDayValue = dataReader.GetFieldFromReader<int>(Sql.CUSTOMER_COUNTERS_END_DAY_VALUE);
                    int endNightValue = dataReader.GetFieldFromReader<int>(Sql.CUSTOMER_COUNTERS_END_NIGHT_VALUE);
                    bool isTwoTariff = dataReader.GetFieldFromReader<string>(Sql.CUSTOMER_COUNTERS_IS_TWO_TARIFF) == "1";

                    if (counterName == null)
                        return null;

                    result = new CustomerCounters(customerId, endDayValue, endNightValue, isTwoTariff);
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

        public Debt GetDebt(int customerId, int dayEncoding)
        {
            DbDataReader dataReader = null;

            try
            {
                dbConnection.Open();

                var command = GetDbCommandByQuery(Sql.SELECT_DEBT);
                command.AddParameter(Sql.PARAM_CUSTOMER_ID, customerId);
                command.AddParameter(Sql.PARAM_DAY_ENCODING, dayEncoding);

                dataReader = command.ExecuteReader(CommandBehavior.SingleRow);

                Debt debt = null;
                if (dataReader.Read())
                {
                    var balance = dataReader.GetFieldFromReader<decimal>(Sql.END_BALANCE);
                    var penalty = dataReader.GetFieldFromReader<decimal>(Sql.PENALTY_BALANCE);

                    debt = new Debt(customerId, balance, penalty);
                }

                return debt;
            }
            catch (Exception e)
            {
                Log.ErrorWithException($"Ошибка получения задолжности для плательщика {customerId}.", e);
                return null;
            }
            finally
            {
                dataReader?.Close();
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

        public void AddRequireToPayJournal(PayJournal payJournal, decimal additionCost)
        {
            try
            {
                dbConnection.Open();

                var command = GetDbCommandByQuery(Sql.UPDATE_PAYJOURNAL);
                command.AddParameter(Sql.PARAM_PAYMENT_COST, additionCost);
                command.AddParameter(Sql.PARAM_PAY_JOURNAL_ID, payJournal.Id);

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
                command.AddParameter(Sql.PARAM_METERS_ID, pay.MetersId);
                command.AddParameter(Sql.PARAM_PAYMENT_COST, pay.Cost);
                command.AddParameter(Sql.PARAM_PENALTY_TOTAL, pay.PenaltyTotal);
                command.AddParameter(Sql.PARAM_DESCRIPTION, pay.Description);

                int id = (int)command.ExecuteScalar();

                command.Transaction.Commit();
                command.Dispose();

                return new Pay(id, pay.CustomerId, pay.ReasonId, pay.MetersId, pay.JournalId,
                    pay.Cost, pay.PenaltyTotal, pay.Description);
            }
            catch (Exception e)
            {
                Log.ErrorWithException($"Ошибка добавления платежа.", e);
                return null;
            }
            finally
            {
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
                Log.ErrorWithException($"Ошибка добавления показаний счетчиков.", e);
                return null;
            }
            finally
            {
                dbConnection?.Close();
            }
        }

        public void UpdateCounterValuesPayId(int counterValuesId, int payId)
        {
            try
            {
                dbConnection.Open();

                var command = GetDbCommandByQuery(Sql.UPDATE_COUNTERVALUES_PAY_ID);
                command.AddParameter(Sql.PARAM_COUNTER_VALUES_ID, counterValuesId);
                command.AddParameter(Sql.PARAM_PAY_ID, payId);

                command.ExecuteNonQuery();
                command.Transaction.Commit();
                command.Dispose();
            }
            catch (Exception e)
            {
                Log.ErrorWithException($"Ошибка обновления контрольных значений.", e);
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
                Log.ErrorWithException($"Ошибка добавления показаний.", e);
                return null;
            }
            finally
            {
                dbConnection?.Close();
            }
        }

        public List<Warehouse> GetWarehouses()
        {
            DbDataReader dataReader = null;

            try
            {
                dbConnection.Open();

                var command = GetDbCommandByQuery(Sql.GET_WAREHOUSES);
                dataReader = command.ExecuteReader();

                var result = new List<Warehouse>();
                while (dataReader.Read())
                {
                    int id = dataReader.GetFieldFromReader<int>(Sql.WAREHOUSE_ID);
                    string code = dataReader.GetFieldFromReader<string>(Sql.WAREHOUSE_CODE);
                    string name = dataReader.GetFieldFromReader<string>(Sql.WAREHOUSE_NAME);
                    float quantity = dataReader.GetFieldFromReader<float>(Sql.WAREHOUSE_QUANTITY);
                    string unitName = dataReader.GetFieldFromReader<string>(Sql.WAREHOUSE_UNITNAME);
                    string barcode = dataReader.GetFieldFromReader<string>(Sql.WAREHOUSE_BARCODE);

                    result.Add(new Warehouse(id, code, name, quantity, unitName, barcode));
                }

                return result;
            }
            catch (Exception e)
            {
                Log.ErrorWithException("Ошибка получения товаров.", e);
                return new List<Warehouse>();
            }
            finally
            {
                dataReader?.Close();
                dbConnection?.Close();
            }
        }

        public List<WarehouseCategory> GetWarehouseCategories()
        {
            DbDataReader dataReader = null;

            try
            {
                dbConnection.Open();

                var command = GetDbCommandByQuery(Sql.GET_WAREHOUSE_CATEGORIES);
                dataReader = command.ExecuteReader();

                var result = new List<WarehouseCategory>();
                while (dataReader.Read())
                {
                    int id = dataReader.GetFieldFromReader<int>(Sql.WAREHOUSE_CATEGORY_ID);
                    string code = dataReader.GetFieldFromReader<string>(Sql.WAREHOUSE_CATEGORY_CODE);
                    string name = dataReader.GetFieldFromReader<string>(Sql.WAREHOUSE_CATEGORY_NAME);
                    bool isDefault = dataReader.GetFieldFromReader<bool>(Sql.WAREHOUSE_CATEGORY_IS_DEFAULT);
                    bool isWholesale = dataReader.GetFieldFromReader<bool>(Sql.WAREHOUSE_CATEGORY_IS_WHOLESALE);

                    result.Add(new WarehouseCategory(id, code, name, isDefault, isWholesale));
                }

                return result;
            }
            catch (Exception e)
            {
                Log.ErrorWithException("Ошибка получения типов стоимостей товаров.", e);
                return new List<WarehouseCategory>();
            }
            finally
            {
                dataReader?.Close();
                dbConnection?.Close();
            }
        }

        public List<WarehousePrice> GetWarehousePrices()
        {
            DbDataReader dataReader = null;

            try
            {
                dbConnection.Open();

                var command = GetDbCommandByQuery(Sql.GET_WAREHOUSE_PRICES);
                dataReader = command.ExecuteReader();

                var result = new List<WarehousePrice>();
                while (dataReader.Read())
                {
                    int id = dataReader.GetFieldFromReader<int>(Sql.WAREHOUSE_PRICE_ID);
                    int warehouseId = dataReader.GetFieldFromReader<int>(Sql.WAREHOUSE_PRICE_WAREHOUSE_ID);
                    int warehouseCategoryId = dataReader.GetFieldFromReader<int>(Sql.WAREHOUSE_PRICE_WAREHOUSE_CATEGORY_ID);
                    DateTime entryDate = dataReader.GetFieldFromReader<DateTime>(Sql.WAREHOUSE_PRICE_ENTRY_DATE);
                    decimal priceValue = dataReader.GetFieldFromReader<decimal>(Sql.WAREHOUSE_PRICE_PRICE_VALUE);

                    result.Add(new WarehousePrice(id, warehouseId, warehouseCategoryId, entryDate, priceValue));
                }

                return result;
            }
            catch (Exception e)
            {
                Log.ErrorWithException("Ошибка получения стоимостей товаров.", e);
                return new List<WarehousePrice>();
            }
            finally
            {
                dataReader?.Close();
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

        public PaymentKind AddPaymentKind(PaymentKind kind)
        {
            try
            {
                dbConnection.Open();

                var command = GetDbCommandByQuery(Sql.INSERT_PAYMENT_KIND);
                command.AddParameter(Sql.PARAM_PAYMENT_KIND_ID, kind.Id);
                command.AddParameter(Sql.PARAM_PAYMENT_KIND_NAME, kind.Name);
                command.AddParameter(Sql.PARAM_PAYMENT_TYPE_ID, kind.TypeId);

                int id = (int)command.ExecuteScalar();

                command.Transaction.Commit();
                command.Dispose();

                return new PaymentKind(id, kind.Name, kind.TypeId);
            }
            catch (Exception e)
            {
                Log.ErrorWithException($"Ошибка добавления вида платежа.", e);
                return null;
            }
            finally
            {
                dbConnection?.Close();
            }
        }

        public PaymentKind GetPaymentKind(int paymentKindId)
        {
            DbDataReader dataReader = null;

            try
            {
                dbConnection.Open();

                var command = GetDbCommandByQuery(Sql.GET_PAYMENT_KIND);
                command.AddParameter(Sql.PARAM_PAYMENT_TYPE_ID, paymentKindId);

                dataReader = command.ExecuteReader(CommandBehavior.SingleRow);

                PaymentKind paymentKind = null;
                if (dataReader.Read())
                {
                    string name = dataReader.GetFieldFromReader<string>(Sql.PAYMENT_KIND_NAME) ?? string.Empty;
                    int typeId = dataReader.GetFieldFromReader<int>(Sql.PAYMENT_KIND_TYPE_ID);

                    paymentKind = new PaymentKind(paymentKindId, name, typeId);
                }

                return paymentKind;
            }
            catch (Exception e)
            {
                Log.ErrorWithException($"Ошибка получения вида платежа с номером {paymentKindId}.", e);
                return null;
            }
            finally
            {
                dataReader?.Close();
                dbConnection?.Close();
            }
        }

        public List<PaymentKind> GetPaymentKinds()
        {
            DbDataReader dataReader = null;

            try
            {
                dbConnection.Open();

                var command = GetDbCommandByQuery(Sql.GET_PAYMENT_KINDS);
                dataReader = command.ExecuteReader();

                var result = new List<PaymentKind>();
                while (dataReader.Read())
                {
                    int id = dataReader.GetFieldFromReader<int>(Sql.PAYMENT_KIND_ID);
                    string name = dataReader.GetFieldFromReader<string>(Sql.PAYMENT_KIND_NAME) ?? string.Empty;
                    int typeId = dataReader.GetFieldFromReader<int>(Sql.PAYMENT_KIND_TYPE_ID);

                    result.Add(new PaymentKind(id, name, typeId));
                }

                return result;
            }
            catch (Exception e)
            {
                Log.ErrorWithException("Ошибка получения видов платежа.", e);
                return new List<PaymentKind>();
            }
            finally
            {
                dataReader?.Close();
                dbConnection?.Close();
            }
        }
    }
}