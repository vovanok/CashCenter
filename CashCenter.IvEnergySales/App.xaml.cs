﻿using CashCenter.Check;
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
            Log.ErrorWithException("Глобальная ошибка в приложении.", e.Exception);
            e.Handled = false;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            CheckPrinter.CloseSession();
        }
    }
}
