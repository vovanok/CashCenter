using System;

namespace CashCenter.DataMigration.Providers.Dbf.Entities
{
    public class DbfArticleSale
    {
        public DateTime DocumentDateTime { get; private set; }
        public string DocumentNumber { get; private set; }
        public string WarehouseCode { get; private set; }
        public string WarehouseName { get; private set; }
        public string ArticleCode { get; private set; }
        public string ArticleName { get; private set; }
        public double ArticleQuantity { get; private set; }
        public decimal ArticlePrice { get; private set; }
        public decimal ArticleTotalPrice { get; private set; }
        public int CheckNumber { get; private set; }
        public string SerialNumber { get; private set; }
        public string Comment { get; private set; }

        public DbfArticleSale()
        {
        }

        public DbfArticleSale(DateTime documentDateTime, string documentNumber, string warehouseCode,
            string warehouseName, string articleCode, string articleName, double articleQuantity,
            decimal articlePrice, decimal articleTotalPrice, int checkNumber, string serialNumber, string comment)
        {
            DocumentDateTime = documentDateTime;
            DocumentNumber = documentNumber;
            WarehouseCode = warehouseCode;
            WarehouseName = warehouseName;
            ArticleCode = articleCode;
            ArticleName = articleName;
            ArticleQuantity = articleQuantity;
            ArticlePrice = articlePrice;
            ArticleTotalPrice = articleTotalPrice;
            CheckNumber = checkNumber;
            SerialNumber = serialNumber;
            Comment = comment;
        }
    }
}
