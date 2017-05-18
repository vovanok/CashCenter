﻿using CashCenter.Common;
using CashCenter.DataMigration.Providers.Dbf.Entities;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Linq;

namespace CashCenter.DataMigration.Providers.Dbf
{
    public class DbfRegistryController
    {
        private static class Sql
        {
            public const string ENERGY_CUSTOMER_DEPARTMENT_CODE = "KO";
            public const string ENERGY_CUSTOMER_ID = "LS";
            public const string ENERGY_CUSTOMER_COUNTERS_END_DAY_VALUE = "CDAY";
            public const string ENERGY_CUSTOMER_COUNTERS_END_NIGHT_VALUE = "CNIGHT";
            public const string ENERGY_CUSTOMER_END_BALANCE = "SUMMA";
            public const string ENERGY_CUSTOMER_NAME = "NAME";
            public const string ENERGY_CUSTOMER_ADDRESS = "ADDRESS";
            public const string ENERGY_CUSTOMER_ISCLOSED = "ISCLOSED";

            public const string WATER_CUSTOMER_NUMBER = "SCHET";
            public const string WATER_CUSTOMER_NAME = "FIO";
            public const string WATER_CUSTOMER_ADDRESS = "ADRESS";
            public const string WATER_CUSTOMER_COUNTER_NUMBER1 = "N_SHET1";
            public const string WATER_CUSTOMER_COUNTER_NUMBER2 = "N_SHET2";
            public const string WATER_CUSTOMER_COUNTER_NUMBER3 = "N_SHET3";

            public const string ARTICLES_DATA = "DATAN";
            public const string ARTICLES_CODE = "TOVARKOD";
            public const string ARTICLES_NAME = "TOVARNAME";
            public const string ARTICLES_BARCODE = "SHTRIHKOD";
            public const string ARTICLES_PRICE = "TOVARCENA";

            public const string WATER_CUSTOMER_PAYMENT_CREATION_DATE = "DATE";
            public const string WATER_CUSTOMER_PAYMENT_CUSTOMER_NUMBER = "SCHET";
            public const string WATER_CUSTOMER_PAYMENT_COST = "SUMZACH";
            public const string WATER_CUSTOMER_PAYMENT_PERIODCODE = "PER_OPL";
            public const string WATER_CUSTOMER_PAYMENT_PENALTY = "PENI";
            public const string WATER_CUSTOMER_PAYMENT_COUNTER_NUMBER1 = "N_SCHET1";
            public const string WATER_CUSTOMER_PAYMENT_COUNTER_COST1 = "SUM_SH1";
            public const string WATER_CUSTOMER_PAYMENT_COUNTER_VALUE1 = "ZN1";
            public const string WATER_CUSTOMER_PAYMENT_COUNTER_NUMBER2 = "N_SCHET2";
            public const string WATER_CUSTOMER_PAYMENT_COUNTER_COST2 = "SUM_SH2";
            public const string WATER_CUSTOMER_PAYMENT_COUNTER_VALUE2 = "ZN2";
            public const string WATER_CUSTOMER_PAYMENT_COUNTER_NUMBER3 = "N_SCHET3";
            public const string WATER_CUSTOMER_PAYMENT_COUNTER_COST3 = "SUM_SH3";
            public const string WATER_CUSTOMER_PAYMENT_COUNTER_VALUE3 = "ZN3";
            public const string WATER_CUSTOMER_PAYMENT_COUNTER_NUMBER4 = "N_SCHET4";
            public const string WATER_CUSTOMER_PAYMENT_COUNTER_COST4 = "SUM_SH4";
            public const string WATER_CUSTOMER_PAYMENT_COUNTER_VALUE4 = "ZN4";

            public const string TYPE_DATE = "Date";
            public const string TYPE_CHARACTER13 = "Character(13)";
            public const string TYPE_CHARACTER12 = "Character(12)";
            public const string TYPE_CHARACTER6 = "Character(6)";
            public const string TYPE_NUMERIC = "Numeric";

            private const string GET_ITEMS = "select * from {0}";
            private const string ADD_WATER_CUSTOMER_PAYMENTS_PRE = "insert into {0} values";

