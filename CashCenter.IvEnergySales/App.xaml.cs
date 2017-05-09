using CashCenter.Common;
using CashCenter.IvEnergySales.Logging;
using System.Windows;
using System.Windows.Threading;

namespace CashCenter.IvEnergySales
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
        private App()
        {
            Log.SetLogger(new MessageBoxLog());
            DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var errorHeader = "Неизвестная ошибка";
            Logger.Error(errorHeader, e.Exception);
            Message.Error(errorHeader);

            e.Handled = false;
        }
    }
}
