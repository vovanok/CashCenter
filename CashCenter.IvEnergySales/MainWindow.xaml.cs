using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using CashCenter.IvEnergySales.DbQualification;
using CashCenter.IvEnergySales.BusinessLogic;
using CashCenter.IvEnergySales.DataModel;
using CashCenter.IvEnergySales.Logging;
using System.Linq;
using CashCenter.IvEnergySales.Check;
using System.Windows.Controls;
using CashCenter.IvEnergySales.Utils;

namespace CashCenter.IvEnergySales
{
    public partial class MainWindow : Window
	{
        private DepartmentModel currentDepartment;
        private CheckPrinter checkPrinter;
        private Observed<EnergySalesContext> salesContext = new Observed<EnergySalesContext>();

        private string CustomerName
        {
            get { return salesContext?.Value?.Customer?.Name ?? string.Empty; }
        }

        private string GetCustomerAddress()
        {
            if (salesContext?.Value == null || !salesContext.Value.IsCustomerFinded)
                return string.Empty;

            var addressComponents = new[]
            {
                salesContext.Value.Customer.LocalityName,
                salesContext.Value.Customer.StreetName,
                salesContext.Value.Customer.BuildingNumber,
                salesContext.Value.Customer.Flat
            };

            return string.Join(", ", addressComponents.Where(item => !string.IsNullOrEmpty(item)));
        }

        private int PreviousDayCounterValue
        {
            get
            {
                if (salesContext?.Value == null || salesContext.Value.IsNormative)
                    return 0;

                return salesContext.Value.CustomerCountersValues.EndDayValue;
            }
        }

        private int PreviousNightCounterValue
        {
            get
            {
                if (salesContext?.Value == null || salesContext.Value.IsNormative || !salesContext.Value.IsTwoTariff)
                    return 0;

                return salesContext.Value.CustomerCountersValues.EndNightValue;
            }
        }

        public MainWindow()
		{
			InitializeComponent();

            salesContext.OnChange += SalesContext_OnChange;
        }

        private void SalesContext_OnChange(EnergySalesContext newSalesContext)
        {
            if (newSalesContext == null)
            {
                tbCustomerId.Text = string.Empty;
                var zeroText = 0.ToString();

                lblDayPreviousCounterValue.Content = zeroText;
                lblNightPreviousCounterValue.Content = zeroText;

                tbDayCurrentCounterValue.Text = zeroText;
                tbNightCurrentCounterValue.Text = zeroText;

                lblDayDeltaCounterValue.Content = zeroText;
                lblNightDeltaCounterValue.Content = zeroText;

                tbCost.Text = string.Empty;
                tbDescription.Text = string.Empty;
            }

            gbPaymentInfo.IsEnabled = newSalesContext != null && newSalesContext.IsCustomerFinded;

            // Payment reasons
            cbPaymentReasons.ItemsSource = newSalesContext?.GetPaymentReasons() ?? new List<PaymentReason>();
            if (cbPaymentReasons.Items.Count > 0)
                cbPaymentReasons.SelectedIndex = 0;

            lblCustomerName.Content = CustomerName;
            lblCustomerAddress.Content = GetCustomerAddress();
            lblDayPreviousCounterValue.Content = tbDayCurrentCounterValue.Text = PreviousDayCounterValue.ToString();
            lblNightPreviousCounterValue.Content = tbNightCurrentCounterValue.Text = PreviousNightCounterValue.ToString();

            lblIsNormative.Visibility = newSalesContext != null && newSalesContext.IsNormative ? Visibility.Visible : Visibility.Hidden;
            tbNightCurrentCounterValue.IsEnabled = newSalesContext != null && newSalesContext.IsTwoTariff;

            UpdateDayDeltaValueLbl();
            UpdateNightDeltaValueLbl();
        }

