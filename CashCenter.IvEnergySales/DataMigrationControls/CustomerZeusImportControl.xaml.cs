using CashCenter.Common;
using CashCenter.Dal;
using CashCenter.DataMigration;
using System;
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
                Message.Error("Отделение не выбрано.");
                return;
            }

            if (!Message.YesNoQuestion($"Вы уверены что хотите произвести импорт из удаленной базы данных:\n{department.Url}"))
                return;

            var resultMessage = new StringBuilder();
            var waiter = new OperationWaiter();
            using (waiter)
            {
                var selectedImportTargets = lbImportTargets.Items.OfType<ImportTargetItem>()
                    .Where(item => item != null && item.IsChecked);

                foreach (var importTarget in selectedImportTargets)
                {
                    try
                    {
                        Logger.Info($"Импорт \"{importTarget.Name}\"");

                        var importResult = importTarget.Importer.Import(controlDepartamentSelector.SelectedDepartment);
                        resultMessage.AppendLine($"Результат импортирования \"{importTarget.Name}\"\n\n");
                        resultMessage.AppendLine($"  Добавлено: {importResult.AddedCount}");
                        resultMessage.AppendLine($"  Обновлено: {importResult.UpdatedCount}");
                        resultMessage.AppendLine($"  Удалено: {importResult.DeletedCount}");
                        resultMessage.AppendLine();
                    }
                    catch (Exception ex)
                    {
                        var errorHead = $"Ошибка импортирования \"{importTarget.Name}\"";

                        Logger.Error($"{errorHead}.\n{ex.Message}");
                        resultMessage.AppendLine(errorHead);
                        resultMessage.AppendLine();
                    }
                }

                RefreshArticlePriceTypes();
            }

            resultMessage.AppendLine($"\nЗатрачено времени: {waiter.DeltaTime}");

            Logger.Info(resultMessage.ToString());
            Message.Info(resultMessage.ToString());
        }

        private void RefreshArticlePriceTypes()
        {
            var artilePriceTypeSelectorItems = DalController.Instance.ArticlePriceTypes
                .Select(artPriceType => new { Item = artPriceType, DisplayInfo = $"{artPriceType.Code} {artPriceType.Name}" });
        }
    }
}