            private static readonly string CREATE_WATER_CUSTOMER_PAYMENTS =
                $@"create table {{0}} (
                    [{WATER_CUSTOMER_PAYMENT_CREATION_DATE}] {TYPE_DATE},
                    [{WATER_CUSTOMER_PAYMENT_CUSTOMER_NUMBER}] {TYPE_CHARACTER12},
                    [{WATER_CUSTOMER_PAYMENT_COST}] {TYPE_NUMERIC},
                    [{WATER_CUSTOMER_PAYMENT_PERIODCODE}] {TYPE_CHARACTER6},
                    [{WATER_CUSTOMER_PAYMENT_PENALTY}] {TYPE_NUMERIC},
                    [{WATER_CUSTOMER_PAYMENT_COUNTER_NUMBER1}] {TYPE_CHARACTER13},
                    [{WATER_CUSTOMER_PAYMENT_COUNTER_COST1}] {TYPE_NUMERIC},
                    [{WATER_CUSTOMER_PAYMENT_COUNTER_VALUE1}] {TYPE_NUMERIC},
                    [{WATER_CUSTOMER_PAYMENT_COUNTER_NUMBER2}] {TYPE_CHARACTER13},
                    [{WATER_CUSTOMER_PAYMENT_COUNTER_COST2}] {TYPE_NUMERIC},
                    [{WATER_CUSTOMER_PAYMENT_COUNTER_VALUE2}] {TYPE_NUMERIC},
                    [{WATER_CUSTOMER_PAYMENT_COUNTER_NUMBER3}] {TYPE_CHARACTER13},
                    [{WATER_CUSTOMER_PAYMENT_COUNTER_COST3}] {TYPE_NUMERIC},
                    [{WATER_CUSTOMER_PAYMENT_COUNTER_VALUE3}] {TYPE_NUMERIC},
                    [{WATER_CUSTOMER_PAYMENT_COUNTER_NUMBER4}] {TYPE_CHARACTER13},
                    [{WATER_CUSTOMER_PAYMENT_COUNTER_COST4}] {TYPE_NUMERIC},
                    [{WATER_CUSTOMER_PAYMENT_COUNTER_VALUE4}] {TYPE_NUMERIC})";

            public static string GetItemsQuery(string tableName)
            {
                return string.Format(GET_ITEMS, tableName);
            }

            public static string GetAddWaterCustomerPaymentsPreQuery(string tableName)
            {
                return string.Format(ADD_WATER_CUSTOMER_PAYMENTS_PRE, tableName);
            }

            public static string GetCreateWaterCustomerPaymentsQuery(string tableName)
            {
                return string.Format(CREATE_WATER_CUSTOMER_PAYMENTS, tableName);
            }
        }

        private OleDbConnection dbfConnection;
        private string dbfName;
        private string filename;

        public DbfRegistryController(string filename)
        {
            this.filename = filename;
            var fileInfo = new FileInfo(filename);
            var connectionString = string.Format(Config.DbfConnectionStringFormat, fileInfo.Directory.FullName);
            dbfConnection = new OleDbConnection(connectionString);
            dbfName = fileInfo.Name;
        }

