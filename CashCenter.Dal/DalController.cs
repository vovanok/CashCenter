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

        public IEnumerable<ArticlePrice> GetArticlePrices()
        {
            return context.ArticlePrices;
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

        #region Article sales

        public ArticleSale AddArticleSale(ArticleSale artilceSale)
        {
            var newArticle = context.ArticleSales.Add(artilceSale);
            context.SaveChanges();
            return newArticle;
        }

        #endregion

        #region Customer

        public Customer GetCustomerById(int id)
        {
            return context.Customers.FirstOrDefault(item => item.Id == id);
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
