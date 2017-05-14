using CashCenter.Common;
using CashCenter.DataMigration;
using CashCenter.DataMigration.WaterCustomers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CashCenter.IvEnergySales.DataMigrationControls
{
    public class CustomerPaymentsExportViewModel : ViewModel
    {
        public IEnumerable<ExportTargetItem> ExportTargets { get; } = new[]
            {
                new ExportTargetItem("Платежи по электроэнергии -> OFF", new CustomerPaymentsOffExporter()),
                new ExportTargetItem("Платежи по электроэнергии -> Word", new CustomerPaymentsWordReportExporter()),
                //new ExportTargetItem("Платежи по электроэнергии -> Зевс", new CustomerPaymentsRemoteExporter()) // TODO: need debug
                new ExportTargetItem("Платежи за воду -> DBF", new WaterCustomerPaymentsDbfExporter())
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

            BeginDatetime.Value = EndDatetime.Value = DateTime.Now;
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

            var beginDatetime = BeginDatetime.Value.DayBegin();
            var endDatetime = EndDatetime.Value.DayEnd();

            try
            {
                Logger.Info($"Экспорт \"{SelectedExportTarget.Value.Name}\" с {beginDatetime} по {endDatetime}");

                var exportResult = SelectedExportTarget.Value.Exporter.Export(beginDatetime, endDatetime);
                if (exportResult.SuccessCount == 0 && exportResult.FailCount == 0)
                {
                    var nothingToExportMessage = "Нет элементов для экспортирования";
                    Logger.Info(nothingToExportMessage);
                    Message.Info(nothingToExportMessage);
                    return;
                }

                var successResultMessage = $"Экспортировано {exportResult.SuccessCount} из {exportResult.SuccessCount + exportResult.FailCount} элементов.";
                Logger.Info(successResultMessage);
                Message.Info(successResultMessage);
            }
            catch (Exception ex)
            {
                var errorHeader = $"Ошибка экспорта \"{SelectedExportTarget.Value.Name}\"";
                Logger.Error(errorHeader, ex);
                Message.Error(errorHeader);
            }
        }
    }
}
