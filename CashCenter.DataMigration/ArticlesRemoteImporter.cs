using CashCenter.Common;
using CashCenter.Common.DbQualification;
using CashCenter.Dal;
using CashCenter.ZeusDb;
using System;
using System.Linq;

namespace CashCenter.DataMigration
{
    public class ArticlesRemoteImporter : BaseRemoteImporter
    {
        public override void Import(DepartmentDef departmentDef)
        {
            if (departmentDef == null)
            {
                Log.Error("Для импорта из БД не указано отделение.");
                return;
            }

            try
            {
                DalController.Instance.ClearAllArticlesData();

                var db = new ZeusDbController(departmentDef);

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

                var articles = DalController.Instance.Articles.ToList();
                var articlePriceTypes = DalController.Instance.ArticlePriceTypes;

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
            catch (Exception ex)
            {
                Log.ErrorWithException("Ошибка импортирования данных из удаленной БД", ex);
            }
        }
    }
}