        private void Main_Loaded(object sender, RoutedEventArgs e)
        {
#if DEBUG
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                return;
#endif
            // Load department info
            currentDepartment = DbQualifier.GetCurrentDepartment();
            if (currentDepartment != null)
            {
                lblDepartmentName.Content = currentDepartment.Name;
                cbDbSelector.ItemsSource = currentDepartment.Dbs ?? new List<DbModel>();
                if (cbDbSelector.Items.Count > 0)
                    cbDbSelector.SelectedIndex = 0;
            }
            else
            {
                lblDepartmentName.Content = "Отделение не задано";
                lblDepartmentName.Foreground = Brushes.Red;
                cbDbSelector.IsEnabled = false;
            }

            try
            {
                checkPrinter = new CheckPrinter();
            }
            catch
            {
                Log.Error("Ошибка создания драйвера. Запустите приложение от имени администратора.");
            }

            salesContext.Value = null;
        }

        private void btnFindCustomer_Click(object sender, RoutedEventArgs e)
        {
            FindCustomerInfo();
        }

        private void tbCustomerId_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            FindCustomerInfo();
        }

        private void FindCustomerInfo()
        {
            int targetCustomerId;
            if (!int.TryParse(tbCustomerId.Text, out targetCustomerId))
            {
                Log.Error($"Номер лицевого счета должен быть числом ({tbCustomerId.Text}).");
                return;
            }

            using (var waiter = new OperationWaiter())
            {
                salesContext.Value = new EnergySalesContext(targetCustomerId, currentDepartment, cbDbSelector.SelectedValue.ToString());
            }

            if (!salesContext.Value.IsCustomerFinded)
                Log.Info($"Плательщик с номером лицевого счета {targetCustomerId} не найден.");
        }

        private void btnPay_Click(object sender, RoutedEventArgs e)
        {
            if (salesContext.Value == null || !salesContext.Value.IsCustomerFinded)
            {
                MessageBox.Show("Не задан плательщик. Произведите поиск по номеру лицевого счета");
                return;
            }

            if (checkPrinter == null || !checkPrinter.IsReady)
            {
                if (MessageBox.Show("Кассовый аппарат не подключен. Продолжить без печати чека?", "Требуется подтверждение продолжения", MessageBoxButton.YesNo) == MessageBoxResult.No)
                    return;
            }

            var errorList = new List<string>();

            int dayValue;
            if (!int.TryParse(tbDayCurrentCounterValue.Text, out dayValue))
                errorList.Add($"Некорректное показание дневного счетчика. Оно должно быть числом ({dayValue}).");

            int nightValue;
            if (!int.TryParse(tbNightCurrentCounterValue.Text, out nightValue))
                errorList.Add($"Некорректное показание ночного счетчика. Оно должно быть числом ({nightValue}).");

            decimal paymentCost;
            if (!decimal.TryParse(tbCost.Text, out paymentCost))
                errorList.Add($"Некорректное значение суммы платежа. Оно должно быть числом ({paymentCost}).");

            if (!(cbPaymentReasons.SelectedValue is int))
                errorList.Add($"Некорректное значение основания платежа.");

            int reasonId = (int)cbPaymentReasons.SelectedValue;

            string description = tbDescription.Text ?? string.Empty;

            if (errorList.Count > 0)
            {
                var errorMessage = $"Ошибки заполнения данных\n{string.Join("\n", errorList.Select(item => $"- {item}"))}";
                Log.Error(errorMessage);
                return;
            }

            using (var waiter = new OperationWaiter())
            {
                salesContext.Value.Pay(reasonId, dayValue, nightValue, paymentCost, description);
            }

            PrintChecks();

            salesContext.Value = null;
        }

