using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using CashCenter.Common;
using CashCenter.Dal;
using CashCenter.DataMigration.Providers.Dbf;
using CashCenter.DataMigration.Providers.Dbf.Entities;

namespace CashCenter.DataMigration.Articles
{
    public class ArticleSalesDbfExporter : BaseExporter<ArticleSale>
    {
        protected override List<ArticleSale> GetSourceItems(DateTime beginDatetime, DateTime endDatetime)
        {
            return DalController.Instance.ArticleSales.Where(articleSale =>
                beginDatetime <= articleSale.CreateDate && articleSale.CreateDate <= endDatetime).ToList();
        }

        protected override ExportResult TryExportItems(IEnumerable<ArticleSale> articleSales)
        {
            if (articleSales == null)
                return new ExportResult();

            var salesForExport = new List<DbfArticleSale>();
            salesForExport.Add(new DbfArticleSale(
                DateTime.Now,
                Settings.ArticlesWarehouseCode + Settings.ArticlesDocumentNumberCurrentValue.ToString("D7"),
                Settings.ArticlesWarehouseCode,
                Settings.ArticlesWarehouseName,
                string.Empty,
                string.Empty,
                0,
                0,
                0,
                0, // TODO
                string.Empty, // TODO
                string.Empty // TODO
                ));

            salesForExport.AddRange(articleSales
                .Where(articleSale => articleSale != null)
                .Select(articleSale =>
                    new DbfArticleSale(
                        DateTime.MinValue,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        articleSale.ArticlePrice.Article.Code,
                        articleSale.ArticlePrice.Article.Name,
                        articleSale.Quantity,
                        articleSale.ArticlePrice.Value,
                        (decimal)articleSale.Quantity * articleSale.ArticlePrice.Value,
                        0, // TODO
                        string.Empty, // TODO
                        string.Empty // TODO
                        )));

            var countItemsForExport = salesForExport.Count() - 1;
            if (countItemsForExport <= 0)
                return new ExportResult();

            var dbfFilename = Path.Combine(Config.OutputDirectory, string.Format(Config.ArticlesDbfOutputFileFormat, DateTime.Now));

            Exception exportException = null;
            try
            {
                using (var fileBuffer = new FileBuffer(dbfFilename, FileBuffer.BufferType.Create))
                {
                    try
                    {
                        var dbfRegistry = new DbfRegistryController(fileBuffer.BufferFilename);
                        dbfRegistry.StoreArticleSales(salesForExport);
                    }
                    catch (Exception ex)
                    {
                        exportException = ex;
                    }
                }
            }
            finally
            {
                if (exportException != null)
                    throw exportException;
            }

            Settings.ArticlesDocumentNumberCurrentValue++;

            return new ExportResult(countItemsForExport, articleSales.Count() - countItemsForExport);
        }
    }
}
