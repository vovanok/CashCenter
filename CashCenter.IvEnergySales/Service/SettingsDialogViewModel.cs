using System.Linq;
using System.Windows;
using System.Collections.Generic;
using CashCenter.Common;
using CashCenter.IvEnergySales.Common;
using CashCenter.Dal;
using System.Text;
using System;

namespace CashCenter.IvEnergySales.Service
{
    public class SettingsDialogViewModel : ViewModel
    {
        public Observed<string> CashierName { get; } = new Observed<string>();
        public List<Department> Deparments { get; }
        public Observed<string> ArticlesDocumentNumber { get; } = new Observed<string>();
        public Observed<string> ArticlesWarehouseCode { get; } = new Observed<string>();
        public Observed<string> ArticlesWarehouseName { get; } = new Observed<string>();

        public Command SaveCommand { get; }
        public Command CloseCommand { get; }

        public SettingsDialogViewModel()
        {
            CashierName.OnChange += (newValue) => DispatchPropertyChanged("CashierName");
            ArticlesDocumentNumber.OnChange += (newValue) => DispatchPropertyChanged("ArticlesDocumentNumber");
            ArticlesWarehouseCode.OnChange += (newValue) => DispatchPropertyChanged("ArticlesWarehouseCode");
            ArticlesWarehouseName.OnChange += (newValue) => DispatchPropertyChanged("ArticlesWarehouseName");

            CashierName.Value = Settings.CasherName;
            ArticlesDocumentNumber.Value = Settings.ArticlesDocumentNumber;
            ArticlesWarehouseCode.Value = Settings.ArticlesWarehouseCode;
            ArticlesWarehouseName.Value = Settings.ArticlesWarehouseName;

            Deparments = DalController.Instance.Departments
                .Where(department => department.RegionId == Config.CurrentRegionId).ToList();

            SaveCommand = new Command(SaveHandler);
            CloseCommand = new Command(CloseHandler);
        }

        private void SaveHandler(object data)
        {
            Settings.CasherName = CashierName.Value;

            var warnindMessage = new StringBuilder();
            HandleLengthLimitedSetting((value) => Settings.ArticlesDocumentNumber = value,
                ArticlesDocumentNumber.Value, "Номер документа (для экспорта товаров)", 8, warnindMessage);

            HandleLengthLimitedSetting((value) => Settings.ArticlesWarehouseCode = value,
                ArticlesWarehouseCode.Value, "Код склада (для экспорта товаров)", 5, warnindMessage);

            HandleLengthLimitedSetting((value) => Settings.ArticlesWarehouseName = value,
                ArticlesWarehouseName.Value, "Название склада (для экспорта товаров)", 25, warnindMessage);

            if (warnindMessage.Length > 0)
                Message.Info(warnindMessage.ToString());

            Settings.Save();

            DalController.Instance.Save();
            GlobalEvents.DispatchDepartmentsChanged();

            var window = data as Window;
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
