using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using CashCenter.IvEnergySales.DbQualification;
using CashCenter.IvEnergySales.BusinessLogic;
using CashCenter.IvEnergySales.DataModel;
using CashCenter.IvEnergySales.Logging;
using System.Text;

namespace CashCenter.IvEnergySales
{
    public partial class MainWindow : Window
	{
        private EnergySalesManager energySalesManager;

		public MainWindow()
		{
			InitializeComponent();
		}

        private void Main_Loaded(object sender, RoutedEventArgs e)
        {
#if DEBUG
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                return;
#endif
            // Load department info
            var currentDepartment = DbQualifier.GetCurrentDepartment();
            if (currentDepartment != null)
            {
                energySalesManager = new EnergySalesManager(currentDepartment);

                lblDepartmentName.Content = currentDepartment.Name;
                cbDbSelector.ItemsSource = currentDepartment.Dbs ?? new List<DbModel>();
                if (cbDbSelector.Items.Count > 0)
                {
                    cbDbSelector.SelectedIndex = 0;
                    UpdateDbCodeFromSelector();
                }
            }
            else
            {
                lblDepartmentName.Content = "Отделение не задано";
                lblDepartmentName.Foreground = Brushes.Red;
                cbDbSelector.IsEnabled = false;
            }

            // Load reasons
            cbPaymentReasons.ItemsSource = energySalesManager?.GetPaymentReasons() ?? new List<PaymentReason>();
            if (cbPaymentReasons.Items.Count > 0)
                cbPaymentReasons.SelectedIndex = 0;
        }

        private void btnFindCustomer_Click(object sender, RoutedEventArgs e)
        {
            FindCustomerInfo();
        }

        private void tbCustomerId_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            FindCustomerInfo();
        }

        private string GetSeparatedString(IEnumerable<string> values, string separator)
        {
            var sb = new StringBuilder();
            foreach(var value in values)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (sb.Length > 0)
                        sb.Append(separator);

                    sb.Append(value);
                }
            }

            return sb.ToString();
        }

        private void cbDbSelector_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            UpdateDbCodeFromSelector();
        }

        private void UpdateDbCodeFromSelector()
        {
            energySalesManager?.SetDbCode(cbDbSelector.SelectedValue.ToString());
        }

        private void FindCustomerInfo()
        {
            int targetCustomerId;
            if (!int.TryParse(tbCustomerId.Text, out targetCustomerId))
            {
                Log.Error($"Номер лицевого счета должен быть числом ({tbCustomerId.Text}).");
                return;
            }

            var targetCustomer = energySalesManager?.GetCustomer(targetCustomerId);
            if (targetCustomer == null)
            {
                Log.Info($"Плательщик с номером лицевого счета {targetCustomerId} не найден.");
                return;
            }

            lblCustomerName.Content = targetCustomer.Name;
            lblCustomerAddress.Content = GetSeparatedString(
                new[] { targetCustomer.LocalityName, targetCustomer.StreetName, targetCustomer.BuildingNumber, targetCustomer.Flat }, ", ");
        }
    }
}
