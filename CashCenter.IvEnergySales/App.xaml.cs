﻿using System.Windows;
using System.Windows.Threading;
using CashCenter.Common;
using CashCenter.ViewCommon;

namespace CashCenter.IvEnergySales
{
	public partial class App : Application
	{
        private App()
        {
            Log.Info(">>> ПРИЛОЖЕНИЕ ОТКРЫТО <<<");

            DispatcherUnhandledException += App_DispatcherUnhandledException;
            Exit += App_Exit;
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var errorHeader = "Неизвестная ошибка";
            Log.Error(errorHeader, e.Exception);
            Message.Error(errorHeader);

            e.Handled = false;
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
            Exit -= App_Exit;

            Log.Info(">>> ПРИЛОЖЕНИЕ ЗАКРЫТО <<<");
        }
    }
}
