using CashCenter.Common;
using CashCenter.DataMigration;
using CashCenter.DataMigration.WaterCustomers;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using CashCenter.Dal;

namespace CashCenter.IvEnergySales.DataMigrationControls
{
    public class CustomerDbfImportViewModel : ViewModel
    {
        public IEnumerable<ImportTargetItem> ImportTargets { get; } = new[]
            {
                new ImportTargetItem("Потребители электроэнергии", new CustomersDbfImporter(), true),
                new ImportTargetItem("Потребители воды", new WaterCustomersDbfImporter(), false)
            };

        public Observed<ImportTargetItem> SelectedImportTarget { get; } = new Observed<ImportTargetItem>();
        public Observed<string> DbfFilename { get; } = new Observed<string>();
        public Observed<Department> SelectedDepartment { get; } = new Observed<Department>();

        public Command ImportCommand { get; }

        public CustomerDbfImportViewModel()
        {
            SelectedImportTarget.OnChange += (newValue) => DispatchPropertyChanged("SelectedImportTarget");
            DbfFilename.OnChange += (newValue) => DispatchPropertyChanged("DbfFilename");
            SelectedDepartment.OnChange += (newValue) => DispatchPropertyChanged("SelectedDepartment");

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

            var energyCustomerDbfImporter = SelectedImportTarget.Value.Importer as CustomersDbfImporter;
            if (energyCustomerDbfImporter != null)
            {
                if (SelectedDepartment.Value == null)
                {
                    Message.Error("Отделение не выбрано");
                    return;
                }

                energyCustomerDbfImporter.TargetDepartment = SelectedDepartment.Value;
            }

            var resultStatistic = MigrationHelper.ImportItem(SelectedImportTarget.Value);

            Logger.Info(resultStatistic);
            Message.Info(resultStatistic);
        }
    }
}
