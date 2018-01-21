using System;
using System.Text;
using System.Linq;
using System.Windows;
using System.Collections.Generic;
using CashCenter.Common;
using CashCenter.IvEnergySales.Common;
using CashCenter.Dal;

namespace CashCenter.IvEnergySales.Service
{
    public class SettingsDialogViewModel : ViewModel
    {
        public Observed<string> CashierName { get; } = new Observed<string>();
        public List<Department> Deparments { get; }
        public Observed<int> ArticlesDocumentNumberCurrentValue { get; } = new Observed<int>();
        public Observed<string> ArticlesWarehouseCode { get; } = new Observed<string>();
        public Observed<string> ArticlesWarehouseName { get; } = new Observed<string>();
        public Observed<ManipulatorType> ArticlesManipulatorType { get; } = new Observed<ManipulatorType>();
        public Observed<string> ArticlesZeusDbUrl { get; } = new Observed<string>();
        public Observed<string> ArticlesZeusDbPath { get; } = new Observed<string>();
        public Observed<float> WaterСommissionPercent { get; } = new Observed<float>();
        public Observed<int> GarbageCollectionFilialCode { get; } = new Observed<int>();
        public Observed<float> GarbageCollectionComissionPercent { get; } = new Observed<float>();
        public Observed<int> RepairFilialCode { get; } = new Observed<int>();
        public Observed<float> RepairComissionPercent { get; } = new Observed<float>();
        public Observed<string> EnergyPerformerInn { get; } = new Observed<string>();
        public Observed<string> EnergyPerformerName { get; } = new Observed<string>();
        public Observed<string> EnergyPerformerKpp { get; } = new Observed<string>();
        public Observed<string> EnergyReceiverInn { get; } = new Observed<string>();
        public Observed<string> EnergyReceiverName { get; } = new Observed<string>();
        public Observed<string> EnergyReceiverBankName { get; } = new Observed<string>();
        public Observed<string> EnergyReceiverBankBik { get; } = new Observed<string>();
        public Observed<string> EnergyReceiverAccount { get; } = new Observed<string>();

        public Command SaveCommand { get; }
        public Command CloseCommand { get; }

