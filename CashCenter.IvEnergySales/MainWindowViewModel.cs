﻿using System;
using System.Reflection;
using CashCenter.Common;
using CashCenter.ViewCommon;
using CashCenter.Check;

namespace CashCenter.IvEnergySales
{
    public class MainWindowViewModel : ViewModel
    {
        private const string CHECK_PRINTER_NOTREADY_MESSAGE = "ККМ не готов";

        public Version Version { get; } = Assembly.GetExecutingAssembly().GetName().Version;

        public Command CashMachineSettings { get; }
        public Command CashMachineCancelCheck { get; }
        public Command CashMachineSysAdminCancelCheck { get; }
        public Command CashMachineCloseSession { get; }
        public Command Settings { get; }
        public Command DataMigration { get; }

        public bool IsWaterPaymentVisible { get { return Config.IsShowWaterPayments && !(DateTime.Now > Config.DeathDate); } }
        public bool IsArticlesVisible { get { return Config.IsShowArticles && !(DateTime.Now > Config.DeathDate); } }
        public bool IsGarbageCollectionVisible { get { return Config.IsShowGarbageCollection && !(DateTime.Now > Config.DeathDate); } }
        public bool IsHotWaterVisible { get { return Config.IsShowHotWater && !(DateTime.Now > Config.DeathDate); } }
        public bool IsDocumentReceiptVisible { get { return Config.IsShowDocumentReceipt && !(DateTime.Now > Config.DeathDate); } }

        public MainWindowViewModel()
        {
            CashMachineSettings = new Command(CashMachineSettingsHandler);
            CashMachineCancelCheck = new Command(CashMachineCancelCheckHandler);
            CashMachineSysAdminCancelCheck = new Command(CashMachineSysAdminCancelCheckHandler);
            CashMachineCloseSession = new Command(CashMachineCloseSessionHandler);
            Settings = new Command(SettingsHandler);
            DataMigration = new Command(DataMigrationHandler);
        }

        private void CashMachineSettingsHandler(object param)
        {
            CheckPrinter.ShowProperties();
        }

        private void CashMachineCancelCheckHandler(object param)
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
                Log.Error(CHECK_PRINTER_NOTREADY_MESSAGE, ex);
            }
        }

        private void CashMachineSysAdminCancelCheckHandler(object param)
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
                Log.Error(CHECK_PRINTER_NOTREADY_MESSAGE, ex);
            }
        }

        private void CashMachineCloseSessionHandler(object param)
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
                Log.Error(CHECK_PRINTER_NOTREADY_MESSAGE, ex);
            }
        }

        private void SettingsHandler(object param)
        {
            new SettingsDialog().ShowDialog();
        }

        private void DataMigrationHandler(object param)
        {
            new DataMigrationDialog().ShowDialog();
        }
    }
}
