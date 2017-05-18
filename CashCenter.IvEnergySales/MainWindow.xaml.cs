using CashCenter.Check;
using CashCenter.Common;
using System;
using System.Windows;

namespace CashCenter.IvEnergySales
{
    public partial class MainWindow : Window
	{
        private const string CHECK_PRINTER_NOTREADY_MESSAGE = "ККМ не готов";

        public MainWindow()
		{
			InitializeComponent();

            tiWaterPayment.Visibility = Config.IsShowWaterPayments ? Visibility.Visible : Visibility.Hidden;
        }

        #region Menu events handlers

        private void On_miCashPrinterSettings_Click(object sender, RoutedEventArgs e)
        {
            CheckPrinter.ShowProperties();
        }

        private void On_miCashPrinterCancelCheck_Click(object sender, RoutedEventArgs e)
        {
            if (!Message.YesNoQuestion("Вы уверены, что хотите аннулировать чек?"))
                return;

            try
            {
                CheckPrinter.CancelCheck();
            }
            catch (Exception ex)
            {
                Message.Error(CHECK_PRINTER_NOTREADY_MESSAGE);
                Logger.Error(CHECK_PRINTER_NOTREADY_MESSAGE, ex);
            }
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

        private void On_miCashPrinterCloseSession_Click(object sender, RoutedEventArgs e)
        {
            if (!Message.YesNoQuestion("Вы уверены, что хотите закрыть сессию?"))
                return;

            try
            {
                CheckPrinter.CloseSession();
            }
            catch (Exception ex)
            {
                Message.Error(CHECK_PRINTER_NOTREADY_MESSAGE);
                Logger.Error(CHECK_PRINTER_NOTREADY_MESSAGE, ex);
            }
        }

        private void On_miCashPrinterSysAdminCancelCheck_Click(object sender, RoutedEventArgs e)
        {
            if (!Message.YesNoQuestion("Вы уверены, что хотите выполнить административную отмену чека?"))
                return;

            try
            {
                CheckPrinter.SysAdminCancelCheck();
            }
            catch (Exception ex)
            {
                Message.Error(CHECK_PRINTER_NOTREADY_MESSAGE);
                Logger.Error(CHECK_PRINTER_NOTREADY_MESSAGE, ex);
            }
        }

        #endregion
    }
}
