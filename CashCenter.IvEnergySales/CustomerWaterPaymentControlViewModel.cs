using System.Windows;
using CashCenter.Common;
using CashCenter.Dal;
using CashCenter.IvEnergySales.BusinessLogic;
using CashCenter.IvEnergySales.Exceptions;

namespace CashCenter.IvEnergySales
{
    public class CustomerWaterPaymentControlViewModel : ViewModel
    {
        private WaterCustomerPaymentContext context = new WaterCustomerPaymentContext();

        public Observed<bool> IsCustomerNumberFocused { get; } = new Observed<bool>();
        public Observed<bool> IsEmailFocused { get; } = new Observed<bool>();
        public Observed<uint> CustomerNumber { get; } = new Observed<uint>();
        public Observed<string> CustomerName { get; } = new Observed<string>();
        public Observed<string> CustomerAddress { get; } = new Observed<string>();
        public Observed<string> Email { get; } = new Observed<string>();

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
        public Observed<string> Description { get; } = new Observed<string>();
        public Observed<bool> IsPaymentEnable { get; } = new Observed<bool>();

        public Command FindCustomerCommand { get; }
        public Command PayCommand { get; }
        public Command ClearCommand { get; }

        public CustomerWaterPaymentControlViewModel()
        {
            IsCustomerNumberFocused.OnChange += (newValue) => DispatchPropertyChanged("IsCustomerNumberFocused");
            IsEmailFocused.OnChange += (newValue) => DispatchPropertyChanged("IsEmailFocused");
            CustomerNumber.OnChange += (newValue) => DispatchPropertyChanged("CustomerNumber");
            CustomerName.OnChange += (newValue) => DispatchPropertyChanged("CustomerName");
            CustomerAddress.OnChange += (newValue) => DispatchPropertyChanged("CustomerAddress");
            Email.OnChange += (newValue) => DispatchPropertyChanged("Email");

            Counter1Number.OnChange += (newValue) => DispatchPropertyChanged("Counter1Number");
            Counter2Number.OnChange += (newValue) => DispatchPropertyChanged("Counter2Number");
            Counter3Number.OnChange += (newValue) => DispatchPropertyChanged("Counter3Number");
            Counter4Number.OnChange += (newValue) => DispatchPropertyChanged("Counter4Number");

            Counter1Value.OnChange += (newValue) => DispatchPropertyChanged("Counter1Value");
            Counter2Value.OnChange += (newValue) => DispatchPropertyChanged("Counter2Value");
            Counter3Value.OnChange += (newValue) => DispatchPropertyChanged("Counter3Value");
            Counter4Value.OnChange += (newValue) => DispatchPropertyChanged("Counter4Value");

            Penalty.OnChange += (newValue) => DispatchPropertyChanged("Penalty");
            Cost.OnChange += (newValue) => DispatchPropertyChanged("Cost");
            Description.OnChange += (newValue) => DispatchPropertyChanged("Description");
            IsPaymentEnable.OnChange += (newValue) => DispatchPropertyChanged("IsPaymentEnable");

            FindCustomerCommand = new Command(FindCustomerHandler);
            PayCommand = new Command(PayHandler);
            ClearCommand = new Command(ClearHandler);

            context.OnCustomerChanged += WaterPaymentcontextCustomerChanged;
        }

        private void WaterPaymentcontextCustomerChanged(WaterCustomer customer)
        {
            CustomerNumber.Value = customer != null ? (uint)customer.Number : 0;
            IsCustomerNumberFocused.Value = customer == null;

            CustomerName.Value = customer?.Name ?? string.Empty;
            CustomerAddress.Value = customer?.Address ?? string.Empty;
            Email.Value = customer?.Email ?? string.Empty;

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
                MessageBox.Show($"Плательщик с номером лицевого счета {targetCustomerNumber} не найден.");

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
            try
            {
                using (new OperationWaiter())
                {
                    context.Pay(
                        Email.Value, Counter1Value.Value, Counter2Value.Value, Counter3Value.Value,
                        Counter4Value.Value, Penalty.Value, Cost.Value, Description.Value, 0); // TODO: Fiscal number
                    context.ClearCustomer();
                }
            }
            catch (UserException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearHandler(object parameters)
        {
            context.ClearCustomer();
        }
    }
}
