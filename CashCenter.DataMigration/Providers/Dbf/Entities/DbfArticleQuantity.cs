namespace CashCenter.DataMigration.Providers.Dbf.Entities
{
    public class DbfArticleQuantity
    {
        public string ArticleCode { get; private set; }
        public double Quantity { get; private set; }
        public string Measure { get; private set; }

        public DbfArticleQuantity(string articleCode, double quantity, string measure)
        {
            ArticleCode = articleCode;
            Quantity = quantity;
            Measure = measure;
        }
    }
}
