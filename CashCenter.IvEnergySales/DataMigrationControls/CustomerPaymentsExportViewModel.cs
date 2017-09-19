using System;
using System.Linq;
using System.Collections.Generic;
using CashCenter.Common;
using CashCenter.DataMigration.EnergyCustomers;
using CashCenter.DataMigration.WaterCustomers;
using CashCenter.DataMigration.Articles;
using CashCenter.IvEnergySales.Common;

namespace CashCenter.IvEnergySales.DataMigrationControls
{
    public class CustomerPaymentsExportViewModel : ViewModel
    {
        public IEnumerable<ExportTargetItem> ExportTargets { get; } = new[]
            {
                new ExportTargetItem("Платежи по электроэнергии -> OFF", new EnergyCustomerPaymentsOffExporter()),
                new ExportTargetItem("Платежи по электроэнергии -> Word", new EnergyCustomerPaymentsWordExporter()),
                new ExportTargetItem("Платежи за воду -> DBF", new WaterCustomerPaymentsDbfExporter()),
                new ExportTargetItem("Платежи за воду -> Word", new WaterCustomerPaymentsWordExporter()),
                new ExportTargetItem("Покупки товаров -> DBF", new ArticleSalesDbfExporter()),
                new ExportTargetItem("Покупки товаров -> Word", new ArticleSalesWordExporter())
            };

        public Observed<ExportTargetItem> SelectedExportTarget { get; } = new Observed<ExportTargetItem>();
        public Observed<DateTime> BeginDatetime { get; } = new Observed<DateTime>();
        public Observed<DateTime> EndDatetime { get; } = new Observed<DateTime>();

        public Command ExportCommand { get; }

        public CustomerPaymentsExportViewModel()
        {
            SelectedExportTarget.OnChange += (newValue) => DispatchPropertyChanged("SelectedExportTarget");
            BeginDatetime.OnChange += (newValue) => DispatchPropertyChanged("BeginDatetime");
            EndDatetime.OnChange += (newValue) => DispatchPropertyChanged("EndDatetime");

            ExportCommand = new Command(DoExport);

            BeginDatetime.Value = DateTime.Now.DayBegin();
            EndDatetime.Value = DateTime.Now.DayEnd();
            SelectedExportTarget.Value = ExportTargets.FirstOrDefault();
        }

        private void DoExport(object parameters)
        {
            if (SelectedExportTarget.Value == null)
            {
                Message.Error("Не выбран вид экспорта");
                return;
            }

            if (!Message.YesNoQuestion($"Вы уверены, что хотите произвести экспорт \"{SelectedExportTarget.Value.Name}\"?"))
                return;

            var beginDatetime = BeginDatetime.Value;
            var endDatetime = EndDatetime.Value;

            try
            {
                Log.Info($"Экспорт \"{SelectedExportTarget.Value.Name}\" с {beginDatetime} по {endDatetime}");

                var exportResult = SelectedExportTarget.Value.Exporter.Export(beginDatetime, endDatetime);
                if (exportResult.SuccessCount == 0 && exportResult.FailCount == 0)
                {
                    var nothingToExportMessage = "Нет элементов для экспортирования";
                    Log.Info(nothingToExportMessage);
                    Message.Info(nothingToExportMessage);
                    return;
                }

                var successResultMessage = $"Экспортировано {exportResult.SuccessCount} из {exportResult.SuccessCount + exportResult.FailCount} элементов.";
                Log.Info(successResultMessage);
                Message.Info(successResultMessage);
            }
            catch (Exception ex)
            {
                var errorHeader = $"Ошибка экспорта \"{SelectedExportTarget.Value.Name}\"";
                Log.Error(errorHeader, ex);
                Message.Error(errorHeader);
            }
        }
    }
}
