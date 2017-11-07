using System;
using CashCenter.DataMigration.Dbf;

namespace CashCenter.DataMigration.Providers.Dbf.Entities
{
    public class DbfArticleSale
    {
        [DateDbfColumn("DATADOC")]
        public DateTime DocumentDateTime { get; private set; }

        [CharacterDbfColumn("NOMERDOC", 10)]
        public string DocumentNumber { get; private set; }

        [CharacterDbfColumn("SKLADKOD", 3)]
        public string WarehouseCode { get; private set; }

        [CharacterDbfColumn("SKLADNAME", 25)]
        public string WarehouseName { get; private set; }

        [CharacterDbfColumn("TOVARKOD", 10)]
        public string ArticleCode { get; private set; }

        [CharacterDbfColumn("TOVARNAME", 50)]
        public string ArticleName { get; private set; }

        [NumericDbfColumn("TOVARKOL")]
        public double ArticleQuantity { get; private set; }

        [MoneyDbfColumn("TOVARCENA")]
        public decimal ArticlePrice { get; private set; }

        [MoneyDbfColumn("TOVARSUMMA")]
        public decimal ArticleTotalPrice { get; private set; }

        [NumericDbfColumn("NOMERCHEKA")]
        public int CheckNumber { get; private set; }

        [CharacterDbfColumn("SERNOMER", 50)]
        public string SerialNumber { get; private set; }

        [CharacterDbfColumn("KOMMENT", 100)]
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
