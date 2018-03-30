using System;
using System.Linq;
using System.Windows;
using System.Collections.Generic;
using CashCenter.Check;
using CashCenter.Common;
using CashCenter.Common.Exceptions;
using CashCenter.ViewCommon;

namespace CashCenter.Objective.GarbageAndRepair
{
    public class GarbageAndRepairPaymentControlViewModel : ViewModel
    {
        private readonly List<GarbageAndRepairPaymentContext> contexts = new List<GarbageAndRepairPaymentContext>
        {
            new GarbagePaymentContext(),
            new RepairPaymentContext()
        };

        private readonly List<string> updateablePropsNames = new List<string>
        {
            "PaymentName", "CustomerNumber", "RegionCode", "FinancialPeriodCode", "OrganizationCode", "FilialCode", "CommissionPercent"
        };

        private Observed<GarbageAndRepairPaymentContext> currentContext { get; } = new Observed<GarbageAndRepairPaymentContext>();

        public ViewProperty<string> BarcodeString { get; }
        public ViewProperty<decimal> OverridedCost { get; }
        public ViewProperty<decimal> TotalCost { get; }

        public string PaymentName => currentContext.Value?.PaymentName ?? string.Empty;
        public int CustomerNumber => currentContext.Value?.Barcode?.CustomerNumber ?? 0;
        public int RegionCode => currentContext.Value?.Barcode?.RegionCode ?? 0;
        public int FinancialPeriodCode => currentContext.Value?.Barcode?.FinancialPeriod ?? 0;
        public int OrganizationCode => currentContext.Value?.OrganizationCode ?? 0;
        public int FilialCode => currentContext.Value?.FilialCode ?? 0;
        public float CommissionPercent => currentContext.Value?.CommissionPercent ?? 0;
            
        public ViewProperty<bool> IsPaymentEnable { get; }
        public ViewProperty<bool> IsBarcodeFocused { get; }

        public Command PayCommand { get; }
        public Command ClearCommand { get; }
        public Command ApplyBarcodeCommand { get; }

        public GarbageAndRepairPaymentControlViewModel()
        {
            currentContext.OnChange += (newValue) => UpdateAllProperties();

            BarcodeString = new ViewProperty<string>("BarcodeString", this);
            OverridedCost = new ViewProperty<decimal>("OverridedCost", this);
            OverridedCost.OnChange += (newValue) => TotalCost.Value = currentContext.Value?.GetCostWithComission(newValue) ?? 0;
            TotalCost = new ViewProperty<decimal>("TotalCost", this);
            IsPaymentEnable = new ViewProperty<bool>("IsPaymentEnable", this);
            IsBarcodeFocused = new ViewProperty<bool>("IsBarcodeFocused", this);

            PayCommand = new Command(PayHandler);
            ClearCommand = new Command(ClearHandler);
            ApplyBarcodeCommand = new Command(ApplyBarCodeHandler);

            ClearHandler(null);
        }

        private void ApplyBarCodeHandler(object parameters)
        {
            try
            {
                var barcode = Barcode.Parse(BarcodeString.Value);
                currentContext.Value = contexts.FirstOrDefault(item => item.OrganizationCode == barcode.OrganizationCode);

                if (currentContext.Value == null)
                {
                    string possibleOrganizationCodeValues = string.Join("; ", contexts.Select(item => $"{item.OrganizationCode} ({item.PaymentName})"));
                    Message.Error($"Код организации не корректен ({barcode.OrganizationCode}). Возможные значения: {possibleOrganizationCodeValues}");
                    Log.Error($"Код организации не корректен ({barcode.OrganizationCode}).");
                    return;
                }

                currentContext.Value.ApplyBarcode(barcode);
                UpdateAllProperties();
                IsPaymentEnable.Value = true;
            }
            catch (Exception ex)
            {
                Message.Error(ex.Message);
                Log.Error(ex.Message);
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

            if (currentContext.Value == null)
            {
                Message.Error("Штрих-код не отсканирован");
                Log.Error("currentContext == null");
                return;
            }

            var operationName = $"Оплата за {currentContext.Value.PaymentName}";
            try
            {
                Log.Info($"Старт -> {operationName}");

                using (new OperationWaiter())
                {
                    currentContext.Value.Pay(OverridedCost.Value, isWithoutCheck);
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
                Message.Error($"Ошибка выполнения операции \"{operationName}\"");
                Log.Error($"Ошибка -> {operationName}", ex);
            }
        }

        private void ClearHandler(object parameters)
        {
            currentContext.Value = null;

            BarcodeString.Value = string.Empty;
            OverridedCost.Value = 0;

            IsPaymentEnable.Value = false;

            // Force barcode focus
            IsBarcodeFocused.Value = false;
            IsBarcodeFocused.Value = true;
        }

        private void UpdateAllProperties()
        {
            foreach (var propName in updateablePropsNames)
            {
                DispatchPropertyChanged(propName);
            }

            OverridedCost.Value = currentContext.Value?.Cost ?? 0;
        }
    }
}
