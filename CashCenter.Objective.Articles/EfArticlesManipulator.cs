using System.Collections.Generic;
using CashCenter.Dal;
using CashCenter.Dal.DataManipulationInterfaces;

namespace CashCenter.Objective.Articles
{
    public class EfArticlesManipulator : IArticlesManipulatable
    {
        public IEnumerable<Article> Articles
        {
            get { return RepositoriesFactory.Get<Article>().GetAll(); }
        }

        public IEnumerable<ArticlePriceType> ArticlePriceTypes
        {
            get { return RepositoriesFactory.Get<ArticlePriceType>().GetAll(); }
        }

        public IEnumerable<ArticlePrice> ArticlePrices
        {
            get { return RepositoriesFactory.Get<ArticlePrice>().GetAll(); }
        }

        public ArticleSale AddArticleSale(ArticleSale articleSale)
        {
            RepositoriesFactory.Get<ArticleSale>().Add(articleSale);
            return articleSale;
        }
    }
}