        public List<DbfEnergyCustomer> GetEnergyCustomers()
        {
            try
            {
                dbfConnection.Open();

                var command = dbfConnection.CreateCommand();
                command.CommandText = Sql.GetItemsQuery(dbfName);

                var dataReader = command.ExecuteReader();

                var energyCustomers = new List<DbfEnergyCustomer>();
                while (dataReader.Read())
                {
                    int number = (int)dataReader.GetFieldFromReader<double>(Sql.ENERGY_CUSTOMER_ID);
                    string name = dataReader.GetFieldFromReader<string>(Sql.ENERGY_CUSTOMER_NAME) ?? string.Empty;
                    string address = dataReader.GetFieldFromReader<string>(Sql.ENERGY_CUSTOMER_ADDRESS) ?? string.Empty;
                    string departamentCode = dataReader.GetFieldFromReader<string>(Sql.ENERGY_CUSTOMER_DEPARTMENT_CODE) ?? string.Empty;
                    int dayValue = (int)dataReader.GetFieldFromReader<double>(Sql.ENERGY_CUSTOMER_COUNTERS_END_DAY_VALUE);
                    int nightValue = (int)dataReader.GetFieldFromReader<double>(Sql.ENERGY_CUSTOMER_COUNTERS_END_NIGHT_VALUE);
                    decimal balance = (decimal)dataReader.GetFieldFromReader<double>(Sql.ENERGY_CUSTOMER_END_BALANCE);
                    bool isClosed = dataReader.GetFieldFromReader<string>(Sql.ENERGY_CUSTOMER_ISCLOSED) == "1";

                    energyCustomers.Add(new DbfEnergyCustomer(number, name, address, departamentCode, dayValue, nightValue, balance, isClosed));
                }

                return energyCustomers;
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

        public List<DbfWaterCustomer> GetWaterCustomers()
        {
            try
            {
                dbfConnection.Open();

                var command = dbfConnection.CreateCommand();
                command.CommandText = Sql.GetItemsQuery(dbfName);

                var dataReader = command.ExecuteReader();

                var waterCustomers = new List<DbfWaterCustomer>();
                while (dataReader.Read())
                {
                    int number = int.Parse(dataReader.GetFieldFromReader<string>(Sql.WATER_CUSTOMER_NUMBER));
                    string name = dataReader.GetFieldFromReader<string>(Sql.WATER_CUSTOMER_NAME) ?? string.Empty;
                    string address = dataReader.GetFieldFromReader<string>(Sql.WATER_CUSTOMER_ADDRESS) ?? string.Empty;
                    string counterNumber1 = dataReader.GetFieldFromReader<string>(Sql.WATER_CUSTOMER_COUNTER_NUMBER1) ?? string.Empty;
                    string counterNumber2 = dataReader.GetFieldFromReader<string>(Sql.WATER_CUSTOMER_COUNTER_NUMBER2) ?? string.Empty;
                    string counterNumber3 = dataReader.GetFieldFromReader<string>(Sql.WATER_CUSTOMER_COUNTER_NUMBER3) ?? string.Empty;

                    waterCustomers.Add(new DbfWaterCustomer(number, name, address, counterNumber1, counterNumber2, counterNumber3));
                }

                return waterCustomers;
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

        public DbfArticlesSet GetArticles()
        {
            try
            {
                dbfConnection.Open();

                var command = dbfConnection.CreateCommand();
                command.CommandText = Sql.GetItemsQuery(dbfName);

                var dataReader = command.ExecuteReader();

                DateTime date = default(DateTime);
                if (dataReader.Read())
                    date = dataReader.GetFieldFromReader<DateTime>(Sql.ARTICLES_DATA);

                var articles = new List<DbfArticle>();
                while (dataReader.Read())
                {
                    var code = dataReader.GetFieldFromReader<string>(Sql.ARTICLES_CODE);
                    var name = dataReader.GetFieldFromReader<string>(Sql.ARTICLES_NAME);
                    var barcode = dataReader.GetFieldFromReader<string>(Sql.ARTICLES_BARCODE);
                    var price = dataReader.GetFieldFromReader<decimal>(Sql.ARTICLES_PRICE);

                    articles.Add(new DbfArticle(code, name, barcode, price));
                }

                return new DbfArticlesSet(date, articles);
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

        public void StoreWaterCustomerPayments(IEnumerable<DbfWaterCustomerPayment> payments)
        {
            CreateDbf();
            AddWaterCustomerPayments(payments);
        }

        private void CreateDbf()
        {
            if (File.Exists(filename))
                File.Delete(filename);

            try
            {
                dbfConnection.Open();

                var command = dbfConnection.CreateCommand();
                command.CommandText = Sql.GetCreateWaterCustomerPaymentsQuery(Path.GetFileNameWithoutExtension(dbfName));

                command.ExecuteNonQuery();
                command.Dispose();
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

        private void AddWaterCustomerPayments(IEnumerable<DbfWaterCustomerPayment> payments)
        {
            if (payments == null || payments.Count() == 0)
                return;

            try
            {
                dbfConnection.Open();

                foreach (var payment in payments)
                {
                    var command = dbfConnection.CreateCommand();
                
                    var values = new[]
                    {
                        GetStringForQuery(payment.CreationDate.ToString("dd.MM.yyyy")),
                        payment.CustomerNumber.ToString(),
                        GetMoneyString(payment.Cost),
                        GetStringForQuery(payment.PeriodCode),
                        GetMoneyString(payment.Penalty),

                        GetStringForQuery(payment.CounterNumber1),
                        GetMoneyString(0),
                        GetCounterValueString(payment.CounterValue1),

                        GetStringForQuery(payment.CounterNumber2),
                        GetMoneyString(0),
                        GetCounterValueString(payment.CounterValue2),

                        GetStringForQuery(payment.CounterNumber3),
                        GetMoneyString(0),
                        GetCounterValueString(payment.CounterValue3),

                        GetStringForQuery(payment.CounterNumber4),
                        GetMoneyString(0),
                        GetCounterValueString(payment.CounterValue4)
                    };

                    command.CommandText = $"{Sql.GetAddWaterCustomerPaymentsPreQuery(dbfName)} ({string.Join(", ", values)})";
                    command.ExecuteNonQuery();
                    command.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw new SystemException($"Ошибка записи в DBF {dbfName}", ex);
            }
            finally
            {
                dbfConnection?.Close();
            }
        }

        private string GetMoneyString(decimal moneyValue)
        {
            return moneyValue.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
        }

        private string GetCounterValueString(double counterValue)
        {
            return counterValue.ToString("0.000", System.Globalization.CultureInfo.InvariantCulture);
        }

        private string GetStringForQuery(string value)
        {
            return $"'{value}'";
        }
    }
}