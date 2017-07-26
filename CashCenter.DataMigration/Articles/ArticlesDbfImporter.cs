using System.Linq;
using System.Collections.Generic;
using CashCenter.Dal;
using CashCenter.DataMigration.Import;
using CashCenter.DataMigration.Providers.Dbf.Entities;

namespace CashCenter.DataMigration.Articles
{
    public class ArticlesDbfImporter : BaseDbfImporter<DbfArticle, Article>
    {
        public ArticlePriceType PriceType { get; set; }

        protected override void CreateNewItems(IEnumerable<Article> articles)
        {
            DalController.Instance.AddArticleRange(articles);
        }

        protected override int DeleteAllTargetItems()
        {
            var articlesForDelete = DalController.Instance.Articles.Where(article => article.IsActive).ToList();
            foreach (var article in articlesForDelete)
            {
                article.IsActive = false;
            }

            return articlesForDelete.Count;
        }

        protected override IEnumerable<DbfArticle> GetSourceItems()
        {
            if (dbfRegistry == null)
                return new List<DbfArticle>();

            return dbfRegistry.GetArticles();
        }

        protected override Article GetTargetItemBySource(DbfArticle dbfArticle)
        {
            var newArticle = new Article
            {
                Name = dbfArticle.Name ?? string.Empty,
                Quantity = 0f,
                Measure = "ед",
                Barcode = dbfArticle.Barcode ?? string.Empty,
                Code = dbfArticle.Code ?? string.Empty,
                IsActive = true
            };

            var newArticlePrice = new ArticlePrice
            {
                Article = newArticle,
                ArticlePriceType = PriceType,
                EntryDate = dbfArticle.EntryDate,
                Value = dbfArticle.Price
            };

            newArticle.ArticlePrices.Add(newArticlePrice);
            return newArticle;
        }

        protected override bool TryUpdateExistingItem(DbfArticle dbfArticle)
        {
            var existingArticle = DalController.Instance.Articles.FirstOrDefault(article =>
                article.Code == dbfArticle.Code);

            if (existingArticle == null)
                return false;

            existingArticle.Name = dbfArticle.Name ?? string.Empty;
            existingArticle.Barcode = dbfArticle.Barcode ?? string.Empty;
            existingArticle.IsActive = true;

            var articlePrice = existingArticle.ArticlePrices.FirstOrDefault(price =>
                price.ArticlePriceType == PriceType && price.EntryDate == dbfArticle.EntryDate);

            if (articlePrice != null)
            {
                articlePrice.Value = dbfArticle.Price;
                return true;
            }

            var newArticlePrice = new ArticlePrice
            {
                Article = existingArticle,
                ArticlePriceType = PriceType,
                EntryDate = dbfArticle.EntryDate,
                Value = dbfArticle.Price
            };

            existingArticle.ArticlePrices.Add(newArticlePrice);
            return true;
        }
    }
}