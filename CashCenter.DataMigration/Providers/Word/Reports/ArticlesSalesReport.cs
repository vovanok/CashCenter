using System;
using System.Collections.Generic;
using CashCenter.DataMigration.Providers.Word.Entities;
using NetOffice.WordApi;

namespace CashCenter.DataMigration.Providers.Word.Reports
{
    public class ArticlesSalesReport : IWordReport
    {
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public decimal TotalCost { get; private set; }
        public decimal NdsValue { get; private set; }
        public IEnumerable<ArticlesSalesReportItem> ArticlesSales { get; private set; }

        public ArticlesSalesReport(DateTime startDate, DateTime endDate, decimal totalCost, decimal ndsValue,
            IEnumerable<ArticlesSalesReportItem> articlesSales)
        {
            StartDate = startDate;
            EndDate = endDate;
            TotalCost = totalCost;
            NdsValue = ndsValue;
            ArticlesSales = articlesSales ?? new List<ArticlesSalesReportItem>();
        }

        public void ExportToDocument(Document wordDocument)
        {
            if (wordDocument == null)
                return;

            wordDocument.Bookmarks["StartDate"].Range.Text = StartDate.ToString("dd MMMM yyyy г.");
            wordDocument.Bookmarks["EndDate"].Range.Text = EndDate.ToString("dd MMMM yyyy г.");

            var salesTable = wordDocument.Bookmarks["ArticlesSales"]?.Range?.Tables[1];
            if (salesTable == null)
                throw new ApplicationException("Не найдена таблица для выгрузки продаж в шаблоне отчета");

            int acticleSaleNumber = 1;
            foreach (var articleSale in ArticlesSales)
            {
                var articleSaleRow = salesTable.Rows.Add(salesTable.Rows[acticleSaleNumber + 1]);
                articleSaleRow.Cells[1].Range.Text = acticleSaleNumber++.ToString();
                articleSaleRow.Cells[2].Range.Text = articleSale.ArticleName;
                articleSaleRow.Cells[3].Range.Text = articleSale.ArticleCode;
                articleSaleRow.Cells[4].Range.Text = articleSale.ArticleQuantity.ToString("0.00");
                articleSaleRow.Cells[5].Range.Text = articleSale.ArticlePrice.ToString("0.00");
                articleSaleRow.Cells[6].Range.Text = articleSale.ArticleCost.ToString("0.00");
            }

            salesTable.Rows[acticleSaleNumber + 1].Delete();

            wordDocument.Bookmarks["TotalCost"].Range.Text = TotalCost.ToString("0.00");
            wordDocument.Bookmarks["NdsValue"].Range.Text = NdsValue.ToString("0.00");
        }
    }
}
