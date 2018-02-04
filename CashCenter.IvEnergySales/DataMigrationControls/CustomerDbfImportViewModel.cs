using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using CashCenter.Dal;
using CashCenter.Common;
using CashCenter.DataMigration.Import;
using CashCenter.DataMigration.EnergyCustomers;
using CashCenter.DataMigration.WaterCustomers;
using CashCenter.DataMigration.Articles;

namespace CashCenter.IvEnergySales.DataMigrationControls
{
    public class CustomerDbfImportViewModel : ViewModel
    {
        public IEnumerable<ImportTargetItem> ImportTargets { get; } = new[]
            {
                new ImportTargetItem("Потребители электроэнергии", new EnergyCustomersDbfImporter(), true, false, false),
                new ImportTargetItem("Потребители воды", new WaterCustomersDbfImporter(), false, false, false),
                new ImportTargetItem("Товары", new ArticlesDbfImporter(), false, true, false),
                new ImportTargetItem("Пополнения товаров", new ArticlesCountsDbfImporter(), false, false, true)
            };

        public List<ArticlePriceType> ArticlePriceTypes { get; } = DalController.Instance.ArticlePriceTypes.ToList();

        public Observed<ImportTargetItem> SelectedImportTarget { get; } = new Observed<ImportTargetItem>();
        public Observed<string> DbfFilename { get; } = new Observed<string>();
        public Observed<Department> SelectedDepartment { get; } = new Observed<Department>();
        public Observed<ArticlePriceType> SelectedArticlePriceType { get; } = new Observed<ArticlePriceType>();
        public Observed<bool> IsAddArticleQuantities { get; } = new Observed<bool>();

        public Command ImportCommand { get; }

        public CustomerDbfImportViewModel()
        {
            SelectedImportTarget.OnChange += (newValue) => DispatchPropertyChanged("SelectedImportTarget");
            DbfFilename.OnChange += (newValue) => DispatchPropertyChanged("DbfFilename");
            SelectedDepartment.OnChange += (newValue) => DispatchPropertyChanged("SelectedDepartment");
            SelectedArticlePriceType.OnChange += (newValue) => DispatchPropertyChanged("SelectedArticlePriceType");
            IsAddArticleQuantities.OnChange += (newValue) => DispatchPropertyChanged("IsAddArticleQuantities");

            ImportCommand = new Command(DoImport);

            SelectedImportTarget.Value = ImportTargets.FirstOrDefault();
            SelectedArticlePriceType.Value = DalController.Instance.ArticlePriceTypes.FirstOrDefault();
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

            if (SelectedImportTarget.Value.Importer is ArticlesDbfImporter articlesDbfImporter)
            {
                if (SelectedArticlePriceType.Value == null)
                {
                    Message.Error("Тип цены не выбран");
                    return;
                }

                articlesDbfImporter.PriceType = SelectedArticlePriceType.Value;
            }

            if (SelectedImportTarget.Value.Importer is ArticlesCountsDbfImporter articlesCountsDbfImporter)
            {
                articlesCountsDbfImporter.IsAddQuantities = IsAddArticleQuantities.Value;
            }

            var resultStatistic = ImportItem(SelectedImportTarget.Value);

            if (SelectedImportTarget.Value.Importer is ArticlesCountsDbfImporter)
            {
                GlobalEvents.DispatchArticlesUpdated();
            }

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
