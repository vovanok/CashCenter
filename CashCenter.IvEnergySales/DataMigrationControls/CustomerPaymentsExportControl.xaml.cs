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

            if (MessageBox.Show($"Вы уверены, что хотите экспортировать плтежи физ.лиц в {messageEnd}?",
                "Подтверждение", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                return;
            }

            if (dpBeginPeriod.SelectedDate == null)
            {
                Log.Error("Начальная дата не выбрана");
                return;
            }

            if (dpEndPeriod.SelectedDate == null)
            {
                Log.Error("Конечная дата не выбрана");
                return;
            }

            var beginDatetime = dpBeginPeriod.SelectedDate.Value.DayBegin();
            var endDatetime = dpEndPeriod.SelectedDate.Value.DayEnd();

            var exportResult = exporter.Export(beginDatetime, endDatetime);

            if (exportResult.SuccessCount == 0 && exportResult.FailCount == 0)
            {
                Log.Info("Нет платежей для экспортирования.");
                return;
            }

            Log.Info($"Экспортировано {exportResult.SuccessCount} из {exportResult.SuccessCount + exportResult.FailCount} платежей.");
        }
    }
}