        private void PrintChecks()
        {
            if (checkPrinter == null || !checkPrinter.IsReady)
                return;

            if (Config.IsPreCheckPrint)
            {
                PrintPreCheck();

                if (MessageBox.Show("Печатать ли основной чек?", "Требуется подтверждение печати", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    PrintMainCheck();
            }
            else
            {
                PrintMainCheck();
            }
        }

        private void PrintPreCheck()
        {
            if (salesContext?.Value == null || salesContext.Value.InfoForCheck == null)
                return;

            bool isPrintSuccess = false;
            using (var waiter = new OperationWaiter())
            {
                var preCheck = new PreCheck(checkPrinter)
                {
                    Date = salesContext.Value.InfoForCheck.Date,
                    Cost = salesContext.Value.InfoForCheck.Cost,
                    RecipientNameShort = Config.RecipientNameShort
                };

                isPrintSuccess = preCheck.Print();
            }

            if (!isPrintSuccess)
                Log.Error($"Ошибка печати чека.");
        }

        private void PrintMainCheck()
        {
            if (salesContext?.Value == null || salesContext.Value.InfoForCheck == null)
                return;

            bool isPrintSuccess = false;
            using (var waiter = new OperationWaiter())
            {
                var mainCheck = new MainCheck(checkPrinter)
                {
                    RecipientName = Config.RecipientName,
                    RecipientNameShort = Config.RecipientNameShort,
                    RecipientInn = Config.RecipientInn,
                    RecipientAddressLine1 = Config.RecipientAddressLine1,
                    RecipientAddressLine2 = Config.RecipientAddressLine2,

                    SellerName = Config.SellerName,
                    SellerInn = Config.SellerInn,
                    SellerAddressLine1 = Config.SellerAddressLine1,
                    SellerAddressLine2 = Config.SellerAddressLine2,

                    CashierName = Config.CashierName,

                    Cost = salesContext.Value.InfoForCheck.Cost
                };

                isPrintSuccess = mainCheck.Print();
            }

            if (!isPrintSuccess)
                Log.Error($"Ошибка печати чека.");
        }

		private void btnClear_Click(object sender, RoutedEventArgs e)
		{
            salesContext.Value = null;
        }

		private void miCashPrinterSettings_Click(object sender, RoutedEventArgs e)
		{
			checkPrinter.ShowProperties();
		}

        private void miCashPrinterCancelCheck_Click(object sender, RoutedEventArgs e)
        {
            checkPrinter.CancelCheck();
        }

        private void tbDayCurrentCounterValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateDayDeltaValueLbl();
        }

        private void tbNightCurrentCounterValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateNightDeltaValueLbl();
        }

        private void UpdateDayDeltaValueLbl()
        {
            if (salesContext.Value == null || salesContext.Value.IsNormative)
                return;

            int dayPreviousCounterValue = salesContext.Value.CustomerCountersValues.EndDayValue;
            UpdateDeltaValueLbl(lblDayDeltaCounterValue, tbDayCurrentCounterValue, dayPreviousCounterValue);
        }

        private void UpdateNightDeltaValueLbl()
        {
            if (salesContext.Value == null || salesContext.Value.IsNormative || !salesContext.Value.IsTwoTariff)
                return;

            int nightPreviousCounterValue = salesContext.Value.CustomerCountersValues.EndNightValue;
            UpdateDeltaValueLbl(lblNightDeltaCounterValue, tbNightCurrentCounterValue, nightPreviousCounterValue);
        }

        private void UpdateDeltaValueLbl(Label lblDeltaCounterValue, TextBox tbCurrentCounterValue, int previousCounterValue)
        {
            int currentValue;
            if (!int.TryParse(tbCurrentCounterValue.Text, out currentValue))
            {
                lblDeltaCounterValue.Content = 0;
                return;
            }

            int deltaValue = currentValue - previousCounterValue;
            lblDeltaCounterValue.Content = deltaValue;
            lblDeltaCounterValue.Foreground = deltaValue >= 0 ? Brushes.Black : Brushes.Red;
        }

        #region Counter values text boxes focus manipulation

        private void tbDayCurrentCounterValue_GotFocus(object sender, RoutedEventArgs e)
        {
            tbDayCurrentCounterValue.SelectAll();
        }

        private void tbDayCurrentCounterValue_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!tbDayCurrentCounterValue.IsKeyboardFocusWithin)
            {
                e.Handled = true;
                tbDayCurrentCounterValue.Focus();
            }
        }

        private void tbNightCurrentCounterValue_GotFocus(object sender, RoutedEventArgs e)
        {
            tbNightCurrentCounterValue.SelectAll();
        }

        private void tbNightCurrentCounterValue_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!tbNightCurrentCounterValue.IsKeyboardFocusWithin)
            {
                e.Handled = true;
                tbNightCurrentCounterValue.Focus();
            }
        }

        #endregion
    }
}
