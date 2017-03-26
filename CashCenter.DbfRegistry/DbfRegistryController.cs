using CashCenter.Common;
using CashCenter.DbfRegistry.Entities;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;

namespace CashCenter.DbfRegistry
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

            public const string ORGANIZATION_DEPARTAMENT_CODE = "CONSTANT";
            public const string ORGANIZATION_ID = "ID";
            public const string ORGANIZATION_CONTRACT_NUMBER = "CONTRACTNO";
            public const string ORGANIZATION_NAME = "NAME";
            public const string ORGANIZATION_INN = "IDENTNO1";
            public const string ORGANIZATION_KPP = "IDENTNO2";

            public const string ARTICLES_DATA = "DATAN";
            public const string ARTICLES_CODE = "TOVARKOD";
            public const string ARTICLES_NAME = "TOVARNAME";
            public const string ARTICLES_BARCODE = "SHTRIHKOD";
            public const string ARTICLES_PRICE = "TOVARCENA";
            
            private static readonly string GET_CUSTOMERS =
                $@"select {CUSTOMER_DEPARTMENT_CODE}, {CUSTOMER_ID}, {CUSTOMER_COUNTERS_END_DAY_VALUE}, {CUSTOMER_COUNTERS_END_NIGHT_VALUE}, {CUSTOMER_END_BALANCE}
                   from {{0}}";

            private static readonly string GET_ORGANIZATIONS =
                $@"select {ORGANIZATION_DEPARTAMENT_CODE}, {ORGANIZATION_ID}, {ORGANIZATION_CONTRACT_NUMBER}, {ORGANIZATION_NAME}, {ORGANIZATION_INN}, {ORGANIZATION_KPP}
                   from {{0}}";

            private static readonly string GET_ARTICLES =
                $@"select {ARTICLES_DATA}, {ARTICLES_CODE}, {ARTICLES_NAME}, {ARTICLES_BARCODE}, {ARTICLES_PRICE}
                   form {{0}}";

            public static string GetCustomersQuery(string tableName)
            {
                return string.Format(GET_CUSTOMERS, tableName);
            }

            public static string GetOrganizationsQuery(string tableName)
            {
                return string.Format(GET_ORGANIZATIONS, tableName);
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

        public List<Customer> GetCustomers()
        {
            try
            {
                dbfConnection.Open();

                var command = dbfConnection.CreateCommand();
                command.CommandText = Sql.GetCustomersQuery(dbfName);

                var dataReader = command.ExecuteReader();

                var customers = new List<Customer>();
                while (dataReader.Read())
                {
                    int id = (int)dataReader.GetFieldFromReader<double>(Sql.CUSTOMER_ID);
                    string departamentCode = dataReader.GetFieldFromReader<string>(Sql.CUSTOMER_DEPARTMENT_CODE);
                    int dayValue = (int)dataReader.GetFieldFromReader<double>(Sql.CUSTOMER_COUNTERS_END_DAY_VALUE);
                    int nightValue = (int)dataReader.GetFieldFromReader<double>(Sql.CUSTOMER_COUNTERS_END_NIGHT_VALUE);
                    decimal balance = (decimal)dataReader.GetFieldFromReader<double>(Sql.CUSTOMER_END_BALANCE);

                    customers.Add(new Customer(id, departamentCode, dayValue, nightValue, balance));
                }

                return customers;
            }
            catch (Exception e)
            {
                Log.ErrorWithException($"Ошибка получения физ.лиц из файла {dbfName}.", e);
                return new List<Customer>();
            }
            finally
            {
                dbfConnection?.Close();
            }
        }

        public List<Organization> GetOrganizations()
        {
            try
            {
                dbfConnection.Open();

                var command = dbfConnection.CreateCommand();
                command.CommandText = Sql.GetOrganizationsQuery(dbfName);

                var dataReader = command.ExecuteReader();

                List<Organization> organizations = new List<Organization>();
                while (dataReader.Read())
                {
                    int id = dataReader.GetFieldFromReader<int>(Sql.ORGANIZATION_ID);
                    string departamentCode = dataReader.GetFieldFromReader<string>(Sql.ORGANIZATION_DEPARTAMENT_CODE);
                    string contractNumber = dataReader.GetFieldFromReader<string>(Sql.ORGANIZATION_CONTRACT_NUMBER);
                    string name = dataReader.GetFieldFromReader<string>(Sql.ORGANIZATION_NAME);
                    string inn = dataReader.GetFieldFromReader<string>(Sql.ORGANIZATION_INN);
                    string kpp = dataReader.GetFieldFromReader<string>(Sql.ORGANIZATION_KPP);

                    organizations.Add(new Organization(id, departamentCode, contractNumber, name, inn, kpp));
                }

                return organizations;
            }
            catch (Exception e)
            {
                Log.ErrorWithException($"Ошибка получения организаций из файла {dbfName}.", e);
                return new List<Organization>();
            }
            finally
            {
                dbfConnection?.Close();
            }
        }

        public ArticlesSet GetArticles()
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

                var articles = new List<Article>();
                while (dataReader.Read())
                {
                    var code = dataReader.GetFieldFromReader<string>(Sql.ARTICLES_CODE);
                    var name = dataReader.GetFieldFromReader<string>(Sql.ARTICLES_NAME);
                    var barcode = dataReader.GetFieldFromReader<string>(Sql.ARTICLES_BARCODE);
                    var price = dataReader.GetFieldFromReader<decimal>(Sql.ARTICLES_PRICE);

                    articles.Add(new Article(code, name, barcode, price));
                }

                return new ArticlesSet(date, articles);
            }
            catch(Exception ex)
            {
                Log.ErrorWithException($"Ошибка загрузки товаров из DBF файла: {dbfName}", ex);
                return new ArticlesSet();
            }
        }
    }
}