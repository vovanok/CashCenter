namespace CashCenter.DataMigration.Providers.Word.Entities
{
    public class ArticlesSalesReportItem
    {
        public string ArticleName { get; private set; }
        public string ArticleCode { get; private set; }
        public double ArticleQuantity { get; private set; }
        public decimal ArticlePrice { get; private set; }
        public decimal ArticleCost { get; private set; }

        public ArticlesSalesReportItem(string articleName, string articleCode,
            double articleQuantity, decimal articlePrice, decimal articleCost)
        {
            ArticleName = articleName;
            ArticleCode = articleCode;
            ArticleQuantity = articleQuantity;
            ArticlePrice = articlePrice;
            ArticleCost = articleCost;
        }
    }
}
