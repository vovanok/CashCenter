using System;
using System.Windows;
using CashCenter.Dal;
using CashCenter.Common;
using CashCenter.Common.Exceptions;
using CashCenter.ViewCommon;
using CashCenter.Check;

namespace CashCenter.Objective.Water
{
    public class CustomerWaterPaymentControlViewModel : ViewModel
    {
        private WaterCustomerSalesContext context = new WaterCustomerSalesContext();

        public ViewProperty<bool> IsCustomerNumberFocused { get; }
        public ViewProperty<bool> IsEmailFocused { get; }
        public ViewProperty<uint> CustomerNumber { get; }
        public ViewProperty<string> CustomerName { get; }
        public ViewProperty<string> CustomerAddress { get; }
        public ViewProperty<string> CustomerEmail { get; }

        public ViewProperty<string> Counter1Number { get; }
        public ViewProperty<string> Counter2Number { get; }
        public ViewProperty<string> Counter3Number { get; }
        public ViewProperty<string> Counter4Number { get; }

        public ViewProperty<double> Counter1Value { get; }
        public ViewProperty<double> Counter2Value { get; }
        public ViewProperty<double> Counter3Value { get; }
        public ViewProperty<double> Counter4Value { get; }

        public ViewProperty<decimal> Penalty { get; }
        public ViewProperty<decimal> Cost { get; }
        public ViewProperty<decimal> TotalCost { get; }
        public ViewProperty<string> Description { get; }
        public ViewProperty<bool> IsPaymentEnable { get; }

        public float СommissionPercent
        {
            get { return Settings.WaterСommissionPercent; }
        }
            
        public Command FindCustomerCommand { get; }
        public Command PayCommand { get; }
        public Command ClearCommand { get; }

        public CustomerWaterPaymentControlViewModel()
        {
            IsCustomerNumberFocused = new ViewProperty<bool>("IsCustomerNumberFocused", this);
            IsEmailFocused = new ViewProperty<bool>("IsEmailFocused", this);
            CustomerNumber = new ViewProperty<uint>("CustomerNumber", this);
            CustomerName = new ViewProperty<string>("CustomerName", this);
            CustomerAddress = new ViewProperty<string>("CustomerAddress", this);
            CustomerEmail = new ViewProperty<string>("CustomerEmail", this);

            Counter1Number = new ViewProperty<string>("Counter1Number", this);
            Counter2Number = new ViewProperty<string>("Counter2Number", this);
            Counter3Number = new ViewProperty<string>("Counter3Number", this);
            Counter4Number = new ViewProperty<string>("Counter4Number", this);

            Counter1Value = new ViewProperty<double>("Counter1Value", this);
            Counter2Value = new ViewProperty<double>("Counter2Value", this);
            Counter3Value = new ViewProperty<double>("Counter3Value", this);
            Counter4Value = new ViewProperty<double>("Counter4Value", this);

            Penalty = new ViewProperty<decimal>("Penalty", this);
            Penalty.OnChange += (newValue) => UpdateTotalCost();

            Cost = new ViewProperty<decimal>("Cost", this);
            Cost.OnChange += (newValue) => UpdateTotalCost();

            TotalCost = new ViewProperty<decimal>("TotalCost", this);
            Description = new ViewProperty<string>("Description", this);
            IsPaymentEnable = new ViewProperty<bool>("IsPaymentEnable", this);

            GlobalEvents.OnWaterComissionPercentChanged += () =>
            {
                DispatchPropertyChanged("СommissionPercent");
                UpdateTotalCost();
            };

            FindCustomerCommand = new Command(FindCustomerHandler);
            PayCommand = new Command(PayHandler);
            ClearCommand = new Command(ClearHandler);
            
            context.OnCustomerChanged += WaterPaymentContextCustomerChanged;
            context.ClearCustomer();
        }

        private void UpdateTotalCost()
        {
            TotalCost.Value = context.GetCostWithComission(Cost.Value + Penalty.Value);
        }

        private void WaterPaymentContextCustomerChanged(WaterCustomer customer)
        {
            CustomerNumber.Value = customer != null ? (uint)customer.Number : 0;
            IsCustomerNumberFocused.Value = customer == null;

            CustomerName.Value = customer?.Name ?? string.Empty;
            CustomerAddress.Value = customer?.Address ?? string.Empty;
            CustomerEmail.Value = customer?.Email ?? string.Empty;

            Counter1Number.Value = customer?.CounterNumber1 ?? string.Empty;
            Counter2Number.Value = customer?.CounterNumber2 ?? string.Empty;
            Counter3Number.Value = customer?.CounterNumber3 ?? string.Empty;
            Counter4Number.Value = customer?.CounterNumber4 ?? string.Empty;

            Counter1Value.Value = 0;
            Counter2Value.Value = 0;
            Counter3Value.Value = 0;
            Counter4Value.Value = 0;

            Cost.Value = 0;
            Penalty.Value = 0;

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

            var operationName = "Оплата за воду";
            try
            {
                Log.Info($"Старт -> {operationName}");

                using (new OperationWaiter())
                {
                    context.Pay(CustomerEmail.Value, Counter1Value.Value, Counter2Value.Value, Counter3Value.Value,
                        Counter4Value.Value, Penalty.Value, Cost.Value, Description.Value, isWithoutCheck);
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
                Message.Error($"Ошибка выполнения операция оплаты за воду");
                Log.Error($"Ошибка -> {operationName}", ex);
            }
        }

        private void ClearHandler(object parameters)
        {
            context.ClearCustomer();
        }
    }
}
