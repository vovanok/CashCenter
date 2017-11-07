using CashCenter.DataMigration.Dbf;

namespace CashCenter.DataMigration.Providers.Dbf.Entities
{
    public class DbfArticleQuantity
    {
        [CharacterDbfColumn("TOVARKOD")]
        public string ArticleCode { get; private set; }

        [NumericDbfColumn("TOVARKOL")]
        public double Quantity { get; private set; }

        [CharacterDbfColumn("EDIZMNAME")]
        public string Measure { get; private set; }

        public DbfArticleQuantity()
        {
        }

        public DbfArticleQuantity(string articleCode, double quantity, string measure)
        {
            ArticleCode = articleCode;
            Quantity = quantity;
            Measure = measure;
        }
    }
}
