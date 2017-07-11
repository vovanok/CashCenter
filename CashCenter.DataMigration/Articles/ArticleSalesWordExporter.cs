﻿using CashCenter.Common;
using CashCenter.Dal;
using CashCenter.DataMigration.Providers.Word;
using CashCenter.DataMigration.Providers.Word.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CashCenter.DataMigration.Articles
{
    public class ArticleSalesWordExporter : BaseExporter<ArticleSale>
    {
        protected override List<ArticleSale> GetSourceItems(DateTime beginDatetime, DateTime endDatetime)
        {
            return DalController.Instance.ArticleSales.Where(articleSale =>
                beginDatetime <= articleSale.CreateDate && articleSale.CreateDate <= endDatetime).ToList();
        }

        protected override int TryExportItems(IEnumerable<ArticleSale> items)
        {
            var articleSalesModels = items.Select(item =>
                new ReportArticleSaleModel(item.ArticlePrice.Article.Name, item.ArticlePrice.Article.Code,
                    item.Quantity, item.ArticlePrice.Value, item.ArticlePrice.Value * (decimal)item.Quantity));

            var articleSalesModelsCount = articleSalesModels.Count();
            if (articleSalesModelsCount == 0)
                return 0;

            var totalCost = articleSalesModels.Sum(item => item.ArticleCost);
            var reportModel = new ReportArticlesSalesModel(beginDatetime, endDatetime, totalCost, totalCost * (decimal)0.18, articleSalesModels);

            var wordReport = new WordReportController(Config.ArticlesSalesReportTemplateFilename);
            wordReport.CreateReport(reportModel);

            return articleSalesModelsCount;
        }
    }
}