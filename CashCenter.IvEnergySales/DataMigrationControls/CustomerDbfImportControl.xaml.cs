using CashCenter.Common;
using CashCenter.DataMigration;
using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace CashCenter.IvEnergySales.DataMigrationControls
{
    public partial class CustomerDbfImportControl : UserControl
    {
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

            if (!Message.YesNoQuestion($"Вы уверены что хотите произвести импорт физ. лиц из файла {sourceFileName}."))
                return;

            ImportResult importResult = new ImportResult();
            var resultMessage = new StringBuilder();
            resultMessage.AppendLine("Результат импортирования потребителей электроэнергии\n\n");

            try
            {
                using (var waiter = new OperationWaiter())
                {
                    var importer = new CustomersDbfImporter();
                    importResult = importer.Import(fscCustomersDbfFileSelector.FileName);
                    resultMessage.AppendLine($"  Добавлено: {importResult.AddedCount}");
                    resultMessage.AppendLine($"  Обновлено: {importResult.UpdatedCount}");
                    resultMessage.AppendLine($"  Удалено: {importResult.DeletedCount}");
                    resultMessage.AppendLine();
                }
            }
            catch (Exception ex)
            {
                var errorHead = $"Ошибка импортирования потребителей электроэнергии";

                Logger.Error($"{errorHead}.\n{ex.Message}");
                resultMessage.AppendLine(errorHead);
            }

            Logger.Info(resultMessage.ToString());
            Message.Info(resultMessage.ToString());
        }
    }
}
