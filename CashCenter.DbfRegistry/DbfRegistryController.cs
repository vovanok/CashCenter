using CashCenter.Common;
using CashCenter.DbfRegistry.Entities;
using System;
using System.Collections.Generic;
using System.Data;
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

            private static readonly string GET_CUSTOMER =
                $@"select {CUSTOMER_DEPARTMENT_CODE}, {CUSTOMER_ID}, {CUSTOMER_COUNTERS_END_DAY_VALUE}, {CUSTOMER_COUNTERS_END_NIGHT_VALUE}, {CUSTOMER_END_BALANCE}
                   from {{0}}
                   where {CUSTOMER_ID} = {{1}}";

            private static readonly string GET_ORGANIZATIONS =
                $@"select {ORGANIZATION_DEPARTAMENT_CODE}, {ORGANIZATION_ID}, {ORGANIZATION_CONTRACT_NUMBER}, {ORGANIZATION_NAME}, {ORGANIZATION_INN}, {ORGANIZATION_KPP}
                   from {{0}}
                   where {ORGANIZATION_CONTRACT_NUMBER} like {{1}} and {ORGANIZATION_NAME} like {{2}} and {ORGANIZATION_KPP} like {{3}}";

            public static string GetCustomerQuery(string tableName, int customerId)
            {
                return string.Format(GET_CUSTOMER, tableName, customerId);
            }

            public static string GetOrganizationsQuery(string tableName, string contractNumberPart, string namePart, string innPart)
            {
                return string.Format(GET_ORGANIZATIONS, tableName, contractNumberPart, namePart, innPart);
            }
        }

        private OleDbConnection dbfConnection;
        private string dbfName;

        public DbfRegistryController(string fileName)
        {
            var fileInfo = new FileInfo(fileName);
            dbfConnection = new OleDbConnection();
            dbfConnection.ConnectionString = string.Format(Config.DbfConnectionStringFormat, fileInfo.Directory.FullName);
            this.dbfName = fileInfo.Name;
        }

        public Customer GetCustomer(int customerId)
        {
            try
            {
                dbfConnection.Open();

                var command = dbfConnection.CreateCommand();
                command.CommandText = Sql.GetCustomerQuery(dbfName, customerId);

                var dataReader = command.ExecuteReader(CommandBehavior.SingleRow);

                Customer customer = null;
                if (dataReader.Read())
                {
                    int id = (int)dataReader.GetFieldFromReader<double>(Sql.CUSTOMER_ID);
                    string departamentCode = dataReader.GetFieldFromReader<string>(Sql.CUSTOMER_DEPARTMENT_CODE);
                    int dayValue = (int)dataReader.GetFieldFromReader<double>(Sql.CUSTOMER_COUNTERS_END_DAY_VALUE);
                    int nightValue = (int)dataReader.GetFieldFromReader<double>(Sql.CUSTOMER_COUNTERS_END_NIGHT_VALUE);
                    decimal balance = (decimal)dataReader.GetFieldFromReader<double>(Sql.CUSTOMER_END_BALANCE);

                    customer = new Customer(id, departamentCode, dayValue, nightValue, balance);
                }

                return customer;
            }
            catch (Exception e)
            {
                Log.ErrorWithException($"Ошибка получения лицевого счета из файла {dbfName} с ИД {customerId}.", e);
                return null;
            }
            finally
            {
                dbfConnection?.Close();
            }
        }

        public List<Organization> GetOrganizations(string contractNumberPart, string namePart, string innPart)
        {
            try
            {
                dbfConnection.Open();

                var command = dbfConnection.CreateCommand();
                command.CommandText = Sql.GetOrganizationsQuery(dbfName, contractNumberPart, namePart, innPart);

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
    }
}