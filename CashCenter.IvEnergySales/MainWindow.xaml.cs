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
        private Observed<EnergySalesDbContext> salesContext = new Observed<EnergySalesDbContext>();

        private string CustomerName
        {
            get { return salesContext?.Value?.Customer?.Name ?? string.Empty; }
        }

        private bool IsSalesContextReady
        {
            get { return salesContext?.Value != null && salesContext.Value.IsCustomerFinded; }
        }

        private string GetCustomerAddress()
        {
            if (!IsSalesContextReady)
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
                if (!IsSalesContextReady || salesContext.Value.IsNormative)
                    return 0;

                return salesContext.Value.CustomerCountersValues.EndDayValue;
            }
        }

        private int PreviousNightCounterValue
        {
            get
            {
                if (!IsSalesContextReady || salesContext.Value.IsNormative || !salesContext.Value.IsTwoTariff)
                    return 0;

                return salesContext.Value.CustomerCountersValues.EndNightValue;
            }
        }

        private decimal DebtBalance
        {
            get
            {
                if (!IsSalesContextReady)
                    return 0;

                var debt = salesContext?.Value?.Debt;
                if (debt == null)
                    return 0;

                return debt.Balance;
            }
        }

        public MainWindow()
		{
			InitializeComponent();

            salesContext.OnChange += SalesContext_OnChange;
        }

        private void SalesContext_OnChange(EnergySalesDbContext newSalesContext)
        {
            using (var waiter = new OperationWaiter())
            {
                if (newSalesContext == null)
                    tbCustomerId.Text = string.Empty;

                gbPaymentInfo.IsEnabled = newSalesContext != null && newSalesContext.IsCustomerFinded;

                // Payment reasons
                cbPaymentReasons.ItemsSource = newSalesContext?.PaymentReasons ?? new List<PaymentReason>();
                if (cbPaymentReasons.Items.Count > 0)
                    cbPaymentReasons.SelectedIndex = 0;

                lblCustomerName.Content = CustomerName;
                lblCustomerAddress.Content = GetCustomerAddress();
                lblDayPreviousCounterValue.Content = tbDayCurrentCounterValue.Text = (IsSalesContextReady ? PreviousDayCounterValue : 0).ToString();
                lblNightPreviousCounterValue.Content = tbNightCurrentCounterValue.Text = (IsSalesContextReady ? PreviousNightCounterValue : 0).ToString();
                tbCost.Text = IsSalesContextReady ? DebtBalance.ToString("0.00") : string.Empty;
                tbDescription.Text = string.Empty;

                lblIsNormative.Visibility = newSalesContext != null && newSalesContext.IsNormative ? Visibility.Visible : Visibility.Hidden;
                tbNightCurrentCounterValue.IsEnabled = newSalesContext != null && newSalesContext.IsTwoTariff;

                UpdateDayDeltaValueLbl();
                UpdateNightDeltaValueLbl();
            }
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
                cbDbSelector.ItemsSource = (currentDepartment.Dbs ?? new List<DbModel>())
                    .Select(dbModel => new { DbCode = dbModel.DbCode, DbFullName = $"{dbModel.DbCode} {dbModel.Name}" });
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
                salesContext.Value = new EnergySalesDbContext(targetCustomerId, currentDepartment, cbDbSelector.SelectedValue.ToString());
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
                if (!salesContext.Value.Pay(reasonId, dayValue, nightValue, paymentCost, description))
                    return;
            }

            PrintChecks();

            salesContext.Value = null;
        }

        #region Print checks

        private void PrintChecks()
        {
            if (checkPrinter == null || !checkPrinter.IsReady)
                return;

            if (!IsSalesContextReady || salesContext.Value.InfoForCheck == null)
                return;

            bool isPrintSuccess = false;
            using (var waiter = new OperationWaiter())
            {
                var mainCheck = new MainCheck(checkPrinter)
                {
                    SalesDepartmentInfo = Config.SalesDepartmentInfo,
                    DepartmentCode = salesContext.Value.InfoForCheck.DbCode,
                    CustomerId = salesContext.Value.InfoForCheck.CustomerId,
                    CustomerName = salesContext.Value.InfoForCheck.CustomerName,
                    PaymentReason = salesContext.Value.InfoForCheck.PaymentReasonName,
                    CashierName = Config.CashierName,
                    Cost = salesContext.Value.InfoForCheck.Cost
                };

                isPrintSuccess = mainCheck.Print();
            }

            if (!isPrintSuccess)
                Log.Error($"Ошибка печати чека.");
        }

        #endregion

        #region Simple events handlers

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

        #endregion

        #region Update counters labels

        private void UpdateDayDeltaValueLbl()
        {
            int dayPreviousCounterValue = (salesContext.Value != null && !salesContext.Value.IsNormative)
                ? salesContext.Value.CustomerCountersValues.EndDayValue
                : 0;

            UpdateDeltaValueLbl(lblDayDeltaCounterValue, tbDayCurrentCounterValue, dayPreviousCounterValue);
        }

        private void UpdateNightDeltaValueLbl()
        {
            int nightPreviousCounterValue =
                (salesContext.Value != null && !salesContext.Value.IsNormative && salesContext.Value.IsTwoTariff)
                    ? salesContext.Value.CustomerCountersValues.EndNightValue
                    : 0;

            UpdateDeltaValueLbl(lblNightDeltaCounterValue, tbNightCurrentCounterValue, nightPreviousCounterValue);
        }

        private void UpdateDeltaValueLbl(Label lblDeltaCounterValue, TextBox tbCurrentCounterValue, int previousCounterValue)
        {
            if (lblDeltaCounterValue == null || tbCurrentCounterValue == null)
                return;

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

        #endregion

        #region Text boxes focus manipulation

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

        private void tbCost_GotFocus(object sender, RoutedEventArgs e)
        {
            tbCost.SelectAll();
        }

        private void tbCost_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!tbCost.IsKeyboardFocusWithin)
            {
                e.Handled = true;
                tbCost.Focus();
            }
        }

        #endregion
    }
}
