using CashCenter.Articles.DataMigration;
using CashCenter.Common;
using CashCenter.Dal;
using System.Linq;
using System.Windows;

namespace CashCenter.IvEnergySales
{
    public partial class DataMigrationDialog : Window
    {
        public DataMigrationDialog()
        {
            InitializeComponent();

            RefreshArticlePriceTypes();
        }

        private void On_btnImportArticlesFromDbf_Click(object sender, RoutedEventArgs e)
        {
            var articlePriceType = cbArticlePriceType.SelectedItem as Dal.ArticlePriceType;
            if (articlePriceType == null)
            {
                Log.Error("Не выбран тип стоимости товара. Если их нет, произведите импорт из удаленной БД.");
                return;
            }

            if (string.IsNullOrEmpty(fscDbfFileSelector.FileName))
            {
                Log.Error("DBF файл не выбран.");
                return;
            }

            if (MessageBox.Show($@"Вы уверены что хотите произвести импорт из файла: {fscDbfFileSelector.FileName}",
                "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                using (var waiter = new OperationWaiter())
                {
                    var importer = new ArticlesImporter();
                    importer.ImportFromFile(fscDbfFileSelector.FileName, articlePriceType);
                }
            }
        }

        private void On_btnImportArticlesFromDb_Click(object sender, RoutedEventArgs e)
        {
            var department = controlDepartamentSelector.SelectedDepartment;

            if (department == null)
            {
                Log.Error("Отделение не выбрано.");
                return;
            }

            if (MessageBox.Show("Вы уверены что хотите произвести импорт из удаленной базы данных:\n"
                + $"{department.Url}\nДанные о товарах и их ценах будут полностью перезаписаны", "Предупреждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                using (var waiter = new OperationWaiter())
                {
                    var importer = new ArticlesImporter();
                    importer.ImportFromDb(controlDepartamentSelector.SelectedDepartment);

                    RefreshArticlePriceTypes();
                }
            }
        }

        private void On_btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void RefreshArticlePriceTypes()
        {
            var artilePriceTypeSelectorItems =
                DalController.Instance.GetArticlePriceTypes()
                .Select(artPriceType => new { Item = artPriceType, DisplayInfo = $"{artPriceType.Code} {artPriceType.Name}" });

            cbArticlePriceType.ItemsSource = artilePriceTypeSelectorItems;
            if (cbArticlePriceType.Items.Count > 0)
                cbArticlePriceType.SelectedIndex = 0;
        }
    }
}