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

        public Observed<uint> CustomerNumber { get; } = new Observed<uint>();
        public Observed<string> CustomerName { get; } = new Observed<string>();
        public Observed<string> CustomerAddress { get; } = new Observed<string>();
        public Observed<string> Email { get; } = new Observed<string>();
        public Observed<uint> Counter1Number { get; } = new Observed<uint>();
        public Observed<uint> Counter2Number { get; } = new Observed<uint>();
        public Observed<uint> Counter3Number { get; } = new Observed<uint>();
        public Observed<decimal> Counter1Cost { get; } = new Observed<decimal>();
        public Observed<decimal> Counter2Cost { get; } = new Observed<decimal>();
        public Observed<decimal> Counter3Cost { get; } = new Observed<decimal>();
        public Observed<decimal> TotalCost { get; } = new Observed<decimal>();
        public Observed<string> Description { get; } = new Observed<string>();
        public Observed<bool> IsPaymentEnable { get; } = new Observed<bool>();

        public Command FindCustomerCommand { get; }
        public Command PayCommand { get; }
        public Command ClearCommand { get; }

        public CustomerWaterPaymentControlViewModel()
        {
            CustomerNumber.OnChange += (newValue) => DispatchPropertyChanged("CustomerNumber");
            CustomerName.OnChange += (newValue) => DispatchPropertyChanged("CustomerName");
            CustomerAddress.OnChange += (newValue) => DispatchPropertyChanged("CustomerAddress");
            Email.OnChange += (newValue) => DispatchPropertyChanged("Email");
            Counter1Number.OnChange += (newValue) => DispatchPropertyChanged("Counter1Number");
            Counter2Number.OnChange += (newValue) => DispatchPropertyChanged("Counter2Number");
            Counter3Number.OnChange += (newValue) => DispatchPropertyChanged("Counter3Number");
            Counter1Cost.OnChange += (newValue) =>
            {
                DispatchPropertyChanged("Counter1Cost");
                UpdateTotalCost();
            };
            Counter2Cost.OnChange += (newValue) =>
            {
                DispatchPropertyChanged("Counter2Cost");
                UpdateTotalCost();
            };
            Counter3Cost.OnChange += (newValue) =>
            {
                DispatchPropertyChanged("Counter3Cost");
                UpdateTotalCost();
            };
            TotalCost.OnChange += (newValue) => DispatchPropertyChanged("TotalCost");
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

            CustomerName.Value = customer?.Name ?? string.Empty;
            CustomerAddress.Value = customer?.Address ?? string.Empty;
            Email.Value = customer?.Email ?? string.Empty;

            Counter1Number.Value = customer != null ? (uint)customer.CounterNumber1 : 0;
            Counter2Number.Value = customer != null ? (uint)customer.CounterNumber2 : 0;
            Counter3Number.Value = customer != null ? (uint)customer.CounterNumber3 : 0;

            Counter1Cost.Value = 0;
            Counter2Cost.Value = 0;
            Counter3Cost.Value = 0;

            Description.Value = string.Empty;

            IsPaymentEnable.Value = customer != null;
        }

        private void FindCustomerHandler(object parameters)
        {
            var targetCustomerNumber = CustomerNumber.Value;
            context.FindAndApplyCustomer(targetCustomerNumber);

            if (context.Customer.Value == null)
                MessageBox.Show($"Плательщик с номером лицевого счета {targetCustomerNumber} не найден.");
        }

        private void PayHandler(object parameters)
        {
            try
            {
                context.Pay(Email.Value, Counter1Cost.Value,
                    Counter2Cost.Value, Counter3Cost.Value, Description.Value);
                context.ClearCustomer();
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

        private void UpdateTotalCost()
        {
            TotalCost.Value = Counter1Cost.Value + Counter2Cost.Value + Counter3Cost.Value;
        }
    }
}
