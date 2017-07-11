using System;
using System.Collections.Generic;

namespace CashCenter.DataMigration.Providers.Word.Entities
{
    public class ReportArticlesSalesModel
    {
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public decimal TotalCost { get; private set; }
        public decimal NdsValue { get; private set; }
        public IEnumerable<ReportArticleSaleModel> ArticlesSales { get; private set; }

        public ReportArticlesSalesModel(DateTime startDate, DateTime endDate, decimal totalCost, decimal ndsValue,
            IEnumerable<ReportArticleSaleModel> articlesSales)
        {
            StartDate = startDate;
            EndDate = endDate;
            TotalCost = totalCost;
            NdsValue = ndsValue;
            ArticlesSales = articlesSales ?? new List<ReportArticleSaleModel>();
        }
    }
}
