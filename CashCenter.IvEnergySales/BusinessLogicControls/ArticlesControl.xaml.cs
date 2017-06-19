﻿using CashCenter.Check;
using CashCenter.Common;
using CashCenter.Dal;
using CashCenter.IvEnergySales.Check;
using CashCenter.IvEnergySales.Common;
using System;
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

        public ArticlesControl()
        {
            InitializeComponent();
        }

        private void UpdateArticles()
        {
            var code = tbArticleCodeFilter.Text;
            var name = tbArticleNameFilter.Text;
            var barcode = tbArticleBarcodeFilter.Text;

            var filteredArticles = DalController.Instance.Articles.Where(article =>
                    (!string.IsNullOrEmpty(code) && article.Code.IndexOf(code, StringComparison.CurrentCultureIgnoreCase) >= 0) ||
                    (!string.IsNullOrEmpty(name) && article.Name.IndexOf(name, StringComparison.CurrentCultureIgnoreCase) >= 0) ||
                    (!string.IsNullOrEmpty(barcode) && article.Barcode.IndexOf(barcode, StringComparison.CurrentCultureIgnoreCase) >= 0));

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

            var prices = DalController.Instance.GetArticlePrices()
                .Where(price => price.ArticleId == selectedArticleItem.Article.Id).ToList();

            var articlePriceWithTypeItems = DalController.Instance.GetArticlePrices()
                .Where(price => price.ArticleId == selectedArticleItem.Article.Id)
                .GroupBy(price => price.ArticlePriceTypeId)
                .Select(pricesByType => pricesByType.FirstOrDefault(g => g.EntryDate == pricesByType.Max(item => item.EntryDate)))
                .Where(price => price != null)
                .Select(price =>
                {
                    var type = DalController.Instance.ArticlePriceTypes.FirstOrDefault(priceType => priceType.Id == price.ArticlePriceTypeId);
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

            if (!double.TryParse(tbQuantity.Text, out double quantity))
            {
                Message.Error("Количество товара должно быть числом");
                return;
            }

            if (quantity <= 0)
            {
                Message.Error("Количество товара должно быть положительно");
                return;
            }

            decimal totalCost = (decimal)quantity * selectedArticlePriceWithTypeItem.Price.Value;

            if (isWithoutCheck || TryPrintChecks(totalCost))

                DalController.Instance.AddArticleSale(
                new ArticleSale
                {
                    ArticlePriceId = selectedArticlePriceWithTypeItem.Price.Id,
                    CreateDate = DateTime.Now,
                    Quantity = quantity
                });

            ClearForm();
        }

        private bool TryPrintChecks(decimal totalCost)
        {
            try
            {
                using (var waiter = new OperationWaiter())
                {
                    var check = new ArticleSaleCheck(totalCost);

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

            tbQuantity.Text = "0 руб.";
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
