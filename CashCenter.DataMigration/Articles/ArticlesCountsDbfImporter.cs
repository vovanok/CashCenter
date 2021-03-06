﻿using System.Linq;
using System.Collections.Generic;
using CashCenter.Dal;
using CashCenter.DataMigration.Import;
using CashCenter.DataMigration.Providers.Dbf.Entities;

namespace CashCenter.DataMigration.Articles
{
    public class ArticlesCountsDbfImporter : BaseDbfImporter<DbfArticleQuantity, Article>
    {
        public bool IsAddQuantities { get; set; }

        protected override void CreateNewItems(IEnumerable<Article> itemsForCreation)
        {
            // Новые позиции не создаются, только обновляется количество товара
        }

        protected override int DeleteAllTargetItems()
        {
            // Элементы не удаляются, только обновляется количество товара
            return 0;
        }

        protected override IEnumerable<DbfArticleQuantity> GetSourceItems()
        {
            if (dbfRegistry == null)
                return new List<DbfArticleQuantity>();

            return dbfRegistry.GetArticleQuantities();
        }

        protected override Article GetTargetItemBySource(DbfArticleQuantity sourceItem)
        {
            return null;
        }

        protected override bool TryUpdateExistingItem(DbfArticleQuantity dbfArticleQuantity)
        {
            var existingArticle = RepositoriesFactory.Get<Article>().Get(article =>
                article.Code == dbfArticleQuantity.ArticleCode);

            if (existingArticle == null)
                return false;

            if (IsAddQuantities)
            {
                existingArticle.Quantity += dbfArticleQuantity.Quantity;
            }
            else
            {
                existingArticle.Quantity = dbfArticleQuantity.Quantity;
            }

            existingArticle.Measure = dbfArticleQuantity.Measure;
            return true;
        }
    }
}
