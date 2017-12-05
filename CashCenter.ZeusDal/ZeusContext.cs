using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using FirebirdSql.Data.FirebirdClient;
using CashCenter.Dal;
using CashCenter.Dal.DataManipulationInterfaces;
using CashCenter.Common;
using System.Text;

namespace CashCenter.ZeusDal
{
    public class ZeusContext : IArticlesManipulatable
    {
        private const string CONNECTION_STRING_FORMAT = "DataSource={0};User=SYSDBA;Password=masterkey;Database={1}";
        private DbConnection connection;

        public IEnumerable<Article> Articles { get; private set; }
        public IEnumerable<ArticlePriceType> ArticlePriceTypes { get; private set; }
        public IEnumerable<ArticlePrice> ArticlePrices { get; private set; }

        public ZeusContext(string url, string path)
        {
            connection = new FbConnection(string.Format(CONNECTION_STRING_FORMAT, url, path));
            LoadDbToRamStorage();
        }

        public ArticleSale AddArticleSale(ArticleSale articleSale)
        {
            try
            {
                connection.Open();

                var artileSalePrice = ArticlePrices.FirstOrDefault(item => item.Id == articleSale.ArticlePriceId);
                if (artileSalePrice == null)
                    throw new Exception($"Не найдена цена для покупки с ИД = {articleSale.ArticlePriceId}");

                decimal total = (decimal)articleSale.Quantity * artileSalePrice.Value;

                //
                var command = GetDbCommandByQuery(
                    $"insert into WAREHOUSE_SALES (ENTRY_DATE, SALE_DIRECTION, TOTAL) " +
                    $"values ('{articleSale.CreateDate.ToString("yyyy-MM-dd")}', '1', {total}) " +
                    $"returning ID");

                int warehouseSalesId = (int)command.ExecuteScalar();
                command.Transaction.Commit();
                command.Dispose();

                //
                command = GetDbCommandByQuery($"select NUMBER from WAREHOUSE_SALES where ID = {warehouseSalesId}");
                var reader = command.ExecuteReader();
                reader.Read();
                string warehouseSalesNumber = reader.GetFieldFromReader<string>("NUMBER");
                command.Transaction.Commit();
                command.Dispose();

                //
                command = GetDbCommandByQuery($"update WAREHOUSE_SALES set NUMBER = '{GetArticleSaleNumber(warehouseSalesNumber)}' where ID = {warehouseSalesId};");
                command.ExecuteNonQuery();
                command.Transaction.Commit();
                command.Dispose();

                //
                command = GetDbCommandByQuery(
                    $"insert into WAREHOUSE_SALES_DETAIL (WAREHOUSE_SALES_ID, WAREHOUSE_ID, QUANTITY, PRICE_VALUE, TOTAL, SERIAL_NUMBER) " +
                    $"values ({warehouseSalesId}, {artileSalePrice.Article.Id}, {articleSale.Quantity}, {artileSalePrice.Value}, {total}, NULL);");

                command.ExecuteNonQuery();
                command.Transaction.Commit();
                command.Dispose();

                return articleSale;
            }
            catch (Exception ex)
            {
                throw new SystemException("Ошибка добавления покупки товара", ex);
            }
            finally
            {
                connection?.Close();
            }
        }

        private string GetArticleSaleNumber(string oldNumber)
        {
            if (oldNumber.Length < 7)
            {
                oldNumber = new string('0', 7 - oldNumber.Length) + oldNumber;
            }
            else if (oldNumber.Length > 7)
            {
                oldNumber = oldNumber.Substring(0, 7);
            }

            return Settings.ArticlesWarehouseCode + oldNumber;
        }

        private FbCommand GetDbCommandByQuery(string query)
        {
            if (connection.State != ConnectionState.Open)
                throw new SystemException("Ошибка получения комманды для текущего подключения к БД. Подключение не открыто");

            var command = connection.CreateCommand();
            command.Transaction = connection.BeginTransaction();
            command.CommandText = query;
            return command as FbCommand;
        }

