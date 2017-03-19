using CashCenter.Articles.DataMigration;
using CashCenter.Common;
using CashCenter.Dal;
using CashCenter.DbfRegistry;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace CashCenter.IvEnergySales
{
    public partial class DataMigrationDialog : Window
    {
        public class ImportTargetItem
        {
            public bool IsChecked { get; set; }
            public string Name { get; private set; }
            public BaseRemoteImporter Importer { get; private set; }

            public ImportTargetItem(string name, BaseRemoteImporter importer)
            {
                IsChecked = false;
                Name = name;
                Importer = importer;
            }
        }

        private List<ImportTargetItem> importTargetItems = new List<ImportTargetItem>
        {
            new ImportTargetItem("Товары", new ArticlesRemoteImporter()),
            new ImportTargetItem("Физ. лица", new CustomersRemoteImporter()),
            new ImportTargetItem("Юр. лица", null), //TODO
            new ImportTargetItem("Основания для оплаты", new PaymentReasonsRemoteImporter()),
            new ImportTargetItem("Виды оплаты", new PaymentKindsRemoteImporter())
        };

        public DataMigrationDialog()
        {
            InitializeComponent();

            RefreshArticlePriceTypes();
            lbImportTargets.ItemsSource = importTargetItems.Where(item => item.Importer != null);
        }

        private void On_btnImportArticlesFromDbf_Click(object sender, RoutedEventArgs e)
        {
            var articlePriceType = cbArticlePriceType.SelectedItem as Dal.ArticlePriceType;
            if (articlePriceType == null)
            {
                Log.Error("Не выбран тип стоимости товара. Если их нет, произведите импорт из удаленной БД.");
                return;
            }

            if (string.IsNullOrEmpty(fscArticlesDbfFileSelector.FileName))
            {
                Log.Error("DBF файл не выбран.");
                return;
            }

            if (MessageBox.Show($@"Вы уверены что хотите произвести импорт из файла: {fscArticlesDbfFileSelector.FileName}",
                "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                using (var waiter = new OperationWaiter())
                {
                    var importer = new ArticlesDbfImporter();
                    importer.PriceType = articlePriceType;
                    importer.Import(fscArticlesDbfFileSelector.FileName);
                }
            }
        }

        private void On_btnImportFromDb_Click(object sender, RoutedEventArgs e)
        {
            var department = controlDepartamentSelector.SelectedDepartment;
            if (department == null)
            {
                Log.Error("Отделение не выбрано.");
                return;
            }

            if (MessageBox.Show("Вы уверены что хотите произвести импорт из удаленной базы данных:\n"
                + $"{department.Url}\nДанные будут полностью перезаписаны", "Предупреждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                using (var waiter = new OperationWaiter())
                {
                    var selectedImportTargets = lbImportTargets.Items.OfType<ImportTargetItem>()
                        .Where(item => item != null && item.IsChecked);

                    foreach (var importTargetItem in selectedImportTargets)
                    {
                        importTargetItem.Importer.Import(controlDepartamentSelector.SelectedDepartment);
                    }

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

        private void On_btnImportCustomersFromDbf_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(fscCustomersDbfFileSelector.FileName) ||
                !File.Exists(fscCustomersDbfFileSelector.FileName))
            {
                Log.Error("DBF файл не задан или не существует.");
                return;
            }

            if (MessageBox.Show($"Вы уверены что хотите произвести импорт физ. лиц из файла {fscCustomersDbfFileSelector.FileName}.",
                    "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

            using (var waiter = new OperationWaiter())
            {
                var importer = new CustomersDbfImporter();
                importer.Import(fscCustomersDbfFileSelector.FileName);
            }

            using (var waiter = new OperationWaiter())
            {
                var importer = new CustomersDbfImporter();
                importer.Import(fscCustomersDbfFileSelector.FileName);
            }
        }

        private void On_btnExportCustomerPaymentsToOff_Click(object sender, RoutedEventArgs e)
        {
            DoExport("OFF файлы", new CustomerPaymentsOffExporter());
        }

        private void On_btnExportCustomerPaymentsToDb_Click(object sender, RoutedEventArgs e)
        {
            DoExport("базу Зевс", new CustomerPaymentsRemoteExporter());
        }

        private void DoExport(string messageEnd, BaseExporter exporter)
        {
            if (messageEnd == null || exporter == null)
                return;

            if (MessageBox.Show($"Вы уверены, что хотите экспортировать плтежи физ.лиц в {messageEnd}?",
                "Подтверждение", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                return;
            }

            if (dpBeginPeriod.SelectedDate == null)
            {
                Log.Error("Начальная дата не выбрана");
                return;
            }

            if (dpEndPeriod.SelectedDate == null)
            {
                Log.Error("Конечная дата не выбрана");
                return;
            }

            exporter.Export(dpBeginPeriod.SelectedDate.Value, dpEndPeriod.SelectedDate.Value);
        }
    }
}