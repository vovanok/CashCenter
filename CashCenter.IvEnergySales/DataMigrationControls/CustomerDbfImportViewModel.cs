using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using CashCenter.Dal;
using CashCenter.Common;
using CashCenter.DataMigration;
using CashCenter.DataMigration.Import;
using CashCenter.DataMigration.WaterCustomers;
using CashCenter.IvEnergySales.Common;

namespace CashCenter.IvEnergySales.DataMigrationControls
{
    public class CustomerDbfImportViewModel : ViewModel
    {
        public IEnumerable<ImportTargetItem> ImportTargets { get; } = new[]
            {
                new ImportTargetItem("Потребители электроэнергии", new EnergyCustomersDbfImporter(), true),
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

            if (SelectedImportTarget.Value.Importer is IDbfImporter dbfImporter)
                dbfImporter.DbfFilename = DbfFilename.Value;

            if (SelectedImportTarget.Value.Importer is EnergyCustomersDbfImporter energyCustomerDbfImporter)
            {
                if (SelectedDepartment.Value == null)
                {
                    Message.Error("Отделение не выбрано");
                    return;
                }

                energyCustomerDbfImporter.TargetDepartment = SelectedDepartment.Value;
            }

            var resultStatistic = ImportItem(SelectedImportTarget.Value);

            Log.Info(resultStatistic);
            Message.Info(resultStatistic);
        }

        private string ImportItem(ImportTargetItem importTarget)
        {
            var resultStatistic = new StringBuilder();
            var waiter = new OperationWaiter();
            using (waiter)
            {
                try
                {
                    Log.Info($"Импорт \"{importTarget.Name}\"");

                    var importResult = importTarget.Importer.Import();
                    resultStatistic.AppendLine($"Результат импортирования \"{importTarget.Name}\"");
                    resultStatistic.AppendLine($"  Добавлено: {importResult.AddedCount}");
                    resultStatistic.AppendLine($"  Обновлено: {importResult.UpdatedCount}");
                    resultStatistic.AppendLine($"  Удалено: {importResult.DeletedCount}");
                }
                catch (Exception ex)
                {
                    var errorHead = $"Ошибка импортирования \"{importTarget.Name}\"";

                    Log.Error($"{errorHead}", ex);
                    resultStatistic.AppendLine(errorHead);
                }
                finally
                {
                    resultStatistic.AppendLine($"Затрачено времени: {waiter.TimeFromStart}");
                }
            }

            return resultStatistic.ToString();
        }
    }
}
