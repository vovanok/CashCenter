using System;
using CashCenter.DbfRegistry;
using CashCenter.Common;
using CashCenter.Dal;
using System.Linq;
using CashCenter.Common.DbQualification;
using CashCenter.ZeusDb;

namespace CashCenter.Articles.DataMigration
{
    public class ArticlesImporter
    {
        private const string IMPORT_ERROR_PREFIX = "Ошибка импортирования данных.";

        public ImportResult ImportFromFile(string dbfFilePath, ArticlePriceType priceType)
        {
            var importResult = new ImportResult();

            if (string.IsNullOrEmpty(dbfFilePath))
            {
                Log.Error($"{IMPORT_ERROR_PREFIX} Путь к файлу не задан.");
                return importResult;
            }

            try
            {
                var dbfRegistry = new DbfRegistryController(dbfFilePath);
                var articlesSet = dbfRegistry.GetArticles();

                var existingArticles = DalController.Instance.GetArticles();

                foreach (var importedArticle in articlesSet.Articles)
                {
                    var existingArticle = existingArticles.FirstOrDefault(article => article.Code == importedArticle.Code);
                    if (existingArticle != null)
                    {
                        UpdateDbArticleByImportedArticle(existingArticle, importedArticle, priceType, articlesSet.EntryDate);
                        importResult.UpdatedArticles.Add(existingArticle);
                    }
                    else
                    {
                        var addedArticle = AddNewArticleByImportedArticle(importedArticle, priceType, articlesSet.EntryDate);
                        importResult.AddedArticles.Add(addedArticle);
                    }
                }

                var articlesForDelete = existingArticles.Where(article =>
                    !importResult.UpdatedArticles.Contains(article) && !importResult.AddedArticles.Contains(article)).ToList();

                DalController.Instance.DeleteArticleRange(articlesForDelete);

                importResult.DeletedArticles.AddRange(articlesForDelete);

                return importResult;
            }
            catch (Exception ex)
            {
                Log.ErrorWithException(IMPORT_ERROR_PREFIX, ex);
                return importResult;
            }
        }

        public void ImportFromDb(DepartmentDef department)
        {
            if (department == null)
            {
                Log.Error("Для импорта из БД не указано отделение.");
                return;
            }

            try
            {
                DalController.Instance.ClearAllArticlesData();

                var db = new ZeusDbController(department);

                // Articles
                var importingWarehouses = db.GetWarehouses();
                var articlesForAdd = importingWarehouses.Select(warehouse => new Article
                {
                    Code = warehouse.Code,
                    Name = warehouse.Name,
                    Quantity = warehouse.Quantity,
                    Measure = warehouse.UnitName,
                    Barcode = warehouse.Barcode,
                });
                DalController.Instance.AddArticleRange(articlesForAdd);

                // Article price types
                var importingWarehouseCategories = db.GetWarehouseCategories();
                var articlePriceTypesForAdd = importingWarehouseCategories.Select(warehousePriceType => new ArticlePriceType
                {
                    Code = warehousePriceType.Code,
                    Name = warehousePriceType.Name,
                    IsDefault = warehousePriceType.IsDefault,
                    IsWholesale = warehousePriceType.IsDefault
                });
                DalController.Instance.AddArticlePriceTypeRange(articlePriceTypesForAdd);

                var articles = DalController.Instance.GetArticles().ToList();
                var articlePriceTypes = DalController.Instance.GetArticlePriceTypes().ToList();

                // Article prices
                var importingWarehousePrices = db.GetWarehousePrices();
                var articlePricesForAdd = importingWarehousePrices.Select(warehousePrice =>
                    {
                        var relWarehouse = importingWarehouses.FirstOrDefault(warehouse => warehouse.Id == warehousePrice.WarehouseId);
                        if (relWarehouse == null)
                            return null;

                        var relArticle = articles.FirstOrDefault(article => article.Code == relWarehouse.Code);
                        if (relArticle == null)
                            return null;

                        var relWarehouseCategory = importingWarehouseCategories
                            .FirstOrDefault(warehouseCategory => warehouseCategory.Id == warehousePrice.WarehouseCategoryId);
                        if (relWarehouseCategory == null)
                            return null;

                        var relArticlePriceType = articlePriceTypes
                            .FirstOrDefault(articlePriceType => articlePriceType.Code == relWarehouseCategory.Code);
                        if (relArticlePriceType == null)
                            return null;

                        return new ArticlePrice
                            {
                                ArticleId = relArticle.Id,
                                ArticlePriceTypeId = relArticlePriceType.Id,
                                EntryDate = warehousePrice.EntryDate,
                                Value = warehousePrice.PriceValue
                            };
                    });
                var newArticlePrices = DalController.Instance.AddArticlePriceRange(articlePricesForAdd);
            }
            catch(Exception ex)
            {
                Log.ErrorWithException("Ошибка импортирования данных из удаленной БД", ex);
                Log.Error(ex.InnerException.Message);
            }
        }

        private Article AddNewArticleByImportedArticle(DbfRegistry.Entities.Article importedArticle, ArticlePriceType priceType, DateTime entryDate)
        {
            if (importedArticle == null || priceType == null)
                return null;

            var newDbArticle = new Article
            {
                Code = importedArticle.Code ?? string.Empty,
                Name = importedArticle.Name ?? string.Empty,
                Quantity = 1f, // TODO
                Measure = string.Empty, // TODO
                Barcode = importedArticle.Barcode
            };

            var newArticlePrice = new ArticlePrice
            {
                Article = newDbArticle,
                ArticlePriceType = priceType,
                EntryDate = entryDate,
                Value = importedArticle.Price
            };

            newDbArticle.ArticlePrices.Add(newArticlePrice);

            DalController.Instance.AddArticle(newDbArticle);

            return newDbArticle;
        }

        private void UpdateDbArticleByImportedArticle(Article dbArticle, DbfRegistry.Entities.Article importedArticle,
            ArticlePriceType priceType, DateTime entryDate)
        {
            if (dbArticle == null || importedArticle == null || priceType == null)
                return;

            dbArticle.Name = importedArticle.Name ?? string.Empty;
            dbArticle.Barcode = importedArticle.Barcode ?? string.Empty;

            var articlePrice = dbArticle.ArticlePrices
                .FirstOrDefault(price => price.ArticlePriceType == priceType && price.EntryDate == entryDate);

            if (articlePrice != null)
            {
                articlePrice.Value = importedArticle.Price;
                return;
            }

            var newArticlePrice = new ArticlePrice
            {
                Article = dbArticle,
                ArticlePriceType = priceType,
                EntryDate = entryDate,
                Value = importedArticle.Price
            };

            dbArticle.ArticlePrices.Add(newArticlePrice);
        }
    }
}
