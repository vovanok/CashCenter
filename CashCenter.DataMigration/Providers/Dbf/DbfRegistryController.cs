using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using CashCenter.Common;
using CashCenter.DataMigration.Providers.Dbf.Entities;

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

            public const string ARTICLEQUANTITY_ARTICLECODE = "TOVARKOD";
            public const string ARTICLEQUANTITY_QUANTITY = "TOVARKOL";
            public const string ARTICLEQUANTITY_MEASURE = "EDIZMNAME";

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

            public const string ARTICLE_SALE_DOCUMENT_DATE = "DATADOC";
            public const string ARTICLE_SALE_DOCUMENT_NUMBER = "NOMERDOC";
            public const string ARTICLE_SALE_WAREHOUSE_CODE = "SKLADKOD";
            public const string ARTICLE_SALE_WAREHOUSE_NAME = "SKLADNAME";
            public const string ARTICLE_SALE_ARTICLE_CODE = "TOVARKOD";
            public const string ARTICLE_SALE_ARTICLE_NAME = "TOVARNAME";
            public const string ARTICLE_SALE_ARTICLE_QUANTITY = "TOVARKOL";
            public const string ARTICLE_SALE_ARTICLE_PRICE = "TOVARCENA";
            public const string ARTICLE_SALE_ARTICLE_TOTAL_PRICE = "TOVARSUMMA";
            public const string ARTICLE_SALE_CHECK_NUMBER = "NOMERCHEKA";
            public const string ARTICLE_SALE_SERIAL_NUMBER = "SERNOMER";
            public const string ARTICLE_SALE_COMMENT = "KOMMENT";

            public const string GC_PAYMENT_FINANCIALPERIODCODE = "CODFIN";
            public const string GC_PAYMENT_CREATEDATE = "DATEPAY";
            public const string GC_PAYMENT_CREATETIME = "TIMEPAY";
            public const string GC_PAYMENT_FILIALCODE = "NUMCASH";
            public const string GC_PAYMENT_ORGANIZATIONCODE = "CODORG";
            public const string GC_PAYMENT_CUSTOMERNUMBER = "NUMAB";
            public const string GC_PAYMENT_COST = "SUMMA";

            public const string TYPE_DATE = "Date";
            public const string TYPE_CHARACTER3 = "Character(3)";
            public const string TYPE_CHARACTER5 = "Character(5)";
            public const string TYPE_CHARACTER6 = "Character(6)";
            public const string TYPE_CHARACTER8 = "Character(8)";
            public const string TYPE_CHARACTER10 = "Character(10)";
            public const string TYPE_CHARACTER11 = "Character(11)";
            public const string TYPE_CHARACTER12 = "Character(12)";
            public const string TYPE_CHARACTER13 = "Character(13)";
            public const string TYPE_CHARACTER25 = "Character(25)";
            public const string TYPE_CHARACTER50 = "Character(50)";
            public const string TYPE_CHARACTER100 = "Character(100)";
            public const string TYPE_NUMERIC = "Numeric";

            private const string GET_ITEMS = "select * from {0}";
            private const string INSERT_SOMETHING_PRE = "insert into {0} values";

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

            private static readonly string CREATE_ARTICLE_SALES =
                $@"create table {{0}} (
                    [{ARTICLE_SALE_DOCUMENT_DATE}] {TYPE_DATE},
                    [{ARTICLE_SALE_DOCUMENT_NUMBER}] {TYPE_CHARACTER10},
                    [{ARTICLE_SALE_WAREHOUSE_CODE}] {TYPE_CHARACTER3},
                    [{ARTICLE_SALE_WAREHOUSE_NAME}] {TYPE_CHARACTER25},
                    [{ARTICLE_SALE_ARTICLE_CODE}] {TYPE_CHARACTER10},
                    [{ARTICLE_SALE_ARTICLE_NAME}] {TYPE_CHARACTER50},
                    [{ARTICLE_SALE_ARTICLE_QUANTITY}] {TYPE_NUMERIC},
                    [{ARTICLE_SALE_ARTICLE_PRICE}] {TYPE_NUMERIC},
                    [{ARTICLE_SALE_ARTICLE_TOTAL_PRICE}] {TYPE_NUMERIC},
                    [{ARTICLE_SALE_CHECK_NUMBER}] {TYPE_NUMERIC},
                    [{ARTICLE_SALE_SERIAL_NUMBER}] {TYPE_CHARACTER50},
                    [{ARTICLE_SALE_COMMENT}] {TYPE_CHARACTER100})";

            private static readonly string CREATE_GARBAGE_COLLECTION_PAYMENTS =
                $@"create table {{0}} (
                    [{GC_PAYMENT_FINANCIALPERIODCODE}] {TYPE_NUMERIC},
                    [{GC_PAYMENT_CREATEDATE}] {TYPE_CHARACTER10},
                    [{GC_PAYMENT_CREATETIME}] {TYPE_CHARACTER8},
                    [{GC_PAYMENT_FILIALCODE}] {TYPE_CHARACTER5},
                    [{GC_PAYMENT_ORGANIZATIONCODE}] {TYPE_NUMERIC},
                    [{GC_PAYMENT_CUSTOMERNUMBER}] {TYPE_NUMERIC},
                    [{GC_PAYMENT_COST}] {TYPE_CHARACTER11})";

            public static string GetItemsQuery(string tableName)
            {
                return string.Format(GET_ITEMS, tableName);
            }

            public static string GetAddSomethingPreQuery(string tableName)
            {
                return string.Format(INSERT_SOMETHING_PRE, tableName);
            }

            public static string GetCreateWaterCustomerPaymentsQuery(string tableName)
            {
                return string.Format(CREATE_WATER_CUSTOMER_PAYMENTS, tableName);
            }

            public static string GetCreateArticleSalesQuery(string tableName)
            {
                return string.Format(CREATE_ARTICLE_SALES, tableName);
            }

            public static string GetCreatGarbageCollectionPaymentsQuery(string tableName)
            {
                return string.Format(CREATE_GARBAGE_COLLECTION_PAYMENTS, tableName);
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

        public List<DbfArticle> GetArticles()
        {
            try
            {
                dbfConnection.Open();

                var command = dbfConnection.CreateCommand();
                command.CommandText = Sql.GetItemsQuery(dbfName);

                var dataReader = command.ExecuteReader();

                DateTime entryDate = default(DateTime);
                if (dataReader.Read())
                    entryDate = dataReader.GetFieldFromReader<DateTime>(Sql.ARTICLES_DATA);

                var articles = new List<DbfArticle>();
                while (dataReader.Read())
                {
                    var code = dataReader.GetFieldFromReader<string>(Sql.ARTICLES_CODE);
                    var name = dataReader.GetFieldFromReader<string>(Sql.ARTICLES_NAME);
                    var barcode = dataReader.GetFieldFromReader<string>(Sql.ARTICLES_BARCODE);
                    var price = dataReader.GetFieldFromReader<double>(Sql.ARTICLES_PRICE);

                    articles.Add(new DbfArticle(code, name, barcode, (decimal)price, entryDate));
                }

                return articles;
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

        public List<DbfArticleQuantity> GetArticleQuantities()
        {
            try
            {
                dbfConnection.Open();

                var command = dbfConnection.CreateCommand();
                command.CommandText = Sql.GetItemsQuery(dbfName);

                var dataReader = command.ExecuteReader();

                var articlesQuantities = new List<DbfArticleQuantity>();
                while (dataReader.Read())
                {
                    var articleCode = dataReader.GetFieldFromReader<string>(Sql.ARTICLEQUANTITY_ARTICLECODE);
                    if (string.IsNullOrEmpty(articleCode))
                        continue;

                    double quantity;
                    try
                    {
                        quantity = dataReader.GetFieldFromReader<double>(Sql.ARTICLEQUANTITY_QUANTITY);
                    }
                    catch (InvalidCastException)
                    {
                        quantity = StringUtils.ForseDoubleParse(dataReader.GetFieldFromReader<string>(Sql.ARTICLEQUANTITY_QUANTITY));
                    }
                    
                    var measure = dataReader.GetFieldFromReader<string>(Sql.ARTICLEQUANTITY_MEASURE);

                    articlesQuantities.Add(new DbfArticleQuantity(articleCode, quantity, measure));
                }

                return articlesQuantities;
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
            CreateDbf(Sql.GetCreateWaterCustomerPaymentsQuery(Path.GetFileNameWithoutExtension(dbfName)));
            AddWaterCustomerPayments(payments);
        }

        public void StoreGarbageCollectionPayments(IEnumerable<DbfGarbageOrRepairPayment> payments)
        {
            CreateDbf(Sql.GetCreatGarbageCollectionPaymentsQuery(Path.GetFileNameWithoutExtension(dbfName)));
            AddGarbageCollectionPayments(payments);
        }

        private void CreateDbf(string query)
        {
            if (File.Exists(filename))
                File.Delete(filename);

            try
            {
                dbfConnection.Open();

                var command = dbfConnection.CreateCommand();
                command.CommandText = query;

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

                    command.CommandText = $"{Sql.GetAddSomethingPreQuery(dbfName)} ({string.Join(", ", values)})";
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

        public void StoreArticleSales(IEnumerable<DbfArticleSale> sales)
        {
            CreateDbf(Sql.GetCreateArticleSalesQuery(Path.GetFileNameWithoutExtension(dbfName)));
            AddArticleSales(sales);
        }

        private void AddArticleSales(IEnumerable<DbfArticleSale> sales)
        {
            if (sales == null || sales.Count() == 0)
                return;

            try
            {
                dbfConnection.Open();

                foreach (var sale in sales)
                {
                    var command = dbfConnection.CreateCommand();

                    var values = new[]
                    {
                        sale.DocumentDateTime != DateTime.MinValue ? GetStringForQuery(sale.DocumentDateTime.ToString("dd.MM.yyyy")) : "NULL",
                        GetStringForQuery(sale.DocumentNumber),
                        GetStringForQuery(sale.WarehouseCode),
                        GetStringForQuery(sale.WarehouseName),
                        GetStringForQuery(sale.ArticleCode),
                        GetStringForQuery(sale.ArticleName),
                        GetCounterValueString(sale.ArticleQuantity),
                        GetMoneyString(sale.ArticlePrice),
                        GetMoneyString(sale.ArticleTotalPrice),
                        sale.CheckNumber.ToString(),
                        GetStringForQuery(sale.SerialNumber),
                        GetStringForQuery(sale.Comment)
                    };

                    command.CommandText = $"{Sql.GetAddSomethingPreQuery(dbfName)} ({string.Join(", ", values)})";
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

        private void AddGarbageCollectionPayments(IEnumerable<DbfGarbageOrRepairPayment> payments)
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
                        payment.FinancialPeriodCode.ToString(),
                        GetStringForQuery(payment.CreateDate.ToString("dd.MM.yyyy")),
                        GetStringForQuery(payment.CreateDate.ToString("HH:mm")),
                        GetStringForQuery(payment.FilialCode.ToString()),
                        payment.OrganizationCode.ToString(),
                        payment.CustomerNumber.ToString(),
                        GetMoneyString(payment.Cost)
                    };

                    command.CommandText = $"{Sql.GetAddSomethingPreQuery(dbfName)} ({string.Join(", ", values)})";
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