using CashCenter.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;

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

        #region Common

        public IEnumerable<PaymentReason> PaymentReasons
        {
            get { return context.PaymentReasons; }
        }

        public IEnumerable<Department> Departments
        {
            get { return context.Departments; }
        }

        public Department GetDepartmentByCode(string code)
        {
            return context.Departments.FirstOrDefault(item => item.Code == code);
        }

        public IEnumerable<PaymentReason> AddPaymentReasonsRange(IEnumerable<PaymentReason> paymentReasons)
        {
            try
            {
                var newPaymentReasons = context.PaymentReasons.AddRange(paymentReasons);
                Save();
                return newPaymentReasons;
            }
            catch (Exception ex)
            {
                HandleEntityFrameworkError("Ошибка добавления оснований оплаты", ex);
                return null;
            }
        }

        #endregion

        #region Articles

        public IEnumerable<Article> Articles
        {
            get { return context.Articles; }
        }

        public Article AddArticle(Article article)
        {
            try
            {
                var newArticle = context.Articles.Add(article);
                Save();
                return newArticle;
            }
            catch (Exception ex)
            {
                HandleEntityFrameworkError("Ошибка добавления товара", ex);
                return null;
            }
        }

        public IEnumerable<Article> AddArticleRange(IEnumerable<Article> articles)
        {
            try
            {
                var newArticles = context.Articles.AddRange(articles);
                Save();
                return newArticles;
            }
            catch (Exception ex)
            {
                HandleEntityFrameworkError("Ошибка добавления товаров", ex);
                return null;
            }
        }

        public IEnumerable<Article> DeleteArticleRange(IEnumerable<Article> articles)
        {
            try
            {
                var deletedArticles = context.Articles.RemoveRange(articles);
                Save();
                return deletedArticles;
            }
            catch (Exception ex)
            {
                HandleEntityFrameworkError("Ошибка удаления товаров", ex);
                return null;
            }
        }

        #endregion

        #region Artile price types

        public IEnumerable<ArticlePriceType> ArticlePriceTypes
        {
            get { return context.ArticlePriceTypes; }
        }

        public ArticlePriceType AddArticlePriceType(ArticlePriceType articlePriceType)
        {
            try
            {
                var newArticlePriceTypes = context.ArticlePriceTypes.Add(articlePriceType);
                Save();
                return newArticlePriceTypes;
            }
            catch (Exception ex)
            {
                HandleEntityFrameworkError("Ошибка добавления типа стоимости товара", ex);
                return null;
            }
        }

        public IEnumerable<ArticlePriceType> AddArticlePriceTypeRange(IEnumerable<ArticlePriceType> articlePriceTypes)
        {
            try
            {
                var newArticlePriceTypes = context.ArticlePriceTypes.AddRange(articlePriceTypes);
                Save();
                return newArticlePriceTypes;
            }
            catch (Exception ex)
            {
                HandleEntityFrameworkError("Ошибка добавления типов стоимости товара", ex);
                return null;
            }
        }

        #endregion

        #region Article prices

        public IEnumerable<ArticlePrice> GetArticlePrices()
        {
            return context.ArticlePrices;
        }

        public ArticlePrice AddArticlePrice(ArticlePrice articlePrice)
        {
            try
            {
                var newArticlePrice = context.ArticlePrices.Add(articlePrice);
                Save();
                return newArticlePrice;
            }
            catch (Exception ex)
            {
                HandleEntityFrameworkError("Ошибка добавления стоимости товара", ex);
                return null;
            }
        }

        public IEnumerable<ArticlePrice> AddArticlePriceRange(IEnumerable<ArticlePrice> articlePrices)
        {
            try
            {
                var newAriclePrices = context.ArticlePrices.AddRange(articlePrices);
                Save();
                return newAriclePrices;
            }
            catch (Exception ex)
            {
                HandleEntityFrameworkError("Ошибка добавления стоимостей товара", ex);
                return null;
            }
        }

        #endregion

        #region Article sales

        public ArticleSale AddArticleSale(ArticleSale artilceSale)
        {
            try
            {
                var newArticle = context.ArticleSales.Add(artilceSale);
                Save();
                return newArticle;
            }
            catch (Exception ex)
            {
                HandleEntityFrameworkError("Ошибка добавления платежа по товару", ex);
                return null;
            }
        }

        #endregion

        #region Customer

        public IEnumerable<Customer> Customers
        {
            get { return context.Customers; }
        }

        public IEnumerable<CustomerPayment> CustomerPayments
        {
            get { return context.CustomerPayments; }
        }

        public Customer AddCustomer(Customer customer)
        {
            try
            {
                var resultCustomer = context.Customers.Add(customer);
                Save();
                return resultCustomer;
            }
            catch (Exception ex)
            {
                HandleEntityFrameworkError("Ошибка добавления физ. лица", ex);
                return null;
            }
        }

        public IEnumerable<Customer> AddCustomersRange(IEnumerable<Customer> customers)
        {
            try
            {
                var resultCustomers = context.Customers.AddRange(customers);
                Save();
                return resultCustomers;
            }
            catch (Exception ex)
            {
                HandleEntityFrameworkError("Ошибка добавления физ. лиц", ex);
                return null;
            }
        }

        public CustomerPayment AddCustomerPayment(CustomerPayment customerPayment)
        {
            try
            {
                var newCustomerPayment = context.CustomerPayments.Add(customerPayment);
                Save();
                return newCustomerPayment;
            }
            catch (Exception ex)
            {
                HandleEntityFrameworkError("Ошибка добавления платежа физ. лица", ex);
                return null;
            }
        }

        #endregion

        #region Clear routines

        public void ClearAllArticlesData()
        {
            try
            {
                context.ArticleSales.RemoveRange(context.ArticleSales);
                context.ArticlePrices.RemoveRange(context.ArticlePrices);
                context.Articles.RemoveRange(context.Articles);
                context.ArticlePriceTypes.RemoveRange(context.ArticlePriceTypes);
                Save();
            }
            catch (Exception ex)
            {
                HandleEntityFrameworkError("Ошибка очистки данных о товарах.", ex);
            }
        }

        public void ClearAllCustomersData()
        {
            try
            {
                context.Customers.RemoveRange(context.Customers);
                Save();
            }
            catch (Exception ex)
            {
                HandleEntityFrameworkError("Ошибка очистки данных о физ. лицах", ex);
            }
        }

        public void ClearPaymentReasons()
        {
            try
            {
                context.PaymentReasons.RemoveRange(context.PaymentReasons);
                Save();
            }
            catch (Exception ex)
            {
                HandleEntityFrameworkError("Ошибка очистки данных об основаниях оплаты", ex);
            }
        }

        public void Save()
        {
            context.SaveChanges();
        }

        #endregion

        #region Regions and departments

        public IEnumerable<Region> Regions
        {
            get { return context.Regions; }
        }

        #endregion

        #region Water

        public IEnumerable<WaterCustomer> WaterCustomers
        {
            get { return context.WaterCustomers; }
        }

        public IEnumerable<WaterCustomerPayment> WaterCustomerPayments
        {
            get { return context.WaterCustomerPayments; }
        }

        public WaterCustomer AddWaterCustomer(WaterCustomer customer)
        {
            try
            {
                var resultCustomer = context.WaterCustomers.Add(customer);
                Save();
                return resultCustomer;
            }
            catch (Exception ex)
            {
                HandleEntityFrameworkError("Ошибка добавления потребителя воды", ex);
                return null;
            }
        }

        public IEnumerable<WaterCustomer> AddWaterCustomersRange(IEnumerable<WaterCustomer> customers)
        {
            try
            {
                var resultCustomers = context.WaterCustomers.AddRange(customers);
                Save();
                return resultCustomers;
            }
            catch (Exception ex)
            {
                HandleEntityFrameworkError("Ошибка добавления потребителей воды", ex);
                return null;
            }
        }
        
        public WaterCustomerPayment AddWaterCustomerPayment(WaterCustomerPayment customerPayment)
        {
            try
            {
                var newCustomerPayment = context.WaterCustomerPayments.Add(customerPayment);
                Save();
                return newCustomerPayment;
            }
            catch (Exception ex)
            {
                HandleEntityFrameworkError("Ошибка добавления платежа за воду", ex);
                return null;
            }
        }

        #endregion

        private void HandleEntityFrameworkError(string message, Exception exception)
        {
            var entityValidationException = exception as DbEntityValidationException;
            if (entityValidationException == null)
            {
                if (exception.InnerException != null)
                {
                    HandleEntityFrameworkError(message, exception.InnerException);
                    return;
                }
                
                Log.Error(message, exception);
                return;
            }

            var sbErrorContent = new StringBuilder();
            foreach (var entityValidationError in entityValidationException.EntityValidationErrors)
            {
                sbErrorContent.AppendLine($"Таблица {entityValidationError.Entry.Entity.GetType().Name} в состоянии {entityValidationError.Entry.State} ошибки:");
                foreach (var validationError in entityValidationError.ValidationErrors)
                {
                    sbErrorContent.AppendLine($"\tСвойство: {validationError.PropertyName}, ошибка: {validationError.ErrorMessage}");
                }
            }

            Log.Error($"{message}\n{sbErrorContent}", exception);
        }
    }
}