        public SettingsDialogViewModel()
        {
            CashierName.OnChange += (newValue) => DispatchPropertyChanged("CashierName");
            ArticlesDocumentNumberCurrentValue.OnChange += (newValue) => DispatchPropertyChanged("ArticlesDocumentNumberCurrentValue");
            ArticlesWarehouseCode.OnChange += (newValue) => DispatchPropertyChanged("ArticlesWarehouseCode");
            ArticlesWarehouseName.OnChange += (newValue) => DispatchPropertyChanged("ArticlesWarehouseName");
            ArticlesManipulatorType.OnChange += (newValue) => DispatchPropertyChanged("ArticlesManipulatorType");
            ArticlesZeusDbUrl.OnChange += (newValue) => DispatchPropertyChanged("ArticlesZeusDbUrl");
            ArticlesZeusDbPath.OnChange += (newValue) => DispatchPropertyChanged("ArticlesZeusDbPath");
            WaterСommissionPercent.OnChange += (newValue) => DispatchPropertyChanged("WaterСommissionPercent");
            GarbageCollectionFilialCode.OnChange += (newValue) => DispatchPropertyChanged("GarbageCollectionFilialCode");
            GarbageCollectionComissionPercent.OnChange += (newValue) => DispatchPropertyChanged("GarbageCollectionComissionPercent");
            RepairFilialCode.OnChange += (newValue) => DispatchPropertyChanged("RepairFilialCode");
            RepairComissionPercent.OnChange += (newValue) => DispatchPropertyChanged("RepairComissionPercent");
            EnergyPerformerInn.OnChange += (newValue) => DispatchPropertyChanged("EnergyPerformerInn");
            EnergyPerformerName.OnChange += (newValue) => DispatchPropertyChanged("EnergyPerformerName");
            EnergyPerformerKpp.OnChange += (newValue) => DispatchPropertyChanged("EnergyPerformerKpp");
            EnergyReceiverInn.OnChange += (newValue) => DispatchPropertyChanged("EnergyReceiverInn");
            EnergyReceiverName.OnChange += (newValue) => DispatchPropertyChanged("EnergyReceiverName");
            EnergyReceiverBankName.OnChange += (newValue) => DispatchPropertyChanged("EnergyReceiverBankName");
            EnergyReceiverBankBik.OnChange += (newValue) => DispatchPropertyChanged("EnergyReceiverBankBik");
            EnergyReceiverAccount.OnChange += (newValue) => DispatchPropertyChanged("EnergyReceiverAccount");

            CashierName.Value = Settings.CasherName;
            ArticlesDocumentNumberCurrentValue.Value = Settings.ArticlesDocumentNumberCurrentValue;
            ArticlesWarehouseCode.Value = Settings.ArticlesWarehouseCode;
            ArticlesWarehouseName.Value = Settings.ArticlesWarehouseName;
            ArticlesManipulatorType.Value = Settings.ArticlesManipulatorType;
            ArticlesZeusDbUrl.Value = Settings.ArticlesZeusDbUrl;
            ArticlesZeusDbPath.Value = Settings.ArticlesZeusDbPath;
            WaterСommissionPercent.Value = Settings.WaterСommissionPercent;
            GarbageCollectionFilialCode.Value = Settings.GarbageCollectionFilialCode;
            GarbageCollectionComissionPercent.Value = Settings.GarbageCollectionCommissionPercent;
            RepairFilialCode.Value = Settings.RepairFilialCode;
            RepairComissionPercent.Value = Settings.RepairCommissionPercent;
            EnergyPerformerInn.Value = Settings.EnergyPerformerInn;
            EnergyPerformerName.Value = Settings.EnergyPerformerName;
            EnergyPerformerKpp.Value = Settings.EnergyPerformerKpp;
            EnergyReceiverInn.Value = Settings.EnergyReceiverInn;
            EnergyReceiverName.Value = Settings.EnergyReceiverName;
            EnergyReceiverBankName.Value = Settings.EnergyReceiverBankName;
            EnergyReceiverBankBik.Value = Settings.EnergyReceiverBankBik;
            EnergyReceiverAccount.Value = Settings.EnergyReceiverAccount;

            Deparments = DalController.Instance.Departments
                .Where(department => department.RegionId == Config.CurrentRegionId).ToList();

            SaveCommand = new Command(SaveHandler);
            CloseCommand = new Command(CloseHandler);
        }

