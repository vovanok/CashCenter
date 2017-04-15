using CashCenter.Common;
using CashCenter.Dal;
using CashCenter.DataMigration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace CashCenter.IvEnergySales.DataMigrationControls
{
    public partial class CustomerZeusImportControl : UserControl
    {
        public class ImportTargetItem
        {
            public bool IsChecked { get; set; }
            public string Name { get; private set; }
            public IRemoteImportiable Importer { get; private set; }

            public ImportTargetItem(string name, IRemoteImportiable importer)
            {
                IsChecked = false;
                Name = name;
                Importer = importer;
            }
        }

        private List<ImportTargetItem> importTargetItems = new List<ImportTargetItem>
        {
            new ImportTargetItem("Физ. лица", new CustomersRemoteImporter()),
            new ImportTargetItem("Основания для оплаты", new PaymentReasonsRemoteImporter())
        };

        public CustomerZeusImportControl()
        {
            InitializeComponent();

            RefreshArticlePriceTypes();
            lbImportTargets.ItemsSource = importTargetItems.Where(item => item.Importer != null);
        }

        private void On_btnImportFromDb_Click(object sender, RoutedEventArgs e)
        {
            var department = controlDepartamentSelector.SelectedDepartment;
            if (department == null)
            {
                Log.Error("Отделение не выбрано.");
                return;
            }

            if (MessageBox.Show($"Вы уверены что хотите произвести импорт из удаленной базы данных:\n{department.Url}",
                "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
            {
                return;
            }

            var resultMessage = new StringBuilder("Результат импортирования\n\n");
            var waiter = new OperationWaiter();
            using (waiter)
            {
                var selectedImportTargets = lbImportTargets.Items.OfType<ImportTargetItem>()
                    .Where(item => item != null && item.IsChecked);

                foreach (var importTargetItem in selectedImportTargets)
                {
                    var importResult = importTargetItem.Importer.Import(controlDepartamentSelector.SelectedDepartment);

                    resultMessage.AppendLine(importTargetItem.Name);
                    if (importResult == null)
                    {
                        resultMessage.AppendLine("\tОшибка импортирования");
                        continue;
                    }

                    resultMessage.AppendLine($"  Добавлено: {importResult.AddedCount}");
                    resultMessage.AppendLine($"  Обновлено: {importResult.UpdatedCount}");
                    resultMessage.AppendLine($"  Удалено: {importResult.DeletedCount}");
                }

                RefreshArticlePriceTypes();
            }

            resultMessage.AppendLine($"\nЗатрачено времени: {waiter.DeltaTime}");
            Log.Info(resultMessage.ToString());
        }

        private void RefreshArticlePriceTypes()
        {
            var artilePriceTypeSelectorItems = DalController.Instance.ArticlePriceTypes
                .Select(artPriceType => new { Item = artPriceType, DisplayInfo = $"{artPriceType.Code} {artPriceType.Name}" });
        }
    }
}
