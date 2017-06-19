using System.Collections.Generic;

namespace CashCenter.IvEnergySales.Check
{
    public class ArticleSaleCheck : CashCenter.Check.Check
    {
        public ArticleSaleCheck(decimal cost, decimal totalCost, double quantity, string cashierName, string articleName)
            : base(
                "ArticleSale",
                new Dictionary<string, string>
                {
                    { "cashierName", cashierName },
                    { "articleName", articleName }
                },
                cost,
                totalCost,
                quantity,
                string.Empty)
        {
        }
    }
}
