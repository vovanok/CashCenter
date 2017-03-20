using System;
using CashCenter.Dal;
using CashCenter.Common;
using CashCenter.DbfRegistry;
using System.Linq;

namespace CashCenter.DataMigration
{
    public class ArticlesDbfImporter : BaseDbfImporter
    {
        public ArticlePriceType PriceType { get; set; }
        public ImportResult ImportResult { get; private set; }

        public override void Import(string dbfFilename)
        {
            if (string.IsNullOrEmpty(dbfFilename))
            {
                Log.Error($"{IMPORT_ERROR_PREFIX} Путь к файлу не задан.");
                return;
            }

            if (PriceType == null)
            {
                Log.Error($"{IMPORT_ERROR_PREFIX} Не задан тип стоимости товара.");
                return;
            }

            try
            {
                var dbfRegistry = new DbfRegistryController(dbfFilename);
                var articlesSet = dbfRegistry.GetArticles();

                var existingArticles = DalController.Instance.Articles;

                ImportResult = new ImportResult();
                foreach (var importedArticle in articlesSet.Articles)
                {
                    var existingArticle = existingArticles.FirstOrDefault(article => article.Code == importedArticle.Code);
                    if (existingArticle != null)
                    {
                        UpdateDbArticleByImportedArticle(existingArticle, importedArticle, PriceType, articlesSet.EntryDate);
                        ImportResult.UpdatedArticles.Add(existingArticle);
                    }
                    else
                    {
                        var addedArticle = AddNewArticleByImportedArticle(importedArticle, PriceType, articlesSet.EntryDate);
                        ImportResult.AddedArticles.Add(addedArticle);
                    }
                }

                var articlesForDelete = existingArticles.Where(article =>
                    !ImportResult.UpdatedArticles.Contains(article) && !ImportResult.AddedArticles.Contains(article)).ToList();

                DalController.Instance.DeleteArticleRange(articlesForDelete);

                ImportResult.DeletedArticles.AddRange(articlesForDelete);
            }
            catch (Exception ex)
            {
                Log.ErrorWithException(IMPORT_ERROR_PREFIX, ex);
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
