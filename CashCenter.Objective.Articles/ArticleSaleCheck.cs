using CashCenter.Common;
using System.Collections.Generic;

namespace CashCenter.Objective.Articles
{
    public class ArticleSaleCheck : CashCenter.Check.Check
    {
        public ArticleSaleCheck(decimal cost, decimal totalCost, double quantity, string cashierName, string articleName)
            : base(
                descriptorId: "ArticleSale",
                parameters: new Dictionary<string, string>
                {
                    { "cashierName", cashierName },
                    { "articleName", articleName }
                },
                cost: cost,
                totalCost: totalCost,
                quantity: quantity,
                email: string.Empty,
                ndsPercent: Config.ArticlesNdsPercent)
        {
        }
    }
}
