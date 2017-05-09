using CashCenter.Common;
using CashCenter.DataMigration;
using CashCenter.DataMigration.WaterCustomers;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace CashCenter.IvEnergySales.DataMigrationControls
{
    public class CustomerDbfImportViewModel : ViewModel
    {
        public IEnumerable<ImportTargetItem> ImportTargets { get; } = new[]
            {
                new ImportTargetItem("Потребители электроэнергии", new CustomersDbfImporter()),
                new ImportTargetItem("Потребители воды", new WaterCustomersDbfImporter())
            };

        public Observed<ImportTargetItem> SelectedImportTarget { get; } = new Observed<ImportTargetItem>();
        public Observed<string> DbfFilename { get; } = new Observed<string>();

        public Command ImportCommand { get; }

        public CustomerDbfImportViewModel()
        {
            SelectedImportTarget.OnChange += (newValue) => DispatchPropertyChanged("SelectedImportTarget");
            DbfFilename.OnChange += (newValue) => DispatchPropertyChanged("DbfFilename");

            ImportCommand = new Command(DoImport);

            SelectedImportTarget.Value = ImportTargets.FirstOrDefault();
        }

        private void DoImport(object parameters)
        {
            if (!File.Exists(DbfFilename.Value))
            {
                Message.Error("DBF файл не задан или не существует.");
                return;
            }

            if (!Message.YesNoQuestion($"Вы уверены что хотите произвести импорт потребителей электроэнергии из файла {DbfFilename.Value}."))
                return;

            if (SelectedImportTarget.Value == null)
            {
                Message.Error("Не выбрано что импортировать");
                return;
            }

            var dbfImporter = SelectedImportTarget.Value.Importer as IDbfImporter;
            if (dbfImporter != null)
                dbfImporter.DbfFilename = DbfFilename.Value;

            var resultStatistic = MigrationHelper.ImportItem(SelectedImportTarget.Value);

            Logger.Info(resultStatistic);
            Message.Info(resultStatistic);
        }
    }
}
