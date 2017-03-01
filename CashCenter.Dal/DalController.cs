using System;
using System.Collections.Generic;
using System.Linq;

namespace CashCenter.Dal
{
    public class DalController
    {
        private readonly CashCenterContext context = new CashCenterContext();

        private static DalController instance;

        public static DalController Instance
        {
            get
            {
                if (instance == null)
                    instance = new DalController();

                return instance;
            }
        }

        #region Articles

        public Article GetArticleByCode(string code)
        {
            return context.Articles.FirstOrDefault(article => article.Code == code);
        }

        public IEnumerable<Article> GetArticles()
        {
            return context.Articles;
        }

        public Article AddArticle(Article article)
        {
            var newArticle = context.Articles.Add(article);
            context.SaveChanges();
            return newArticle;
        }

        public IEnumerable<Article> AddArticleRange(IEnumerable<Article> articles)
        {
            var newArticles = context.Articles.AddRange(articles);
            context.SaveChanges();
            return newArticles;
        }

        public IEnumerable<Article> DeleteArticleRange(IEnumerable<Article> articles)
        {
            var deletedArticles = context.Articles.RemoveRange(articles);
            context.SaveChanges();
            return deletedArticles;
        }

        public IEnumerable<Article> GetArticlesByFilter(string codeFilter, string nameFilter, string barcodeFilter)
        {
            return GetArticles().Where(article =>
                (!string.IsNullOrEmpty(codeFilter) && article.Code.IndexOf(codeFilter, StringComparison.CurrentCultureIgnoreCase) >= 0) ||
                (!string.IsNullOrEmpty(nameFilter) && article.Name.IndexOf(nameFilter, StringComparison.CurrentCultureIgnoreCase) >= 0) ||
                (!string.IsNullOrEmpty(nameFilter) && article.Barcode.IndexOf(barcodeFilter, StringComparison.CurrentCultureIgnoreCase) >= 0));
        }

        #endregion

        #region Artile price types

        public ArticlePriceType GetArticlePriceTypeByCode(string code)
        {
            return context.ArticlePriceTypes.FirstOrDefault(articlePriceType => articlePriceType.Code == code);
        }

        public IEnumerable<ArticlePriceType> GetArticlePriceTypes()
        {
            return context.ArticlePriceTypes;
        }

        public ArticlePriceType AddArticlePriceType(ArticlePriceType articlePriceType)
        {
            var newArticlePriceTypes = context.ArticlePriceTypes.Add(articlePriceType);
            context.SaveChanges();
            return newArticlePriceTypes;
        }

        public IEnumerable<ArticlePriceType> AddArticlePriceTypeRange(IEnumerable<ArticlePriceType> articlePriceTypes)
        {
            var newArticlePriceTypes = context.ArticlePriceTypes.AddRange(articlePriceTypes);
            context.SaveChanges();
            return newArticlePriceTypes;
        }

        #endregion

        #region Article prices

        public List<ArticlePrice> GetArticlePrices()
        {
            return context.ArticlePrices.ToList();
        }

        public ArticlePrice AddArticlePrice(ArticlePrice articlePrice)
        {
            var newArticlePrice = context.ArticlePrices.Add(articlePrice);
            context.SaveChanges();
            return newArticlePrice;
        }

        public IEnumerable<ArticlePrice> AddArticlePriceRange(IEnumerable<ArticlePrice> articlePrices)
        {
            var newAriclePrices = context.ArticlePrices.AddRange(articlePrices);
            context.SaveChanges();
            return newAriclePrices;
        }

        #endregion

        public void ClearAllArticlesData()
        {
            context.ArticleSales.RemoveRange(context.ArticleSales);
            context.ArticlePrices.RemoveRange(context.ArticlePrices);
            context.Articles.RemoveRange(context.Articles);
            context.ArticlePriceTypes.RemoveRange(context.ArticlePriceTypes);
            context.SaveChanges();
        }
    }
}