        #region Load and cache data
        private IEnumerable<Article> GetArticlesFromReader(DbDataReader reader)
        {
            while (reader.Read())
            {
                yield return new Article
                {
                    Id = reader.GetFieldFromReader<int>("ID"),
                    Code = reader.GetFieldFromReader("CODE", string.Empty),
                    Name = reader.GetFieldFromReader("NAME", string.Empty),
                    Quantity = (double)reader.GetFieldFromReader<decimal>("QUANTITY"),
                    Measure = reader.GetFieldFromReader("UNIT_NAME", string.Empty),
                    Barcode = reader.GetFieldFromReader("BARCODE", string.Empty),
                    IsActive = true
                };
            }
        }

        private IEnumerable<ArticlePriceType> GetArticlePriceTypesFromReader(DbDataReader reader)
        {
            while (reader.Read())
            {
                yield return new ArticlePriceType
                {
                    Id = reader.GetFieldFromReader<int>("ID"),
                    Code = reader.GetFieldFromReader("CODE", string.Empty),
                    Name = reader.GetFieldFromReader("NAME", string.Empty),
                    IsDefault = reader.GetFieldFromReader("IS_DEFAULT", string.Empty) == "1",
                    IsWholesale = reader.GetFieldFromReader("IS_WHOLESALE", string.Empty) == "1"
                };
            }
        }

        private IEnumerable<ArticlePrice> GetArticlePricesFromReader(DbDataReader reader)
        {
            while (reader.Read())
            {
                var articleId = reader.GetFieldFromReader<int>("WAREHOUSE_ID");
                var articlePriceTypeId = reader.GetFieldFromReader<int>("WAREHOUSE_CATEGORY_ID");

                yield return new ArticlePrice
                {
                    Id = reader.GetFieldFromReader<int>("ID"),
                    ArticleId = articleId,
                    ArticlePriceTypeId = reader.GetFieldFromReader<int>("WAREHOUSE_CATEGORY_ID"),
                    Value = reader.GetFieldFromReader<decimal>("PRICE_VALUE"),
                    EntryDate = reader.GetFieldFromReader<DateTime>("ENTRY_DATE"),

                    Article = Articles.FirstOrDefault(articleItem => articleItem.Id == articleId),
                    ArticlePriceType = ArticlePriceTypes.FirstOrDefault(articlePriceTypeItem => articlePriceTypeItem.Id == articlePriceTypeId)
                };
            }
        }

        private void LoadDbToRamStorage()
        {
            DbDataReader dataReader = null;

            try
            {
                connection.Open();

                // Acticles price types
                var command = GetDbCommandByQuery("select ID, CODE, NAME, IS_DEFAULT, IS_WHOLESALE from WAREHOUSE_CATEGORY");
                ArticlePriceTypes = GetArticlePriceTypesFromReader(command.ExecuteReader()).ToList();
                command.Transaction.Commit();
                command.Dispose();

                // Articles
                command = GetDbCommandByQuery("select ID, CODE, NAME, QUANTITY, UNIT_NAME, BARCODE from WAREHOUSE");
                Articles = GetArticlesFromReader(command.ExecuteReader()).ToList();
                command.Transaction.Commit();
                command.Dispose();

                // Article prices
                command = GetDbCommandByQuery("select ID, WAREHOUSE_ID, WAREHOUSE_CATEGORY_ID, PRICE_VALUE, ENTRY_DATE from WAREHOUSE_PRICE");
                ArticlePrices = GetArticlePricesFromReader(command.ExecuteReader()).ToList();
                command.Transaction.Commit();
                command.Dispose();
            }
            catch (Exception ex)
            {
                throw new SystemException($"Ошибка получения данных о товарах из Зевса", ex);
            }
            finally
            {
                dataReader?.Close();
                connection?.Close();
            }
        }
        #endregion
    }
}