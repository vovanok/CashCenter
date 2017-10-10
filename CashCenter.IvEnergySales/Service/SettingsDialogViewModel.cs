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
        public Observed<float> СommissionPercent { get; } = new Observed<float>();
        public Observed<int> GarbageCollectionFilialCode { get; } = new Observed<int>();

        public Command SaveCommand { get; }
        public Command CloseCommand { get; }

        public SettingsDialogViewModel()
        {
            CashierName.OnChange += (newValue) => DispatchPropertyChanged("CashierName");
            ArticlesDocumentNumberCurrentValue.OnChange += (newValue) => DispatchPropertyChanged("ArticlesDocumentNumberCurrentValue");
            ArticlesWarehouseCode.OnChange += (newValue) => DispatchPropertyChanged("ArticlesWarehouseCode");
            ArticlesWarehouseName.OnChange += (newValue) => DispatchPropertyChanged("ArticlesWarehouseName");
            СommissionPercent.OnChange += (newValue) => DispatchPropertyChanged("СommissionPercent");
            GarbageCollectionFilialCode.OnChange += (newValue) => DispatchPropertyChanged("GarbageCollectionFilialCode");

            CashierName.Value = Settings.CasherName;
            ArticlesDocumentNumberCurrentValue.Value = Settings.ArticlesDocumentNumberCurrentValue;
            ArticlesWarehouseCode.Value = Settings.ArticlesWarehouseCode;
            ArticlesWarehouseName.Value = Settings.ArticlesWarehouseName;
            СommissionPercent.Value = Settings.WaterСommissionPercent;
            GarbageCollectionFilialCode.Value = Settings.GarbageCollectionFilialCode;

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

            if (СommissionPercent.Value < 0 || СommissionPercent.Value > 100)
            {
                Message.Error("% комиссии должен быть от 0 до 100");
                return;
            }

            Settings.WaterСommissionPercent = СommissionPercent.Value;

            if (GarbageCollectionFilialCode.Value <= 0)
            {
                Message.Error("Код филиала для приема оплаты за вывоз ТКО должен быть положительным числом");
                return;
            }

            Settings.GarbageCollectionFilialCode = GarbageCollectionFilialCode.Value;

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