        private void SaveHandler(object parameters)
        {
            var controlForValidate = parameters as DependencyObject;
            if (!IsValid(controlForValidate))
            {
                Message.Error("При вводе были допущены ошибки. Исправьте их и попробуйте снова.\nОшибочные поля обведены красным.");
                return;
            }

            // Имя кассира
            Settings.CasherName = CashierName.Value;

            // Текущий номер документа для эксорта товаров
            Settings.ArticlesDocumentNumberCurrentValue = ArticlesDocumentNumberCurrentValue.Value;

            // Код склада
            if (ArticlesWarehouseCode.Value.Length > 3)
            {
                Message.Error("Длина поля \"Код склада (для экспорта товаров)\" не должна превышать 3 символов");
                return;
            }

            Settings.ArticlesWarehouseCode = ArticlesWarehouseCode.Value;

            // Название склада
            if (ArticlesWarehouseName.Value.Length > 25)
            {
                Message.Error("Длина поля \"Название склада (для экспорта товаров)\" не должна превышать 25 символов");
                return;
            }

            Settings.ArticlesWarehouseName = ArticlesWarehouseName.Value;

            //
            if (WaterСommissionPercent.Value < 0 || WaterСommissionPercent.Value > 100)
            {
                Message.Error("% комиссии за оплату воды должен быть от 0 до 100");
                return;
            }

            Settings.WaterСommissionPercent = WaterСommissionPercent.Value;

            //
            var oldArticlesManipulatorType = Settings.ArticlesManipulatorType;
            Settings.ArticlesManipulatorType = ArticlesManipulatorType.Value;
            if (oldArticlesManipulatorType != ArticlesManipulatorType.Value)
            {
                GlobalEvents.DispatchArticlesManipulatorTypeChanged();
            }

            //
            var oldArticlesZeusDbUrl = Settings.ArticlesZeusDbUrl;
            Settings.ArticlesZeusDbUrl = ArticlesZeusDbUrl.Value;
            if (oldArticlesZeusDbUrl != ArticlesZeusDbUrl.Value)
            {
                GlobalEvents.DispatchArticlesZeusDbUrlChanged();
            }

            //
            var oldArticlesZeusDbPath = Settings.ArticlesZeusDbPath;
            Settings.ArticlesZeusDbPath = ArticlesZeusDbPath.Value;
            if (oldArticlesZeusDbPath != ArticlesZeusDbPath.Value)
            {
                GlobalEvents.DispatchArticlesZeusDbPathChanged();
            }

            //
            if (GarbageCollectionFilialCode.Value <= 0)
            {
                Message.Error("Код филиала для приема оплаты за вывоз ТКО должен быть положительным числом");
                return;
            }

            Settings.GarbageCollectionFilialCode = GarbageCollectionFilialCode.Value;

            //
            if (GarbageCollectionComissionPercent.Value < 0 || GarbageCollectionComissionPercent.Value > 100)
            {
                Message.Error("% комиссии за оплату вывоза ТКО должен быть от 0 до 100");
                return;
            }

            Settings.GarbageCollectionCommissionPercent = GarbageCollectionComissionPercent.Value;

            //
            if (RepairFilialCode.Value <= 0)
            {
                Message.Error("Код филиала для приема оплаты за кап. ремонт должен быть положительным числом");
                return;
            }

            Settings.RepairFilialCode = RepairFilialCode.Value;

            //
            if (RepairComissionPercent.Value < 0 || RepairComissionPercent.Value > 100)
            {
                Message.Error("% комиссии за оплату за кап. ремонт должен быть от 0 до 100");
                return;
            }

            Settings.RepairCommissionPercent = RepairComissionPercent.Value;

            //
            Settings.EnergyPerformerInn = EnergyPerformerInn.Value;

            //
            Settings.EnergyPerformerName = EnergyPerformerName.Value;

            //
            Settings.EnergyPerformerKpp = EnergyPerformerKpp.Value;

            //
            Settings.EnergyReceiverInn = EnergyReceiverInn.Value;

            //
            Settings.EnergyReceiverName = EnergyReceiverName.Value;

            //
            Settings.EnergyReceiverBankName = EnergyReceiverBankName.Value;

            //
            Settings.EnergyReceiverBankBik = EnergyReceiverBankBik.Value;

            //
            Settings.EnergyReceiverAccount = EnergyReceiverAccount.Value;

            Settings.Save();

            GlobalEvents.DispatchWaterComissionPercentChanged();

            DalController.Instance.Save();
            GlobalEvents.DispatchDepartmentsChanged();

            var window = parameters as Window;
            if (window != null)
            {
                window.DialogResult = true;
                window.Close();
            }
        }

        private void HandleLengthLimitedSetting(Action<string> settingSetter, string newValue,
            string valueName, int lengthLimit, StringBuilder warnindMessage)
        {
            if (newValue.Length > lengthLimit)
            {
                settingSetter(newValue.Substring(0, lengthLimit));
                warnindMessage.AppendLine($"Поле \"{valueName}\" имеет максимальную длину {lengthLimit}. Вы ввели большее количество символов. Значение будет обрезано.");
                warnindMessage.AppendLine();
            }
            else
            {
                settingSetter(newValue);
            }
        }
        
        private void CloseHandler(object data)
        {
            DalController.Instance.DetachDepartmentsChanges();

            var window = data as Window;
            if (window != null)
            {
                window.DialogResult = false;
                window.Close();
            }
        }
    }
}
