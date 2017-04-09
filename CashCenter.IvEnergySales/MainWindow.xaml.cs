using System.Windows;
using CashCenter.IvEnergySales.Check;
using CashCenter.Common;

namespace CashCenter.IvEnergySales
{
    public partial class MainWindow : Window
	{
        private CheckPrinter checkPrinter;

        public MainWindow()
		{
			InitializeComponent();

            try
            {
                checkPrinter = new CheckPrinter();
                customerControl.CheckPrinter = checkPrinter;
            }
            catch
            {
                Log.Error("Ошибка создания драйвера. Запустите приложение от имени администратора.");
            }
        }

        #region Menu events handlers

        private void On_miCashPrinterSettings_Click(object sender, RoutedEventArgs e)
        {
            checkPrinter.ShowProperties();
        }

        private void On_miCashPrinterCancelCheck_Click(object sender, RoutedEventArgs e)
        {
            checkPrinter.CancelCheck();
        }

        private void On_miSettings_Click(object sender, RoutedEventArgs e)
        {
            var settings = new SettingsDialog();
            var settingsDialogResult = settings.ShowDialog();

            if (settingsDialogResult != null && settingsDialogResult.Value)
                customerControl.SettingsWereChanged();
        }

        private void miDataMigration_Click(object sender, RoutedEventArgs e)
        {
            var dataMigrationDialog = new DataMigrationDialog();
            dataMigrationDialog.ShowDialog();
        }

        #endregion
    }
}
