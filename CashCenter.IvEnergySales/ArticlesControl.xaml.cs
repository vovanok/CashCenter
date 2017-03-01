using CashCenter.Dal;
using System.Linq;
using System.Windows.Controls;

namespace CashCenter.IvEnergySales
{
    /// <summary>
    /// Interaction logic for ArticlesControl.xaml
    /// </summary>
    public partial class ArticlesControl : UserControl
    {
        public ArticlesControl()
        {
            InitializeComponent();
        }

        private void UpdateArticles()
        {
            var filteredArticles = DalController.Instance.GetArticlesByFilter(tbArticleCodeFilter.Text, tbArticleNameFilter.Text, tbArticleBarcodeFilter.Text);
            var articlesInfo = filteredArticles.Select(article =>
                new
                {
                    Code = article.Code,
                    Name = article.Name,
                    Barcode = article.Barcode,
                    QuantityMeasure = string.Format("{0} {1}", article.Quantity, article.Measure)
                });

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
    }
}
