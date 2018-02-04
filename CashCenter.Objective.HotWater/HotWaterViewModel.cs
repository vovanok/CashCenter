using System;
using System.Windows;
using CashCenter.Common;
using CashCenter.Check;
using CashCenter.Common.Exceptions;

namespace CashCenter.Objective.HotWater
{
    public class HotWaterViewModel : ViewModel
    {
        //private WaterCustomerSalesContext context = new WaterCustomerSalesContext();

        public Observed<bool> IsCustomerNumberFocused { get; } = new Observed<bool>();
        public Observed<bool> IsEmailFocused { get; } = new Observed<bool>();
        public Observed<uint> CustomerNumber { get; } = new Observed<uint>();
        public Observed<string> CustomerName { get; } = new Observed<string>();
        public Observed<string> CustomerAddress { get; } = new Observed<string>();
        public Observed<string> CustomerEmail { get; } = new Observed<string>();

        public Observed<string> Counter1Number { get; } = new Observed<string>();
        public Observed<string> Counter2Number { get; } = new Observed<string>();
        public Observed<string> Counter3Number { get; } = new Observed<string>();
        public Observed<string> Counter4Number { get; } = new Observed<string>();

        public Observed<double> Counter1Value { get; } = new Observed<double>();
        public Observed<double> Counter2Value { get; } = new Observed<double>();
        public Observed<double> Counter3Value { get; } = new Observed<double>();
        public Observed<double> Counter4Value { get; } = new Observed<double>();

        public Observed<decimal> Penalty { get; } = new Observed<decimal>();
        public Observed<decimal> Cost { get; } = new Observed<decimal>();
        public Observed<decimal> TotalCost { get; } = new Observed<decimal>();
        public Observed<string> Description { get; } = new Observed<string>();
        public Observed<bool> IsPaymentEnable { get; } = new Observed<bool>();

        public float СommissionPercent
        {
            get { return Settings.WaterСommissionPercent; }
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
            CustomerEmail.OnChange += (newValue) => DispatchPropertyChanged("CustomerEmail");

            Counter1Number.OnChange += (newValue) => DispatchPropertyChanged("Counter1Number");
            Counter2Number.OnChange += (newValue) => DispatchPropertyChanged("Counter2Number");
            Counter3Number.OnChange += (newValue) => DispatchPropertyChanged("Counter3Number");
            Counter4Number.OnChange += (newValue) => DispatchPropertyChanged("Counter4Number");

            Counter1Value.OnChange += (newValue) => DispatchPropertyChanged("Counter1Value");
            Counter2Value.OnChange += (newValue) => DispatchPropertyChanged("Counter2Value");
            Counter3Value.OnChange += (newValue) => DispatchPropertyChanged("Counter3Value");
            Counter4Value.OnChange += (newValue) => DispatchPropertyChanged("Counter4Value");

            Penalty.OnChange += (newValue) =>
            {
                DispatchPropertyChanged("Penalty");
                UpdateTotalCost();
            };

            Cost.OnChange += (newValue) =>
            {
                DispatchPropertyChanged("Cost");
                UpdateTotalCost();
            };

            TotalCost.OnChange += (newValue) => DispatchPropertyChanged("TotalCost");
            Description.OnChange += (newValue) => DispatchPropertyChanged("Description");
            IsPaymentEnable.OnChange += (newValue) => DispatchPropertyChanged("IsPaymentEnable");

            GlobalEvents.OnWaterComissionPercentChanged += () =>
            {
                DispatchPropertyChanged("СommissionPercent");
                UpdateTotalCost();
            };

            FindCustomerCommand = new Command(FindCustomerHandler);
            PayCommand = new Command(PayHandler);
            ClearCommand = new Command(ClearHandler);
            
            //context.OnCustomerChanged += WaterPaymentContextCustomerChanged;
            //context.ClearCustomer();
        }

        private void UpdateTotalCost()
        {
            //TotalCost.Value = context.GetCostWithComission(Cost.Value + Penalty.Value);
        }

        //private void WaterPaymentContextCustomerChanged(WaterCustomer customer)
        //{
        //    CustomerNumber.Value = customer != null ? (uint)customer.Number : 0;
        //    IsCustomerNumberFocused.Value = customer == null;

        //    CustomerName.Value = customer?.Name ?? string.Empty;
        //    CustomerAddress.Value = customer?.Address ?? string.Empty;
        //    CustomerEmail.Value = customer?.Email ?? string.Empty;

        //    Counter1Number.Value = customer?.CounterNumber1 ?? string.Empty;
        //    Counter2Number.Value = customer?.CounterNumber2 ?? string.Empty;
        //    Counter3Number.Value = customer?.CounterNumber3 ?? string.Empty;
        //    Counter4Number.Value = customer?.CounterNumber4 ?? string.Empty;

        //    Counter1Value.Value = 0;
        //    Counter2Value.Value = 0;
        //    Counter3Value.Value = 0;
        //    Counter4Value.Value = 0;

        //    Cost.Value = 0;
        //    Penalty.Value = 0;

        //    Description.Value = string.Empty;

        //    IsPaymentEnable.Value = customer != null;
        //}

        private void FindCustomerHandler(object parameters)
        {
            //var targetCustomerNumber = CustomerNumber.Value;
            //using (new OperationWaiter())
            //{
            //    context.FindAndApplyCustomer(targetCustomerNumber);
            //}

            //if (context.Customer.Value == null)
            //{
            //    Message.Info($"Плательщик с номером лицевого счета {targetCustomerNumber} не найден");

            //    // Force customer number focus
            //    IsCustomerNumberFocused.Value = false;
            //    IsCustomerNumberFocused.Value = true;
            //}
            //else
            //{
            //    // Force email focus
            //    IsEmailFocused.Value = false;
            //    IsEmailFocused.Value = true;
            //}
        }

        private void PayHandler(object parameters)
        {
            //var controlForValidate = parameters as DependencyObject;
            //if (!IsValid(controlForValidate))
            //{
            //    Message.Error("При вводе были допущены ошибки. Исправьте их и попробуйте снова.\nОшибочные поля обведены красным.");
            //    return;
            //}

            //var isWithoutCheck = false;
            //if (!CheckPrinter.IsReady)
            //{
            //    if (!Message.YesNoQuestion("Кассовый аппарат не подключен. Продолжить без печати чека?"))
            //        return;

            //    isWithoutCheck = true;
            //}

            //var operationName = "Оплата за воду";
            //try
            //{
            //    Log.Info($"Старт -> {operationName}");

            //    using (new OperationWaiter())
            //    {
            //        context.Pay(CustomerEmail.Value, Counter1Value.Value, Counter2Value.Value, Counter3Value.Value,
            //            Counter4Value.Value, Penalty.Value, Cost.Value, Description.Value, isWithoutCheck);
            //        context.ClearCustomer();
            //    }

            //    Log.Info($"Успешно -> {operationName}");
            //}
            //catch (IncorrectDataException ex)
            //{
            //    Message.Error(ex.Message);
            //}
            //catch (Exception ex)
            //{
            //    Message.Error($"Ошибка выполнения операция оплаты за воду");
            //    Log.Error($"Ошибка -> {operationName}", ex);
            //}
        }

        private void ClearHandler(object parameters)
        {
            //context.ClearCustomer();
        }
    }
}
