using System;
using System.Windows;
using CashCenter.Check;
using CashCenter.Common;
using CashCenter.Common.Exceptions;
using CashCenter.IvEnergySales.BusinessLogic;
using CashCenter.IvEnergySales.Common;

namespace CashCenter.IvEnergySales.BusinessLogicControls
{
    public class GarbageAndRepairPaymentControlViewModel : ViewModel
    {
        private GarbageAndRepairPaymentContext context = new GarbageAndRepairPaymentContext();

        public Observed<string> Barcode { get; } = new Observed<string>();
        public Observed<decimal> OverridedCost { get; } = new Observed<decimal>();
        public Observed<decimal> TotalCost { get; } = new Observed<decimal>();

        public int CustomerNumber => context.CustomerNumber.Value;
        public int RegionCode => context.RegionCode.Value;
        public int FinancialPeriodCode => context.FinancialPeriodCode.Value;
        public int OrganizationCode => context.OrganizationCode.Value;
        public int FilialCode => context.FilialCode.Value;
        public float CommissionPercent => context.CommissionPercent.Value;
            
        public Observed<bool> IsPaymentEnable { get; } = new Observed<bool>();
        public Observed<bool> IsBarcodeFocused { get; } = new Observed<bool>();

        public Command PayCommand { get; }
        public Command ClearCommand { get; }
        public Command ApplyBarcodeCommand { get; }

        public GarbageAndRepairPaymentControlViewModel()
        {
            Barcode.OnChange += (newValue) => DispatchPropertyChanged("Barcode");
            context.CustomerNumber.OnChange += (newValue) => DispatchPropertyChanged("CustomerNumber");
            context.RegionCode.OnChange += (newValue) => DispatchPropertyChanged("RegionCode");
            context.FinancialPeriodCode.OnChange += (newValue) => DispatchPropertyChanged("FinancialPeriodCode");
            context.OrganizationCode.OnChange += (newValue) => DispatchPropertyChanged("OrganizationCode");
            context.FilialCode.OnChange += (newValue) => DispatchPropertyChanged("FilialCode");
            context.CommissionPercent.OnChange += (newValue) => DispatchPropertyChanged("CommissionPercent");
            context.Cost.OnChange += (newValue) =>
            {
                OverridedCost.Value = newValue;
                DispatchPropertyChanged("OverridedCost");
            };

            OverridedCost.OnChange += (newValue) =>
            {
                DispatchPropertyChanged("OverridedCost");
                TotalCost.Value = context.GetCostWithComission(newValue);
            };
            
            TotalCost.OnChange += (newValue) => DispatchPropertyChanged("TotalCost");

            IsPaymentEnable.OnChange += (newValue) => DispatchPropertyChanged("IsPaymentEnable");
            IsBarcodeFocused.OnChange += (newValue) => DispatchPropertyChanged("IsBarcodeFocused");

            PayCommand = new Command(PayHandler);
            ClearCommand = new Command(ClearHandler);
            ApplyBarcodeCommand = new Command(ApplyBarCodeHandler);

            ClearHandler(null);
        }

        private void ApplyBarCodeHandler(object parameters)
        {
            try
            {
                context.ApplyBarcode(Barcode.Value);
                IsPaymentEnable.Value = true;
            }
            catch (Exception ex)
            {
                Message.Error(ex.Message);
                Log.Error("Ошибка применения штрих-кода. " + ex.Message);
            }
        }

        private void PayHandler(object parameters)
        {
            var controlForValidate = parameters as DependencyObject;
            if (!IsValid(controlForValidate))
            {
                Message.Error("При вводе были допущены ошибки. Исправьте их и попробуйте снова.\nОшибочные поля обведены красным.");
                return;
            }

            var isWithoutCheck = false;
            if (!CheckPrinter.IsReady)
            {
                if (!Message.YesNoQuestion("Кассовый аппарат не подключен. Продолжить без печати чека?"))
                    return;

                isWithoutCheck = true;
            }

            var operationName = "Оплата за вывоз ТКО";
            try
            {
                Log.Info($"Старт -> {operationName}");

                using (new OperationWaiter())
                {
                    context.Pay(OverridedCost.Value, isWithoutCheck);
                    ClearHandler(null);
                }

                Log.Info($"Успешно -> {operationName}");
            }
            catch (IncorrectDataException ex)
            {
                Message.Error(ex.Message);
            }
            catch (Exception ex)
            {
                Message.Error($"Ошибка выполнения операция оплаты за воду");
                Log.Error($"Ошибка -> {operationName}", ex);
            }
        }

        private void ClearHandler(object parameters)
        {
            context.Clear();

            Barcode.Value = string.Empty;
            OverridedCost.Value = 0;

            IsPaymentEnable.Value = false;

            // Force barcode focus
            IsBarcodeFocused.Value = false;
            IsBarcodeFocused.Value = true;
        }
    }
}
