using CashCenter.Common;
using CashCenter.DataMigration;
using System.IO;
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
    }
}
