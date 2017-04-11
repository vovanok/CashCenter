using CashCenter.DataMigration;
using CashCenter.Common;
using CashCenter.Dal;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System;
using System.Text;

namespace CashCenter.IvEnergySales
{
    public partial class DataMigrationDialog : Window
    {
        public class ImportTargetItem
        {
            public bool IsChecked { get; set; }
            public string Name { get; private set; }
            public IRemoteImportiable Importer { get; private set; }

            public ImportTargetItem(string name, IRemoteImportiable importer)
            {
                IsChecked = false;
                Name = name;
                Importer = importer;
            }
        }

        private List<ImportTargetItem> importTargetItems = new List<ImportTargetItem>
        {
            new ImportTargetItem("Физ. лица", new CustomersRemoteImporter()),
            new ImportTargetItem("Основания для оплаты", new PaymentReasonsRemoteImporter()),
            new ImportTargetItem("Виды оплаты", new PaymentKindsRemoteImporter())
        };

        public DataMigrationDialog()
        {
            InitializeComponent();

            RefreshArticlePriceTypes();
            lbImportTargets.ItemsSource = importTargetItems.Where(item => item.Importer != null);

            dpBeginPeriod.SelectedDate = DateTime.Now;
            dpEndPeriod.SelectedDate = DateTime.Now;
        }

        private void On_btnImportFromDb_Click(object sender, RoutedEventArgs e)
        {
            var department = controlDepartamentSelector.SelectedDepartment;
            if (department == null)
            {
                Log.Error("Отделение не выбрано.");
                return;
            }

            if (MessageBox.Show($"Вы уверены что хотите произвести импорт из удаленной базы данных:\n{department.Url}",
                "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
            {
                return;
            }

            var resultMessage = new StringBuilder("Результат импортирования\n\n");
            var waiter = new OperationWaiter();
            using (waiter)
            {
                var selectedImportTargets = lbImportTargets.Items.OfType<ImportTargetItem>()
                    .Where(item => item != null && item.IsChecked);

                foreach (var importTargetItem in selectedImportTargets)
                {
                    var importResult = importTargetItem.Importer.Import(controlDepartamentSelector.SelectedDepartment);

                    resultMessage.AppendLine(importTargetItem.Name);
                    if (importResult == null)
                    {
                        resultMessage.AppendLine("\tОшибка импортирования");
                        continue;
                    }

                    resultMessage.AppendLine($"  Добавлено: {importResult.AddedCount}");
                    resultMessage.AppendLine($"  Обновлено: {importResult.UpdatedCount}");
                    resultMessage.AppendLine($"  Удалено: {importResult.DeletedCount}");
                }

                RefreshArticlePriceTypes();
            }

            resultMessage.AppendLine($"\nЗатрачено времени: {waiter.DeltaTime}");
            Log.Info(resultMessage.ToString());
        }

        private void On_btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void RefreshArticlePriceTypes()
        {
            var artilePriceTypeSelectorItems = DalController.Instance.ArticlePriceTypes
                .Select(artPriceType => new { Item = artPriceType, DisplayInfo = $"{artPriceType.Code} {artPriceType.Name}" });
        }

        private void On_btnImportCustomersFromDbf_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(fscCustomersDbfFileSelector.FileName) ||
                !File.Exists(fscCustomersDbfFileSelector.FileName))
            {
                Log.Error("DBF файл не задан или не существует.");
                return;
            }

            if (MessageBox.Show($"Вы уверены что хотите произвести импорт физ. лиц из файла {fscCustomersDbfFileSelector.FileName}.",
                    "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

            ImportResult importResult = new ImportResult();
            using (var waiter = new OperationWaiter())
            {
                var importer = new CustomersDbfImporter();
                importResult = importer.Import(fscCustomersDbfFileSelector.FileName);
            }

            if (importResult == null)
            {
                Log.Info("Ошибка импортирования.");
                return;
            }

            Log.Info($"Результат импортирования\nДобавлено: {importResult.AddedCount}\nОбновлено: {importResult.UpdatedCount}\nУдалено: {importResult.DeletedCount}");
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