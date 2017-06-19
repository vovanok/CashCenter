using System.Collections.Generic;

namespace CashCenter.IvEnergySales.Check
{
    public class ArticleSaleCheck : CashCenter.Check.Check
    {
        public ArticleSaleCheck(decimal totalCost)
            : base("ArticleSale", new Dictionary<string, string>(), totalCost, string.Empty)
        {
        }
    }
}
