using System;
using System.Linq;
using System.Windows;
using System.Collections.Generic;
using CashCenter.Dal;
using CashCenter.Check;
using CashCenter.Common;
using CashCenter.Common.Exceptions;
using CashCenter.ViewCommon;

namespace CashCenter.Objective.Energy
{
    public class CustomerEnergyPaymentControlViewModel : ViewModel
    {
        private EnergyCustomerSalesContext context = new EnergyCustomerSalesContext();

        public ViewProperty<Department> SelectedDepartment { get; }
        public ViewProperty<uint> CustomerNumber { get; }
        public ViewProperty<string> CustomerName { get; }
        public ViewProperty<string> CustomerAddress { get; }
        public ViewProperty<bool?> CustomerIsClosed { get; }
        public ViewProperty<string> CustomerEmail { get; }
        public ViewProperty<uint> PreviousDayValue { get; }
        public ViewProperty<uint> PreviousNightValue { get; }
        public ViewProperty<uint> CurrentDayValue { get; }
        public ViewProperty<uint> CurrentNightValue { get; }
        public ViewProperty<int> DeltaDayValue { get; }
        public ViewProperty<int> DeltaNightValue { get; }
        public ViewProperty<PaymentReason> SelectedPaymentReason { get; }
        public ViewProperty<decimal> Cost { get; }
        public ViewProperty<string> Description { get; }

        public ViewProperty<bool> IsCustomerNumberFocused { get; }
        public ViewProperty<bool> IsEmailFocused { get; }
        public ViewProperty<bool> IsPaymentEnable { get; }
        public ViewProperty<bool> IsNormative { get; }
        public ViewProperty<bool> IsDayValueActive { get; }
        public ViewProperty<bool> IsNightValueActive { get; }

        public List<PaymentReason> PaymentReasons { get; } = RepositoriesFactory.Get<PaymentReason>().GetAll().ToList();

        public Command FindCustomerCommand { get; }
        public Command PayCommand { get; }
        public Command ClearCommand { get; }

        public CustomerEnergyPaymentControlViewModel()
        {
            SelectedDepartment = new ViewProperty<Department>("SelectedDepartment", this);
            SelectedDepartment.OnChange += (newValue) =>
            {
                // Force customer number focus
                IsCustomerNumberFocused.Value = false;
                IsCustomerNumberFocused.Value = true;
            };
            
            CustomerNumber = new ViewProperty<uint>("CustomerNumber", this);
            CustomerName = new ViewProperty<string>("CustomerName", this);
            CustomerAddress = new ViewProperty<string>("CustomerAddress", this);
            CustomerIsClosed = new ViewProperty<bool?>("CustomerIsClosed", this);
            CustomerEmail = new ViewProperty<string>("CustomerEmail", this);
            PreviousDayValue = new ViewProperty<uint>("PreviousDayValue", this);
            PreviousNightValue = new ViewProperty<uint>("PreviousNightValue", this);
            DeltaDayValue = new ViewProperty<int>("DeltaDayValue", this);
            DeltaNightValue = new ViewProperty<int>("DeltaNightValue", this);
            SelectedPaymentReason = new ViewProperty<PaymentReason>("SelectedPaymentReason", this);
            Cost = new ViewProperty<decimal>("Cost", this);
            Description = new ViewProperty<string>("Description", this);

            CurrentDayValue = new ViewProperty<uint>("CurrentDayValue", this);
            CurrentDayValue.OnChange += (newValue) => UpdateDayDeltaValue();

            CurrentNightValue = new ViewProperty<uint>("CurrentNightValue", this);
            CurrentNightValue.OnChange += (newValue) => UpdateNightDeltaValue();

            IsCustomerNumberFocused = new ViewProperty<bool>("IsCustomerNumberFocused", this);
            IsEmailFocused = new ViewProperty<bool>("IsEmailFocused", this);
            IsPaymentEnable = new ViewProperty<bool>("IsPaymentEnable", this);
            IsNormative = new ViewProperty<bool>("IsNormative", this);
            IsDayValueActive = new ViewProperty<bool>("IsDayValueActive", this);
            IsNightValueActive = new ViewProperty<bool>("IsNightValueActive", this);

            FindCustomerCommand = new Command(FindCustomerHandler);
            PayCommand = new Command(PayHandler);
            ClearCommand = new Command(ClearHandler);

            context.OnCustomerChanged += EnergyPaymentContextCustomerChanged;
            context.ClearCustomer();
        }

        private void EnergyPaymentContextCustomerChanged(EnergyCustomer customer)
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
