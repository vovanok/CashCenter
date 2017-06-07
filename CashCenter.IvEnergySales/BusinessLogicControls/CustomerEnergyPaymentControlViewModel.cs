using System;
using System.Linq;
using System.Windows;
using System.Collections.Generic;
using CashCenter.Dal;
using CashCenter.Check;
using CashCenter.Common;
using CashCenter.IvEnergySales.Common;
using CashCenter.IvEnergySales.BusinessLogic;
using CashCenter.Common.Exceptions;

namespace CashCenter.IvEnergySales
{
    public class CustomerEnergyPaymentControlViewModel : ViewModel
    {
        private EnergyCustomerSalesContext context = new EnergyCustomerSalesContext();

        public Observed<Department> SelectedDepartment { get; } = new Observed<Department>();
        public Observed<uint> CustomerNumber { get; } = new Observed<uint>();
        public Observed<string> CustomerName { get; } = new Observed<string>();
        public Observed<string> CustomerAddress { get; } = new Observed<string>();
        public Observed<bool?> CustomerIsClosed { get; } = new Observed<bool?>();
        public Observed<string> CustomerEmail { get; } = new Observed<string>();
        public Observed<uint> PreviousDayValue { get; } = new Observed<uint>();
        public Observed<uint> PreviousNightValue { get; } = new Observed<uint>();
        public Observed<uint> CurrentDayValue { get; } = new Observed<uint>();
        public Observed<uint> CurrentNightValue { get; } = new Observed<uint>();
        public Observed<int> DeltaDayValue { get; } = new Observed<int>();
        public Observed<int> DeltaNightValue { get; } = new Observed<int>();
        public Observed<PaymentReason> SelectedPaymentReason { get; } = new Observed<PaymentReason>();
        public Observed<decimal> Cost { get; } = new Observed<decimal>();
        public Observed<string> Description { get; } = new Observed<string>();

        public Observed<bool> IsCustomerNumberFocused { get; } = new Observed<bool>();
        public Observed<bool> IsEmailFocused { get; } = new Observed<bool>();
        public Observed<bool> IsPaymentEnable { get; } = new Observed<bool>();
        public Observed<bool> IsNormative { get; } = new Observed<bool>();
        public Observed<bool> IsDayValueActive { get; } = new Observed<bool>();
        public Observed<bool> IsNightValueActive { get; } = new Observed<bool>();

        public List<PaymentReason> PaymentReasons { get; } = DalController.Instance.PaymentReasons.ToList();

        public Command FindCustomerCommand { get; }
        public Command PayCommand { get; }
        public Command ClearCommand { get; }

        public CustomerEnergyPaymentControlViewModel()
        {
            SelectedDepartment.OnChange += (newValue) => DispatchPropertyChanged("SelectedDepartment");
            CustomerNumber.OnChange += (newValue) => DispatchPropertyChanged("CustomerNumber");
            CustomerName.OnChange += (newValue) => DispatchPropertyChanged("CustomerName");
            CustomerAddress.OnChange += (newValue) => DispatchPropertyChanged("CustomerAddress");
            CustomerIsClosed.OnChange += (newValue) => DispatchPropertyChanged("CustomerIsClosed");
            CustomerEmail.OnChange += (newValue) => DispatchPropertyChanged("CustomerEmail");
            PreviousDayValue.OnChange += (newValue) => DispatchPropertyChanged("PreviousDayValue"); ;
            PreviousNightValue.OnChange += (newValue) => DispatchPropertyChanged("PreviousNightValue");
            DeltaDayValue.OnChange += (newValue) => DispatchPropertyChanged("DeltaDayValue");
            DeltaNightValue.OnChange += (newValue) => DispatchPropertyChanged("DeltaNightValue");
            SelectedPaymentReason.OnChange += (newValue) => DispatchPropertyChanged("SelectedPaymentReason");
            Cost.OnChange += (newValue) => DispatchPropertyChanged("Cost");
            Description.OnChange += (newValue) => DispatchPropertyChanged("Description");

            CurrentDayValue.OnChange += (newValue) =>
            {
                DispatchPropertyChanged("CurrentDayValue");
                UpdateDayDeltaValue();
            };

            CurrentNightValue.OnChange += (newValue) =>
            {
                DispatchPropertyChanged("CurrentNightValue");
                UpdateNightDeltaValue();
            };

            IsCustomerNumberFocused.OnChange += (newValue) => DispatchPropertyChanged("IsCustomerNumberFocused");
            IsEmailFocused.OnChange += (newValue) => DispatchPropertyChanged("IsEmailFocused");
            IsPaymentEnable.OnChange += (newValue) => DispatchPropertyChanged("IsPaymentEnable");
            IsNormative.OnChange += (newValue) => DispatchPropertyChanged("IsNormative");
            IsDayValueActive.OnChange += (newValue) => DispatchPropertyChanged("IsDayValueActive");
            IsNightValueActive.OnChange += (newValue) => DispatchPropertyChanged("IsNightValueActive");

            FindCustomerCommand = new Command(FindCustomerHandler);
            PayCommand = new Command(PayHandler);
            ClearCommand = new Command(ClearHandler);

            context.OnCustomerChanged += EnergyPaymentContextCustomerChanged;
            context.ClearCustomer();
        }

