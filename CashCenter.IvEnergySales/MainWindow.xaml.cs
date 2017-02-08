using System.Windows;
using CashCenter.IvEnergySales.Logging;
using CashCenter.IvEnergySales.Check;
using System;
using CashCenter.Common;
using CashCenter.Dal;

namespace CashCenter.IvEnergySales
{
    public partial class MainWindow : Window
	{
        private CheckPrinter checkPrinter;

        public MainWindow()
		{
			InitializeComponent();

            Log.SetLogger(new MessageBoxLog());

            try
            {
                checkPrinter = new CheckPrinter();
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
            dataMigrationDialog.Show();
        }

        #endregion
    }
}
