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
    public class ArticleSalesSeparatedDbfExporter : BaseExporter<ArticleSale>
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

            string targetExportDirectory = Path.Combine(Config.OutputDirectory,
                string.Format("{0:dd-MM-yyyy_HH-mm-ss} ({1:dd-MM-yyyy_HH-mm-ss}___{2:dd-MM-yyyy_HH-mm-ss})",
                    DateTime.Now, beginDatetime, endDatetime));
            if (Directory.Exists(targetExportDirectory))
                Directory.Delete(targetExportDirectory);

            int exportedArticleSalesCount = 0;
            foreach (ArticleSale articleSale in articleSales)
            {
                if (articleSale == null)
                    continue;

                var salesForExport = new List<DbfArticleSale>();
                salesForExport.Add(new DbfArticleSale(
                    articleSale.CreateDate,
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

                salesForExport.Add(new DbfArticleSale(
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
                    ));

                var dbfFilename = Path.Combine(targetExportDirectory, string.Format(
                    Config.ArticlesSeparatedDbfOutputFileFormat, Settings.ArticlesWarehouseCode, Settings.ArticlesDocumentNumberCurrentValue.ToString("D5")));

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
                Settings.Save();

                exportedArticleSalesCount++;
            }

            if (exportedArticleSalesCount <= 0)
                return new ExportResult();

            return new ExportResult(exportedArticleSalesCount, articleSales.Count() - exportedArticleSalesCount);
        }
    }
}