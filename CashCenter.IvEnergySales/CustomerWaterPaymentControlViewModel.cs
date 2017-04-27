using CashCenter.Common;
using CashCenter.IvEnergySales.BusinessLogic;
using System.Windows;
using CashCenter.Dal;
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
            Counter1Cost.OnChange += (newValue) => DispatchPropertyChanged("Counter1Cost");
            Counter2Cost.OnChange += (newValue) => DispatchPropertyChanged("Counter2Cost");
            Counter3Cost.OnChange += (newValue) => DispatchPropertyChanged("Counter3Cost");
            TotalCost.OnChange += (newValue) => DispatchPropertyChanged("TotalCost");
            Description.OnChange += (newValue) => DispatchPropertyChanged("Description");

            FindCustomerCommand = new Command(FindCustomerHandler);
            PayCommand = new Command(PayHandler);
            ClearCommand = new Command(ClearHandler);

            context.OnCustomerChanged += WaterPaymentcontextCustomerChanged;
        }

        private void WaterPaymentcontextCustomerChanged(WaterCustomer customer)
        {
            if (customer == null)
            {
                MessageBox.Show($"Плательщик с номером лицевого счета {CustomerNumber} не найден.");
                return;
            }

            CustomerName.Value = customer.Name;
            CustomerAddress.Value = customer.Address;
            Email.Value = customer.Email;

            Counter1Number.Value = (uint)customer.CounterNumber1;
            Counter2Number.Value = (uint)customer.CounterNumber2;
            Counter3Number.Value = (uint)customer.CounterNumber3;
        }

        private void FindCustomerHandler(object parameters)
        {
            context.FindAndApplyCustomer(CustomerNumber.Value);
        }

        private void PayHandler(object parameters)
        {
            try
            {
                context.Pay( Email.Value, Counter1Cost.Value,
                    Counter2Cost.Value, Counter3Cost.Value, Description.Value);
            }
            catch(UserException ex)
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