        private void EnergyPaymentContextCustomerChanged(Customer customer)
        {
            CustomerNumber.Value = customer != null ? (uint)customer.Number : 0;
            IsCustomerNumberFocused.Value = customer == null;

            CustomerName.Value = customer?.Name ?? string.Empty;
            CustomerAddress.Value = customer?.Address ?? string.Empty;
            CustomerIsClosed.Value = customer != null ? (bool?)customer.IsClosed : null;
            CustomerEmail.Value = customer?.Email ?? string.Empty;

            PreviousDayValue.Value = customer != null ? (uint)customer.DayValue : 0;
            PreviousNightValue.Value = customer != null ? (uint)customer.NightValue : 0;
            CurrentDayValue.Value = PreviousDayValue.Value;
            CurrentNightValue.Value = PreviousNightValue.Value;
            UpdateDayDeltaValue();
            UpdateNightDeltaValue();

            SelectedPaymentReason.Value = PaymentReasons.FirstOrDefault();

            Cost.Value = 0;
            Description.Value = string.Empty;

            IsNormative.Value = customer != null ? customer.IsNormative() : false;
            IsPaymentEnable.Value = customer != null;

            IsDayValueActive.Value = !IsNormative.Value && IsPaymentEnable.Value;
            IsNightValueActive.Value = (customer?.IsTwoTariff() ?? false) && IsPaymentEnable.Value;
        }

        private void FindCustomerHandler(object parameters)
        {
            var targetCustomerNumber = CustomerNumber.Value;
            using (new OperationWaiter())
            {
                try
                {
                    context.FindAndApplyCustomer(targetCustomerNumber, SelectedDepartment.Value);
                }
                catch (Exception ex)
                {
                    Log.Error("Ошибка поиска плательщика за электроэнергию", ex);
                    Message.Error($"Ошибка поиска плательщика за электроэнергию. {ex.Message}");
                }
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

            var operationName = "Оплата за электроэнергию";
            try
            {
                Log.Info($"Старт -> {operationName}");

                using (new OperationWaiter())
                {

                    context.Pay(CustomerEmail.Value, (int)CurrentDayValue.Value, (int)CurrentNightValue.Value,
                        Cost.Value, SelectedPaymentReason.Value, Description.Value, isWithoutCheck);

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
                Message.Error($"Ошибка выполнения операция оплаты за электроэнергию");
                Log.Error($"Ошибка -> {operationName}", ex);
            }
        }

        private void ClearHandler(object parameters)
        {
            context.ClearCustomer();
        }

        private void UpdateDayDeltaValue()
        {
            DeltaDayValue.Value = (int)CurrentDayValue.Value - (int)PreviousDayValue.Value;
        }

        private void UpdateNightDeltaValue()
        {
            DeltaNightValue.Value = (int)CurrentNightValue.Value - (int)PreviousNightValue.Value;
        }
    }
}
