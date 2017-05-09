using CashCenter.Common;
using CashCenter.DataMigration;
using System.IO;
using System.Windows;

namespace CashCenter.IvEnergySales.DataMigrationControls
{
    public partial class CustomerDbfImportControl : BaseImportControl
    {
        private ImportTargetItem importTargetItem =
            new ImportTargetItem("Потребители электроэнергии", new CustomersDbfImporter());

        public CustomerDbfImportControl()
        {
            InitializeComponent();
        }

        private void On_btnImportCustomersFromDbf_Click(object sender, RoutedEventArgs e)
        {
            var sourceFileName = fscCustomersDbfFileSelector.FileName;

            if (!File.Exists(sourceFileName))
            {
                Message.Error("DBF файл не задан или не существует.");
                return;
            }

            if (!Message.YesNoQuestion($"Вы уверены что хотите произвести импорт потребителей электроэнергии из файла {sourceFileName}."))
                return;

            var dbfImporter = importTargetItem.Importer as IDbfImporter;
            if (dbfImporter != null)
                dbfImporter.DbfFilename = fscCustomersDbfFileSelector.FileName;

            var resultStatistic = ImportItem(importTargetItem);

            Logger.Info(resultStatistic);
            Message.Info(resultStatistic);
        }
    }
}
