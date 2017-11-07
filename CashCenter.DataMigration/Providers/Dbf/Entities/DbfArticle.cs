using CashCenter.DataMigration.Dbf;
using System;

namespace CashCenter.DataMigration.Providers.Dbf.Entities
{
    public class DbfArticle
    {
        [CharacterDbfColumn("TOVARKOD")]
        public string Code { get; private set; }

        [CharacterDbfColumn("TOVARNAME")]
        public string Name { get; private set; }

        [CharacterDbfColumn("SHTRIHKOD")]
        public string Barcode { get; private set; }

        [MoneyDbfColumn("TOVARCENA")]
        public decimal Price { get; private set; }

        [DateDbfColumn("DATAN")]
        public DateTime EntryDate { get; private set; }

        public DbfArticle()
        {
        }

        public DbfArticle(string code, string name, string barcode, decimal price, DateTime entryDate)
        {
            Code = code ?? string.Empty;
            Name = name ?? string.Empty;
            Barcode = barcode ?? string.Empty;
            Price = price;
            EntryDate = entryDate;
        }
    }
}
