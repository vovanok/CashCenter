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
        private readonly List<ImportTargetItem> importTargetItems = new List<ImportTargetItem>
        {
            new ImportTargetItem("Потребители электроэнергии", new CustomersRemoteImporter()),
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

            var selectedImportTargets = lbImportTargets.Items.OfType<ImportTargetItem>()
                .Where(item => item != null && item.IsChecked);

            foreach (var importTarget in selectedImportTargets)
            {
                var remoteImporter = importTarget.Importer as IRemoteImporter;
                if (remoteImporter != null)
                    remoteImporter.SourceDepartment = controlDepartamentSelector.SelectedDepartment;

                resultMessage.AppendLine(MigrationHelper.ImportItem(importTarget));
            }

            RefreshArticlePriceTypes();

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
