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
            public const string CUSTOMER_DEPARTMENT_CODE = "KO";
            public const string CUSTOMER_ID = "LS";
            public const string CUSTOMER_COUNTERS_END_DAY_VALUE = "CDAY";
            public const string CUSTOMER_COUNTERS_END_NIGHT_VALUE = "CNIGHT";
            public const string CUSTOMER_END_BALANCE = "SUMMA";

            public const string ARTICLES_DATA = "DATAN";
            public const string ARTICLES_CODE = "TOVARKOD";
            public const string ARTICLES_NAME = "TOVARNAME";
            public const string ARTICLES_BARCODE = "SHTRIHKOD";
            public const string ARTICLES_PRICE = "TOVARCENA";
            
            private static readonly string GET_CUSTOMERS =
                $@"select {CUSTOMER_DEPARTMENT_CODE}, {CUSTOMER_ID}, {CUSTOMER_COUNTERS_END_DAY_VALUE}, {CUSTOMER_COUNTERS_END_NIGHT_VALUE}, {CUSTOMER_END_BALANCE}
                   from {{0}}";

            private static readonly string GET_ARTICLES =
                $@"select {ARTICLES_DATA}, {ARTICLES_CODE}, {ARTICLES_NAME}, {ARTICLES_BARCODE}, {ARTICLES_PRICE}
                   form {{0}}";

            public static string GetCustomersQuery(string tableName)
            {
                return string.Format(GET_CUSTOMERS, tableName);
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

        public List<DbfCustomer> GetCustomers()
        {
            try
            {
                dbfConnection.Open();

                var command = dbfConnection.CreateCommand();
                command.CommandText = Sql.GetCustomersQuery(dbfName);

                var dataReader = command.ExecuteReader();

                var customers = new List<DbfCustomer>();
                while (dataReader.Read())
                {
                    int number = (int)dataReader.GetFieldFromReader<double>(Sql.CUSTOMER_ID);
                    string departamentCode = dataReader.GetFieldFromReader<string>(Sql.CUSTOMER_DEPARTMENT_CODE);
                    int dayValue = (int)dataReader.GetFieldFromReader<double>(Sql.CUSTOMER_COUNTERS_END_DAY_VALUE);
                    int nightValue = (int)dataReader.GetFieldFromReader<double>(Sql.CUSTOMER_COUNTERS_END_NIGHT_VALUE);
                    decimal balance = (decimal)dataReader.GetFieldFromReader<double>(Sql.CUSTOMER_END_BALANCE);

                    customers.Add(new DbfCustomer(number, departamentCode, dayValue, nightValue, balance));
                }

                return customers;
            }
            catch (Exception e)
            {
                Log.ErrorWithException($"Ошибка получения физ.лиц из файла {dbfName}.", e);
                return new List<DbfCustomer>();
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
            catch(Exception ex)
            {
                Log.ErrorWithException($"Ошибка загрузки товаров из DBF файла: {dbfName}", ex);
                return new DbfArticlesSet();
            }
        }
    }
}