using CashCenter.Common;
using CashCenter.Dal;
using CashCenter.DataMigration;
using System;
using System.Windows;
using System.Windows.Controls;

namespace CashCenter.IvEnergySales.DataMigrationControls
{
    public partial class CustomerPaymentsExportControl : UserControl
    {
        public CustomerPaymentsExportControl()
        {
            InitializeComponent();

            dpBeginPeriod.SelectedDate = DateTime.Now;
            dpEndPeriod.SelectedDate = DateTime.Now;
        }

        private void On_btnExportCustomerPaymentsToOff_Click(object sender, RoutedEventArgs e)
        {
            DoExport("OFF файлы", new CustomerPaymentsOffExporter());
        }

        private void On_btnExportCustomerPaymentsToDb_Click(object sender, RoutedEventArgs e)
        {
            DoExport("базу Зевс", new CustomerPaymentsRemoteExporter());
        }

        private void On_btnExportCustomerPaymentsToWordReport_Click(object sender, RoutedEventArgs e)
        {
            DoExport("отчет Word", new CustomerPaymentsWordReportExporter());
        }

        private void DoExport(string messageEnd, BaseExporter<CustomerPayment> exporter)
        {
            if (messageEnd == null || exporter == null)
                return;

            if (!Message.YesNoQuestion($"Вы уверены, что хотите экспортировать платежи за электроэнергию в {messageEnd}?"))
                return;

            if (dpBeginPeriod.SelectedDate == null || dpEndPeriod.SelectedDate == null)
            {
                Message.Error("Начальная или конечная дата не выбрана");
                return;
            }

            var beginDatetime = dpBeginPeriod.SelectedDate.Value.DayBegin();
            var endDatetime = dpEndPeriod.SelectedDate.Value.DayEnd();

            try
            {
                Logger.Info($"Экспорта платежей за электроэнергию с {beginDatetime} по {endDatetime}");

                var exportResult = exporter.Export(beginDatetime, endDatetime);
                if (exportResult.SuccessCount == 0 && exportResult.FailCount == 0)
                {
                    var nothingToExportMessage = "Нет платежей для экспортирования";
                    Logger.Info(nothingToExportMessage);
                    Message.Info(nothingToExportMessage);
                    return;
                }

                var successResultMessage = $"Экспортировано {exportResult.SuccessCount} из {exportResult.SuccessCount + exportResult.FailCount} платежей.";
                Logger.Info(successResultMessage);
                Message.Info(successResultMessage);
            }
            catch (Exception ex)
            {
                var errorHeader = "Ошибка экспорта платежей за электроэнергию";
                Logger.Error(errorHeader, ex);
                Message.Error(errorHeader);
            }
        }
    }
}
