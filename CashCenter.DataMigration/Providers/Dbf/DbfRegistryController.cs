using CashCenter.Common;
using CashCenter.DataMigration.Providers.Dbf.Entities;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;

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
            
            private static readonly string GET_ENERGY_CUSTOMERS =
                $@"select {ENERGY_CUSTOMER_DEPARTMENT_CODE}, {ENERGY_CUSTOMER_ID}, {ENERGY_CUSTOMER_COUNTERS_END_DAY_VALUE}, {ENERGY_CUSTOMER_COUNTERS_END_NIGHT_VALUE}, {ENERGY_CUSTOMER_END_BALANCE}
                   from {{0}}";

            private static readonly string GET_WATER_CUSTOMERS =
                $@"select {WATER_CUSTOMER_NUMBER}, {WATER_CUSTOMER_NAME}, {WATER_CUSTOMER_ADDRESS}, {WATER_CUSTOMER_COUNTER_NUMBER1}, {WATER_CUSTOMER_COUNTER_NUMBER2}, {WATER_CUSTOMER_COUNTER_NUMBER3}
                   from {{0}}";

            private static readonly string GET_ARTICLES =
                $@"select {ARTICLES_DATA}, {ARTICLES_CODE}, {ARTICLES_NAME}, {ARTICLES_BARCODE}, {ARTICLES_PRICE}
                   form {{0}}";

            public static string GetEnergyCustomersQuery(string tableName)
            {
                return string.Format(GET_ENERGY_CUSTOMERS, tableName);
            }

            public static string GetWaterCustomersQuery(string tableName)
            {
                return string.Format(GET_WATER_CUSTOMERS, tableName);
            }

            public static string GetArticlesQuery(string tableName)
            {
                return string.Format(GET_ARTICLES, tableName);
            }
        }

        private OleDbConnection dbfConnection;
        private string dbfName;

        public DbfRegistryController(string fileName)
        {
            var fileInfo = new FileInfo(fileName);
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
                command.CommandText = Sql.GetEnergyCustomersQuery(dbfName);

                var dataReader = command.ExecuteReader();

                var energyCustomers = new List<DbfEnergyCustomer>();
                while (dataReader.Read())
                {
                    int number = (int)dataReader.GetFieldFromReader<double>(Sql.ENERGY_CUSTOMER_ID);
                    string departamentCode = dataReader.GetFieldFromReader<string>(Sql.ENERGY_CUSTOMER_DEPARTMENT_CODE);
                    int dayValue = (int)dataReader.GetFieldFromReader<double>(Sql.ENERGY_CUSTOMER_COUNTERS_END_DAY_VALUE);
                    int nightValue = (int)dataReader.GetFieldFromReader<double>(Sql.ENERGY_CUSTOMER_COUNTERS_END_NIGHT_VALUE);
                    decimal balance = (decimal)dataReader.GetFieldFromReader<double>(Sql.ENERGY_CUSTOMER_END_BALANCE);

                    energyCustomers.Add(new DbfEnergyCustomer(number, departamentCode, dayValue, nightValue, balance));
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
                command.CommandText = Sql.GetWaterCustomersQuery(dbfName);

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
                command.CommandText = Sql.GetArticlesQuery(dbfName);

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
    }
}