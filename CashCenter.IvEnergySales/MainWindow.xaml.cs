using CashCenter.Check;
using System.Windows;

namespace CashCenter.IvEnergySales
{
    public partial class MainWindow : Window
	{
        public MainWindow()
		{
			InitializeComponent();
        }

        #region Menu events handlers

        private void On_miCashPrinterSettings_Click(object sender, RoutedEventArgs e)
        {
            CheckPrinter.ShowProperties();
        }

        private void On_miCashPrinterCancelCheck_Click(object sender, RoutedEventArgs e)
        {
            CheckPrinter.CancelCheck();
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
