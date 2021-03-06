﻿using System;
using System.Windows;
using CashCenter.Dal;
using CashCenter.Check;
using CashCenter.Common;
using CashCenter.Common.Exceptions;
using CashCenter.ViewCommon;

namespace CashCenter.Objective.HotWater
{
    public class HotWaterViewModel : ViewModel
    {
        private HotWaterContext context = new HotWaterContext();

        public Observed<bool> IsCustomerNumberFocused { get; } = new Observed<bool>();
        public Observed<bool> IsEmailFocused { get; } = new Observed<bool>();
        public Observed<uint> CustomerNumber { get; } = new Observed<uint>();
        public Observed<string> CustomerName { get; } = new Observed<string>();
        public Observed<string> CustomerAddress { get; } = new Observed<string>();
        public Observed<string> CustomerEmail { get; } = new Observed<string>();

        public Observed<string> Counter1Name { get; } = new Observed<string>();
        public Observed<string> Counter2Name { get; } = new Observed<string>();
        public Observed<string> Counter3Name { get; } = new Observed<string>();
        public Observed<string> Counter4Name { get; } = new Observed<string>();
        public Observed<string> Counter5Name { get; } = new Observed<string>();

        public Observed<double> Counter1Value { get; } = new Observed<double>();
        public Observed<double> Counter2Value { get; } = new Observed<double>();
        public Observed<double> Counter3Value { get; } = new Observed<double>();
        public Observed<double> Counter4Value { get; } = new Observed<double>();
        public Observed<double> Counter5Value { get; } = new Observed<double>();

        public Observed<decimal> Total { get; } = new Observed<decimal>();
        public Observed<decimal> TotalWithCommission { get; } = new Observed<decimal>();
        public Observed<string> Description { get; } = new Observed<string>();
        public Observed<bool> IsPaymentEnable { get; } = new Observed<bool>();

        public float СommissionPercent
        {
            get { return Settings.HotWaterСommissionPercent; }
        }
            
        public Command FindCustomerCommand { get; }
        public Command PayCommand { get; }
        public Command ClearCommand { get; }

        public HotWaterViewModel()
        {
            IsCustomerNumberFocused.OnChange += (newValue) => DispatchPropertyChanged("IsCustomerNumberFocused");
            IsEmailFocused.OnChange += (newValue) => DispatchPropertyChanged("IsEmailFocused");
            CustomerNumber.OnChange += (newValue) => DispatchPropertyChanged("CustomerNumber");
            CustomerName.OnChange += (newValue) => DispatchPropertyChanged("CustomerName");
            CustomerAddress.OnChange += (newValue) => DispatchPropertyChanged("CustomerAddress");

            Counter1Name.OnChange += (newValue) => DispatchPropertyChanged("Counter1Name");
            Counter2Name.OnChange += (newValue) => DispatchPropertyChanged("Counter2Name");
            Counter3Name.OnChange += (newValue) => DispatchPropertyChanged("Counter3Name");
            Counter4Name.OnChange += (newValue) => DispatchPropertyChanged("Counter4Name");
            Counter5Name.OnChange += (newValue) => DispatchPropertyChanged("Counter5Name");

            Counter1Value.OnChange += (newValue) => DispatchPropertyChanged("Counter1Value");
            Counter2Value.OnChange += (newValue) => DispatchPropertyChanged("Counter2Value");
            Counter3Value.OnChange += (newValue) => DispatchPropertyChanged("Counter3Value");
            Counter4Value.OnChange += (newValue) => DispatchPropertyChanged("Counter4Value");
            Counter5Value.OnChange += (newValue) => DispatchPropertyChanged("Counter5Value");

            TotalWithCommission.OnChange += (newValue) =>
            {
                DispatchPropertyChanged("TotalWithCommission");
                UpdateTotal();
            };

            Total.OnChange += (newValue) => DispatchPropertyChanged("Total");
            Description.OnChange += (newValue) => DispatchPropertyChanged("Description");
            IsPaymentEnable.OnChange += (newValue) => DispatchPropertyChanged("IsPaymentEnable");

            GlobalEvents.OnHotWaterComissionPercentChanged += () =>
            {
                DispatchPropertyChanged("СommissionPercent");
                UpdateTotal();
            };

            FindCustomerCommand = new Command(FindCustomerHandler);
            PayCommand = new Command(PayHandler);
            ClearCommand = new Command(ClearHandler);

            context.OnCustomerChanged += HotWaterCustomerChanged;
            context.ClearCustomer();
        }

        private void UpdateTotal()
        {
            Total.Value = context.GetTotal(TotalWithCommission.Value);
        }

        private void HotWaterCustomerChanged(HotWaterCustomer customer)
        {
            CustomerNumber.Value = customer != null ? (uint)customer.Number : 0;
            IsCustomerNumberFocused.Value = customer == null;

            CustomerName.Value = customer?.Name ?? string.Empty;
            CustomerAddress.Value = customer?.Address ?? string.Empty;
            CustomerEmail.Value = customer?.Email ?? string.Empty;

            Counter1Name.Value = customer?.CounterName1 ?? string.Empty;
            Counter2Name.Value = customer?.CounterName2 ?? string.Empty;
            Counter3Name.Value = customer?.CounterName3 ?? string.Empty;
            Counter4Name.Value = customer?.CounterName4 ?? string.Empty;
            Counter5Name.Value = customer?.CounterName5 ?? string.Empty;

            Counter1Value.Value = customer?.CounterValue1 ?? 0;
            Counter2Value.Value = customer?.CounterValue2 ?? 0;
            Counter3Value.Value = customer?.CounterValue3 ?? 0;
            Counter4Value.Value = customer?.CounterValue4 ?? 0;
            Counter5Value.Value = customer?.CounterValue5 ?? 0;

            TotalWithCommission.Value = customer?.TotalForPay ?? 0;

            Description.Value = string.Empty;

            IsPaymentEnable.Value = customer != null;
        }

        private void FindCustomerHandler(object parameters)
        {
            var targetCustomerNumber = CustomerNumber.Value;
            using (new OperationWaiter())
            {
                context.FindAndApplyCustomer(targetCustomerNumber);
            }

            if (context.Customer.Value == null)
            {
                Message.Info($"Плательщик с номером лицевого счета {targetCustomerNumber} не найден");

                // Force customer number focus
                IsCustomerNumberFocused.Value = false;
                IsCustomerNumberFocused.Value = true;
            }
            else
            {
                // Force email focus
                IsEmailFocused.Value = false;
                IsEmailFocused.Value = true;
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

            var operationName = "Оплата за горячую воду";
            try
            {
                Log.Info($"Старт -> {operationName}");

                using (new OperationWaiter())
                {
                    context.Pay(
                        CustomerEmail.Value,
                        Counter1Value.Value, Counter2Value.Value, Counter3Value.Value,
                        Counter4Value.Value, Counter5Value.Value, TotalWithCommission.Value,
                        Description.Value, isWithoutCheck);
                    context.ClearCustomer();
                }

                Log.Info($"Успешно -> {operationName}");
            }
            catch (IncorrectDataException ex)
            {
                Message.Error(ex.Message);
            }
            catch (Exception ex)
            {
                Message.Error($"Ошибка выполнения операции '{operationName}'");
                Log.Error($"Ошибка -> {operationName}", ex);
            }
        }

        private void ClearHandler(object parameters)
        {
            context.ClearCustomer();
        }
    }
}
