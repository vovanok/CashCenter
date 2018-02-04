using CashCenter.Check;
using CashCenter.Common;
using CashCenter.Dal;
using CashCenter.Dal.DataManipulationInterfaces;
using CashCenter.IvEnergySales.Check;
using CashCenter.ZeusDal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace CashCenter.IvEnergySales
{
    public partial class ArticlesControl : UserControl
    {
        private class ArticleItem
        {
            public Article Article { get; private set; }
            public string QuantityMeasure { get { return string.Format("{0} {1}", Article.Quantity, Article.Measure); } }

            public ArticleItem(Article article)
            {
                Article = article;
            }
        }

        private class ArticlePriceWithType
        {
            public ArticlePrice Price { get; private set; }
            public ArticlePriceType Type { get; private set; }

            public ArticlePriceWithType(ArticlePrice price, ArticlePriceType type)
            {
                Price = price;
                Type = type;
            }
        }

        private IArticlesManipulatable articleManipulator;

        public ArticlesControl()
        {
            InitializeComponent();

            GlobalEvents.OnArticlesUpdated += UpdateArticles;

            UpdateArticleManipulator();
            GlobalEvents.OnArticlesManipulatorTypeChanged += UpdateArticleManipulator;
            GlobalEvents.OnArticlesZeusDbUrlChanged += UpdateArticleManipulator;
            GlobalEvents.OnArticlesZeusDbPathChanged += UpdateArticleManipulator;
        }

        private void UpdateArticleManipulator()
        {
            if (Settings.ArticlesManipulatorType == ManipulatorType.Local)
            {
                articleManipulator = DalController.Instance;
            }
            else if (Settings.ArticlesManipulatorType == ManipulatorType.Zeus)
            {
                articleManipulator = new ZeusContext(Settings.ArticlesZeusDbUrl, Settings.ArticlesZeusDbPath);
            }

            UpdateArticles();
        }

        private void UpdateArticles()
        {
            if (articleManipulator == null)
                return;

            var code = tbArticleCodeFilter.Text;
            var name = tbArticleNameFilter.Text;
            var barcode = tbArticleBarcodeFilter.Text;

            var filteredArticles =
                string.IsNullOrEmpty(code) && string.IsNullOrEmpty(name) && string.IsNullOrEmpty(barcode)
                    ? new List<Article>()
                    : articleManipulator.Articles.Where(article =>
                        (string.IsNullOrEmpty(code) || article.Code.IndexOf(code, StringComparison.CurrentCultureIgnoreCase) >= 0) &&
                        (string.IsNullOrEmpty(name) || article.Name.IndexOf(name, StringComparison.CurrentCultureIgnoreCase) >= 0) &&
                        (string.IsNullOrEmpty(barcode) || article.Barcode.IndexOf(barcode, StringComparison.CurrentCultureIgnoreCase) >= 0));

            var articlesInfo = filteredArticles.Select(article => new ArticleItem(article));

            dgArticles.ItemsSource = articlesInfo;
        }

        #region Filters handlers

        private void On_tbArticleCodeFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateArticles();
        }

        private void On_tbArticleNameFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateArticles();
        }

        private void On_tbArticleBarcodeFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateArticles();
        }

        #endregion

        private void On_dgArticles_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            e.Cancel = true;
        }

        private void On_dgArticles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedArticleItem = dgArticles.SelectedItem as ArticleItem;
            if (selectedArticleItem == null)
            {
                dgPricesTypes.ItemsSource = null;
                return;
            }

            var prices = articleManipulator.ArticlePrices
                .Where(price => price.ArticleId == selectedArticleItem.Article.Id).ToList();

            var articlePriceWithTypeItems = articleManipulator.ArticlePrices
                .Where(price => price.ArticleId == selectedArticleItem.Article.Id)
                .GroupBy(price => price.ArticlePriceTypeId)
                .Select(pricesByType => pricesByType.FirstOrDefault(g => g.EntryDate == pricesByType.Max(item => item.EntryDate)))
                .Where(price => price != null)
                .Select(price =>
                {
                    var type = articleManipulator.ArticlePriceTypes.FirstOrDefault(priceType => priceType.Id == price.ArticlePriceTypeId);
                    return new ArticlePriceWithType(price, type);
                });

            dgPricesTypes.ItemsSource = articlePriceWithTypeItems;
            if (dgPricesTypes.Items.Count > 0)
                dgPricesTypes.SelectedIndex = 0;
        }

        private void On_dgPricesTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdatePrice();
        }

        private void On_dgPricesTypes_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            e.Cancel = true;
        }

        private void On_tbQuantity_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            tbQuantity.SelectAll();
        }

        private void On_tbQuantity_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!tbQuantity.IsKeyboardFocusWithin)
            {
                e.Handled = true;
                tbQuantity.Focus();
            }
        }

        private void On_btnPay_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var isWithoutCheck = false;
            if (!CheckPrinter.IsReady)
            {
                if (!Message.YesNoQuestion("Кассовый аппарат не подключен. Продолжить без печати чека?"))
                    return;

                isWithoutCheck = true;
            }

            var selectedArticleItem = dgArticles.SelectedItem as ArticleItem;
            if (selectedArticleItem == null)
            {
                Message.Error("Товар не выбран");
                return;
            }

            var selectedArticlePriceWithTypeItem = dgPricesTypes.SelectedItem as ArticlePriceWithType;
            if (selectedArticlePriceWithTypeItem == null)
            {
                Message.Error("Цена не выбрана");
                return;
            }

            double quantity = StringUtils.ForseDoubleParse(tbQuantity.Text);
            if (quantity <= 0)
            {
                Message.Error("Количество товара должно быть положительным числом");
                return;
            }

            if (quantity > selectedArticleItem.Article.Quantity)
            {
                Message.Error("Заданное количество товара отсутствует на складе");
                return;
            }

            decimal cost = selectedArticlePriceWithTypeItem.Price.Value;
            decimal totalCost = (decimal)quantity * cost;

            if (isWithoutCheck || TryPrintChecks(cost, totalCost, quantity, selectedArticleItem.Article.Name))
            {
                selectedArticleItem.Article.Quantity -= quantity;

                articleManipulator.AddArticleSale(
                    new ArticleSale
                    {
                        ArticlePriceId = selectedArticlePriceWithTypeItem.Price.Id,
                        CreateDate = DateTime.Now,
                        Quantity = quantity
                    });
            }

            ClearForm();
        }

        private bool TryPrintChecks(decimal cost, decimal totalCost, double quantity, string articleName)
        {
            try
            {
                using (var waiter = new OperationWaiter())
                {
                    var check = new ArticleSaleCheck(cost, totalCost, quantity, Settings.CasherName, articleName);

                    CheckPrinter.Print(check);
                    return true;
                }
            }
            catch (Exception ex)
            {
                var errorMessage = "Ошибка печати чека";
                Log.Error(errorMessage, ex);
                Message.Error(errorMessage);
                return false;
            }
        }

        private void ClearForm()
        {
            tbArticleCodeFilter.Text = string.Empty;
            tbArticleNameFilter.Text = string.Empty;
            tbArticleBarcodeFilter.Text = string.Empty;

            tbQuantity.Text = 0.ToString();
        }

        private void On_tbQuantity_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdatePrice();
        }

        private void UpdatePrice()
        {
            if (lblPrice == null)
                return;

            var selectedArticlePriceWithTypeItem = dgPricesTypes.SelectedItem as ArticlePriceWithType;

            if (selectedArticlePriceWithTypeItem == null || !double.TryParse(tbQuantity.Text, out double quantity))
            {
                lblPrice.Content = "0 руб.";
                return;
            }

            lblPrice.Content = (selectedArticlePriceWithTypeItem.Price.Value * (decimal)quantity).ToString("0.00");
        }
    }
}
