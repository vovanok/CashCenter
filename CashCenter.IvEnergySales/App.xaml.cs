using CashCenter.Common;
using CashCenter.IvEnergySales.Common;
using System.Windows;
using System.Windows.Threading;

namespace CashCenter.IvEnergySales
{
	public partial class App : Application
	{
        private App()
        {
            DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var errorHeader = "Неизвестная ошибка";
            Log.Error(errorHeader, e.Exception);
            Message.Error(errorHeader);

            e.Handled = false;
        }
    }
}
