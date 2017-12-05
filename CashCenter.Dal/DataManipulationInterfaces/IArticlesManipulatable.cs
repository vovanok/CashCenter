using System.Collections.Generic;

namespace CashCenter.Dal.DataManipulationInterfaces
{
    public interface IArticlesManipulatable
    {
        IEnumerable<Article> Articles { get; }
        IEnumerable<ArticlePriceType> ArticlePriceTypes { get; }
        IEnumerable<ArticlePrice> ArticlePrices { get; }

        ArticleSale AddArticleSale(ArticleSale articleSale);
    }
}
