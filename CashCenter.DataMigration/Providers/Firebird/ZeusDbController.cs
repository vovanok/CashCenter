using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using FirebirdSql.Data.FirebirdClient;
using CashCenter.DataMigration.Providers.Firebird.Entities;
using CashCenter.Common;

namespace CashCenter.DataMigration.Providers.Firebird
{
    public partial class ZeusDbController
    {
        private DbConnection dbConnection;

        public string DepartmentCode { get; private set; }

        public ZeusDbController(string departmentCode, string dbUrl, string dbPath)
        {
            DepartmentCode = departmentCode;
            this.dbConnection = new FbConnection(string.Format(Config.DbConnectionStringFormat, dbUrl, dbPath));
        }

        public List<ZeusPaymentReason> GetPaymentReasons()
        {
            DbDataReader dataReader = null;

            try
            {
                dbConnection.Open();

                var command = GetDbCommandByQuery(Sql.GET_REASONS);
                dataReader = command.ExecuteReader();

                var result = new List<ZeusPaymentReason>();
                while (dataReader.Read())
                {
                    int id = dataReader.GetFieldFromReader<int>(Sql.REASON_ID);
                    string name = dataReader.GetFieldFromReader<string>(Sql.REASON_NAME);
                    bool isCanPay = dataReader.GetFieldFromReader<string>(Sql.REASON_CANPAY) == "1";

                    result.Add(new ZeusPaymentReason(id, name, isCanPay));
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new SystemException($"Ошибка получения оснований для оплаты", ex);
            }
            finally
            {
                dataReader?.Close();
                dbConnection?.Close();
            }
        }

        public List<ZeusCustomer> GetCustomers()
        {
            DbDataReader dataReader = null;

            try
            {
                dbConnection.Open();

                var command = GetDbCommandByQuery(Sql.GET_CUSTOMERS);
                dataReader = command.ExecuteReader();

                var result = new List<ZeusCustomer>();
                while (dataReader.Read())
                {
                    int number = dataReader.GetFieldFromReader<int>(Sql.CUSTOMER_ID);
                    string name = dataReader.GetFieldFromReader<string>(Sql.CUSTOMER_NAME) ?? string.Empty;
                    string flat = dataReader.GetFieldFromReader<string>(Sql.CUSTOMER_FLAT) ?? string.Empty;
                    string buildingNumber = dataReader.GetFieldFromReader<string>(Sql.CUSTOMER_BUILDING_NUMBER) ?? string.Empty;
                    string streetName = dataReader.GetFieldFromReader<string>(Sql.CUSTOMER_STREET_NAME) ?? string.Empty;
                    string localityName = dataReader.GetFieldFromReader<string>(Sql.CUSTOMER_LOCALITY_NAME) ?? string.Empty;

                    result.Add(new ZeusCustomer(number, DepartmentCode ?? string.Empty, name, flat, buildingNumber, streetName, localityName));
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new SystemException("Ошибка получения физических лиц", ex);
            }
            finally
            {
                dataReader?.Close();
                dbConnection?.Close();
            }
        }

        public ZeusCustomerCounters GetCustomerCounterValues(
            int customerNumber, DateTime beginDate, DateTime endDate)
        {
            DbDataReader dataReader = null;

            try
            {
                dbConnection.Open();

                var command = GetDbCommandByQuery(Sql.GET_CUSTOMER_COUNTER_VALUES);
                command.AddParameter(Sql.PARAM_CUSTOMER_ID, customerNumber);
                command.AddParameter(Sql.PARAM_START_DATE, beginDate.ToShortDateString());
                command.AddParameter(Sql.PARAM_END_DATE, endDate.ToShortDateString());

                dataReader = command.ExecuteReader(CommandBehavior.SingleRow);

                ZeusCustomerCounters result = null;
                if (dataReader.Read())
                {
                    string counterName = dataReader.GetFieldFromReader<string>(Sql.COUNTER_NAME);
                    int endDayValue = dataReader.GetFieldFromReader<int>(Sql.CUSTOMER_COUNTERS_END_DAY_VALUE);
                    int endNightValue = dataReader.GetFieldFromReader<int>(Sql.CUSTOMER_COUNTERS_END_NIGHT_VALUE);
                    bool isTwoTariff = dataReader.GetFieldFromReader<string>(Sql.CUSTOMER_COUNTERS_IS_TWO_TARIFF) == "1";

                    if (counterName == null)
                        return null;

                    result = new ZeusCustomerCounters(customerNumber, endDayValue, endNightValue, isTwoTariff);
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new SystemException($"Ошибка получения лицевого счета с номером {customerNumber}", ex);
            }
            finally
            {
                dataReader?.Close();
                dbConnection?.Close();
            }
        }

        public ZeusDebt GetDebt(int customerNumber, int dayEncoding)
        {
            DbDataReader dataReader = null;

            try
            {
                dbConnection.Open();

                var command = GetDbCommandByQuery(Sql.SELECT_DEBT);
                command.AddParameter(Sql.PARAM_CUSTOMER_ID, customerNumber);
                command.AddParameter(Sql.PARAM_DAY_ENCODING, dayEncoding);

                dataReader = command.ExecuteReader(CommandBehavior.SingleRow);

                ZeusDebt debt = null;
                if (dataReader.Read())
                {
                    var balance = dataReader.GetFieldFromReader<decimal>(Sql.END_BALANCE);
                    var penalty = dataReader.GetFieldFromReader<decimal>(Sql.PENALTY_BALANCE);

                    debt = new ZeusDebt(customerNumber, balance, penalty);
                }

                return debt;
            }
            catch (Exception ex)
            {
                throw new SystemException($"Ошибка получения задолжности для плательщика {customerNumber}", ex);
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
            catch (Exception ex)
            {
                throw new SystemException($"Ошибка получения счетчика плательщика {customerId}", ex);
            }
            finally
            {
                dataReader?.Close();
                dbConnection?.Close();
            }
        }

        public ZeusPayJournal GetPayJournal(DateTime createDate, int paymentKindId)
        {
            DbDataReader dataReader = null;

            try
            {
                dbConnection.Open();

                var command = GetDbCommandByQuery(Sql.GET_PAYJOURNAL);
                command.AddParameter(Sql.PARAM_CREATE_DATE, createDate.ToShortDateString());
                command.AddParameter(Sql.PARAM_PAYMENT_KIND_ID, paymentKindId);

                dataReader = command.ExecuteReader(CommandBehavior.SingleRow);

                ZeusPayJournal resultPayJournal = null;
                if (dataReader.Read())
                {
                    var id = dataReader.GetFieldFromReader<int>(Sql.PAY_JOURNAL_ID);
                    var name = dataReader.GetFieldFromReader<string>(Sql.PAY_JOURNAL_NAME);

                    resultPayJournal = new ZeusPayJournal(id, name, createDate, paymentKindId);
                }

                return resultPayJournal;
            }
            catch (Exception ex)
            {
                throw new SystemException("Ошибка получения журнала платежа", ex);
            }
            finally
            {
                dataReader?.Close();
                dbConnection?.Close();
            }
        }

        public void AddRequireToPayJournal(ZeusPayJournal payJournal, decimal additionCost)
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
            catch (Exception ex)
            {
                throw new SystemException("Ошибка обновления журнала платежа", ex);
            }
            finally
            {
                dbConnection?.Close();
            }
        }

        public ZeusPayJournal AddPayJournal(ZeusPayJournal payJournal, decimal cost)
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

                return new ZeusPayJournal(id, payJournal.Name, payJournal.CreateDate, payJournal.PaymentKindId);
            }
            catch (Exception ex)
            {
                throw new SystemException("Ошибка добавления журнала платежа", ex);
            }
            finally
            {
                dbConnection?.Close();
            }
        }

        public ZeusPay AddPay(ZeusPay pay)
        {
            try
            {
                dbConnection.Open();

                var command = GetDbCommandByQuery(Sql.INSERT_PAY);
                command.AddParameter(Sql.PARAM_CUSTOMER_ID, pay.CustomerNumber);
                command.AddParameter(Sql.PARAM_PAY_JOURNAL_ID, pay.JournalId);
                command.AddParameter(Sql.PARAM_REASON_ID, pay.ReasonId);
                command.AddParameter(Sql.PARAM_METERS_ID, pay.MetersId);
                command.AddParameter(Sql.PARAM_PAYMENT_COST, pay.Cost);
                command.AddParameter(Sql.PARAM_PENALTY_TOTAL, pay.PenaltyTotal);
                command.AddParameter(Sql.PARAM_DESCRIPTION, pay.Description);

                int id = (int)command.ExecuteScalar();

                command.Transaction.Commit();
                command.Dispose();

                return new ZeusPay(id, pay.CustomerNumber, pay.ReasonId, pay.MetersId, pay.JournalId,
                    pay.Cost, pay.PenaltyTotal, pay.Description);
            }
            catch (Exception ex)
            {
                throw new SystemException("Ошибка добавления платежа", ex);
            }
            finally
            {
                dbConnection?.Close();
            }
        }

        public ZeusCounterValues AddCounterValues(ZeusCounterValues counterValues, DateTime createDate)
        {
            try
            {
                dbConnection.Open();

                var command = GetDbCommandByQuery(Sql.INSERT_COUNTERVALUES);
                command.AddParameter(Sql.PARAM_CUSTOMER_ID, counterValues.CustomerNumber);
                command.AddParameter(Sql.PARAM_CUSTOMER_COUNTER_ID, counterValues.CustomerCounterId);
                command.AddParameter(Sql.PARAM_CREATE_DATE, createDate.ToShortDateString());
                command.AddParameter(Sql.PARAM_VALUE1, counterValues.Value1);
                command.AddParameter(Sql.PARAM_VALUE2, counterValues.Value2);

                int id = (int)command.ExecuteScalar();

                command.Transaction.Commit();
                command.Dispose();

                return new ZeusCounterValues(id, counterValues.CustomerNumber,
                    counterValues.CustomerCounterId, counterValues.Value1,
                    counterValues.Value2);
            }
            catch (Exception ex)
            {
                throw new SystemException("Ошибка добавления показаний счетчиков", ex);
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
            catch (Exception ex)
            {
                throw new SystemException("Ошибка обновления контрольных значений", ex);
            }
            finally
            {
                dbConnection?.Close();
            }
        }

        public ZeusMeter AddMeters(ZeusMeter meter)
        {
            try
            {
                dbConnection.Open();

                var command = GetDbCommandByQuery(Sql.INSERT_METERS);
                command.AddParameter(Sql.PARAM_CUSTOMER_ID, meter.CustomerNumber);
                command.AddParameter(Sql.PARAM_CUSTOMER_COUNTER_ID, meter.CustomerCounterId);
                command.AddParameter(Sql.PARAM_VALUE1, meter.Value1);
                command.AddParameter(Sql.PARAM_VALUE2, meter.Value2);
                command.AddParameter(Sql.PARAM_COUNTER_VALUES_ID, meter.CounterValuesId);

                int id = (int)command.ExecuteScalar();

                command.Transaction.Commit();
                command.Dispose();

                return new ZeusMeter(id, meter.CustomerNumber, meter.CustomerCounterId,
                    meter.Value1, meter.Value2, meter.CounterValuesId);
            }
            catch (Exception ex)
            {
                throw new SystemException("Ошибка добавления показаний", ex);
            }
            finally
            {
                dbConnection?.Close();
            }
        }

        public List<ZeusWarehouse> GetWarehouses()
        {
            DbDataReader dataReader = null;

            try
            {
                dbConnection.Open();

                var command = GetDbCommandByQuery(Sql.GET_WAREHOUSES);
                dataReader = command.ExecuteReader();

                var result = new List<ZeusWarehouse>();
                while (dataReader.Read())
                {
                    int id = dataReader.GetFieldFromReader<int>(Sql.WAREHOUSE_ID);
                    string code = dataReader.GetFieldFromReader<string>(Sql.WAREHOUSE_CODE);
                    string name = dataReader.GetFieldFromReader<string>(Sql.WAREHOUSE_NAME);
                    float quantity = dataReader.GetFieldFromReader<float>(Sql.WAREHOUSE_QUANTITY);
                    string unitName = dataReader.GetFieldFromReader<string>(Sql.WAREHOUSE_UNITNAME);
                    string barcode = dataReader.GetFieldFromReader<string>(Sql.WAREHOUSE_BARCODE);

                    result.Add(new ZeusWarehouse(id, code, name, quantity, unitName, barcode));
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new SystemException("Ошибка получения товаров", ex);
            }
            finally
            {
                dataReader?.Close();
                dbConnection?.Close();
            }
        }

        public List<ZeusWarehouseCategory> GetWarehouseCategories()
        {
            DbDataReader dataReader = null;

            try
            {
                dbConnection.Open();

                var command = GetDbCommandByQuery(Sql.GET_WAREHOUSE_CATEGORIES);
                dataReader = command.ExecuteReader();

                var result = new List<ZeusWarehouseCategory>();
                while (dataReader.Read())
                {
                    int id = dataReader.GetFieldFromReader<int>(Sql.WAREHOUSE_CATEGORY_ID);
                    string code = dataReader.GetFieldFromReader<string>(Sql.WAREHOUSE_CATEGORY_CODE);
                    string name = dataReader.GetFieldFromReader<string>(Sql.WAREHOUSE_CATEGORY_NAME);
                    bool isDefault = dataReader.GetFieldFromReader<bool>(Sql.WAREHOUSE_CATEGORY_IS_DEFAULT);
                    bool isWholesale = dataReader.GetFieldFromReader<bool>(Sql.WAREHOUSE_CATEGORY_IS_WHOLESALE);

                    result.Add(new ZeusWarehouseCategory(id, code, name, isDefault, isWholesale));
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new SystemException("Ошибка получения типов стоимостей товаров", ex);
            }
            finally
            {
                dataReader?.Close();
                dbConnection?.Close();
            }
        }

        public List<ZeusWarehousePrice> GetWarehousePrices()
        {
            DbDataReader dataReader = null;

            try
            {
                dbConnection.Open();

                var command = GetDbCommandByQuery(Sql.GET_WAREHOUSE_PRICES);
                dataReader = command.ExecuteReader();

                var result = new List<ZeusWarehousePrice>();
                while (dataReader.Read())
                {
                    int id = dataReader.GetFieldFromReader<int>(Sql.WAREHOUSE_PRICE_ID);
                    int warehouseId = dataReader.GetFieldFromReader<int>(Sql.WAREHOUSE_PRICE_WAREHOUSE_ID);
                    int warehouseCategoryId = dataReader.GetFieldFromReader<int>(Sql.WAREHOUSE_PRICE_WAREHOUSE_CATEGORY_ID);
                    DateTime entryDate = dataReader.GetFieldFromReader<DateTime>(Sql.WAREHOUSE_PRICE_ENTRY_DATE);
                    decimal priceValue = dataReader.GetFieldFromReader<decimal>(Sql.WAREHOUSE_PRICE_PRICE_VALUE);

                    result.Add(new ZeusWarehousePrice(id, warehouseId, warehouseCategoryId, entryDate, priceValue));
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new SystemException("Ошибка получения стоимостей товаров", ex);
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
                throw new SystemException("Ошибка получения комманды для текущего подключения к БД. Подключение не открыто.");

            var command = dbConnection.CreateCommand();
            command.Transaction = dbConnection.BeginTransaction();
            command.CommandText = query;
            return command;
        }

        public ZeusPaymentKind AddPaymentKind(ZeusPaymentKind kind)
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

                return new ZeusPaymentKind(id, kind.Name, kind.TypeId);
            }
            catch (Exception ex)
            {
                throw new SystemException("Ошибка добавления вида платежа", ex);
            }
            finally
            {
                dbConnection?.Close();
            }
        }

        public ZeusPaymentKind GetPaymentKind(int paymentKindId)
        {
            DbDataReader dataReader = null;

            try
            {
                dbConnection.Open();

                var command = GetDbCommandByQuery(Sql.GET_PAYMENT_KIND);
                command.AddParameter(Sql.PARAM_PAYMENT_TYPE_ID, paymentKindId);

                dataReader = command.ExecuteReader(CommandBehavior.SingleRow);

                ZeusPaymentKind paymentKind = null;
                if (dataReader.Read())
                {
                    string name = dataReader.GetFieldFromReader<string>(Sql.PAYMENT_KIND_NAME) ?? string.Empty;
                    int typeId = dataReader.GetFieldFromReader<int>(Sql.PAYMENT_KIND_TYPE_ID);

                    paymentKind = new ZeusPaymentKind(paymentKindId, name, typeId);
                }

                return paymentKind;
            }
            catch (Exception ex)
            {
                throw new SystemException($"Ошибка получения вида платежа с номером {paymentKindId}.", ex);
            }
            finally
            {
                dataReader?.Close();
                dbConnection?.Close();
            }
        }

        public List<ZeusPaymentKind> GetPaymentKinds()
        {
            DbDataReader dataReader = null;

            try
            {
                dbConnection.Open();

                var command = GetDbCommandByQuery(Sql.GET_PAYMENT_KINDS);
                dataReader = command.ExecuteReader();

                var result = new List<ZeusPaymentKind>();
                while (dataReader.Read())
                {
                    int id = dataReader.GetFieldFromReader<int>(Sql.PAYMENT_KIND_ID);
                    string name = dataReader.GetFieldFromReader<string>(Sql.PAYMENT_KIND_NAME) ?? string.Empty;
                    int typeId = dataReader.GetFieldFromReader<int>(Sql.PAYMENT_KIND_TYPE_ID);

                    result.Add(new ZeusPaymentKind(id, name, typeId));
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new SystemException("Ошибка получения видов платежа", ex);
            }
            finally
            {
                dataReader?.Close();
                dbConnection?.Close();
            }
        }
    }
}