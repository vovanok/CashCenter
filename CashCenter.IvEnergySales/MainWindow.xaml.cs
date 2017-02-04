using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using CashCenter.IvEnergySales.BusinessLogic;
using CashCenter.IvEnergySales.Logging;
using System.Linq;
using CashCenter.IvEnergySales.Check;
using System.Windows.Controls;
using System;
using System.ComponentModel;
using CashCenter.Common;
using CashCenter.Common.DbQualification;
using CashCenter.Common.DataEntities;

namespace CashCenter.IvEnergySales
{
    public partial class MainWindow : Window
	{
        private RegionDef currentRegion;
        private CheckPrinter checkPrinter;
        private Observed<BaseCustomerSalesContext> customerSalesContext = new Observed<BaseCustomerSalesContext>();

        private bool IsSalesContextReady
        {
            get { return customerSalesContext?.Value != null && customerSalesContext.Value.IsCustomerFinded; }
        }

        private int PreviousDayCounterValue
        {
            get { return IsSalesContextReady ? customerSalesContext.Value.Customer.DayValue : 0; }
        }

        private int PreviousNightCounterValue
        {
            get { return IsSalesContextReady ? customerSalesContext.Value.Customer.NightValue : 0;}
        }

        private decimal DebtBalance
        {
            get { return IsSalesContextReady ? customerSalesContext.Value.Customer.Balance : 0; }
        }

        public MainWindow()
		{
			InitializeComponent();

            Log.SetLogger(new MessageBoxLog());
            customerSalesContext.OnChange += SalesContext_OnChange;
        }

        private void SalesContext_OnChange(BaseCustomerSalesContext newSalesContext)
        {
            using (var waiter = new OperationWaiter())
            {
                gridOfflineDataSrcView.Visibility = Properties.Settings.Default.IsCustomerOfflineMode ? Visibility.Visible : Visibility.Collapsed;
                gridOnlineDataSrcView.Visibility = !Properties.Settings.Default.IsCustomerOfflineMode ? Visibility.Visible : Visibility.Collapsed;
                
                lblDbfFileName.Text = Properties.Settings.Default.CustomerInputDbfPath;

                if (newSalesContext == null)
                    tbCustomerId.Text = string.Empty;

                gbPaymentInfo.IsEnabled = newSalesContext != null && newSalesContext.IsCustomerFinded;

                // Payment reasons
                cbPaymentReasons.ItemsSource = newSalesContext?.PaymentReasons ?? new List<PaymentReason>();
                if (cbPaymentReasons.Items.Count > 0)
                    cbPaymentReasons.SelectedIndex = 0;

                lblCustomerName.Content = customerSalesContext?.Value?.Customer?.Name ?? string.Empty;
                lblCustomerAddress.Content = customerSalesContext?.Value?.Customer?.Address ?? string.Empty;
                lblDayPreviousCounterValue.Content = tbDayCurrentCounterValue.Text = (IsSalesContextReady ? PreviousDayCounterValue : 0).ToString();
                lblNightPreviousCounterValue.Content = tbNightCurrentCounterValue.Text = (IsSalesContextReady ? PreviousNightCounterValue : 0).ToString();
                tbCost.Text = IsSalesContextReady ? DebtBalance.ToString("0.00") : string.Empty;
                tbDescription.Text = string.Empty;

                lblIsNormative.Visibility =
                    newSalesContext != null && newSalesContext.Customer != null && newSalesContext.Customer.IsNormative
                        ? Visibility.Visible : Visibility.Hidden;
                tbNightCurrentCounterValue.IsEnabled =
                    newSalesContext != null && newSalesContext.Customer != null && newSalesContext.Customer.IsTwoTariff;

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
            currentRegion = QualifierManager.GetCurrentRegion();
            if (currentRegion != null)
            {
                lblRegionName.Content = currentRegion.Name;
                cbDepartmentSelector.ItemsSource = (currentRegion.Departments ?? new List<DepartmentDef>())
                    .Select(departmentDef => new { DepartmentCode = departmentDef.Code, DepartmentFullName = $"{departmentDef.Code} {departmentDef.Name}" });
                if (cbDepartmentSelector.Items.Count > 0)
                    cbDepartmentSelector.SelectedIndex = 0;
            }
            else
            {
                lblRegionName.Content = "Район не задан";
                lblRegionName.Foreground = Brushes.Red;
                cbDepartmentSelector.IsEnabled = false;
            }

            try
            {
                checkPrinter = new CheckPrinter();
            }
            catch
            {
                Log.Error("Ошибка создания драйвера. Запустите приложение от имени администратора.");
            }

            customerSalesContext.Value = null;
        }

        private void FindCustomerInfo()
        {
            if (!int.TryParse(tbCustomerId.Text, out int targetCustomerId))
            {
                Log.Error($"Номер лицевого счета должен быть числом ({tbCustomerId.Text}).");
                return;
            }

            using (var waiter = new OperationWaiter())
            {
                customerSalesContext.Value =
                    !Properties.Settings.Default.IsCustomerOfflineMode
                        ? (BaseCustomerSalesContext)new OnlineCustomerSalesContext(targetCustomerId, currentRegion, cbDepartmentSelector.SelectedValue.ToString())
                        : (BaseCustomerSalesContext)new OfflineCustomerSalesContext(targetCustomerId, Properties.Settings.Default.CustomerInputDbfPath);
            }

            if (!customerSalesContext.Value.IsCustomerFinded)
                Log.Info($"Плательщик с номером лицевого счета {targetCustomerId} не найден.");
        }

        private void On_btnPay_Click(object sender, RoutedEventArgs e)
        {
            if (customerSalesContext.Value == null || !customerSalesContext.Value.IsCustomerFinded)
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

            if (!int.TryParse(tbDayCurrentCounterValue.Text, out int dayValue))
                errorList.Add($"Некорректное показание дневного счетчика. Оно должно быть числом ({dayValue}).");

            if (!int.TryParse(tbNightCurrentCounterValue.Text, out int nightValue))
                errorList.Add($"Некорректное показание ночного счетчика. Оно должно быть числом ({nightValue}).");

            if (!decimal.TryParse(tbCost.Text, out decimal paymentCost))
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
                var customerPayment = new Common.DataEntities.CustomerPayment(customerSalesContext.Value.Customer,
                    dayValue, nightValue, paymentCost, 19, reasonId, DateTime.Now); // TODO: Make combobox for paymentKind

                if (!customerSalesContext.Value.Pay(customerPayment)) 
                    return;
            }

            PrintChecks();
            customerSalesContext.Value = null;
        }

        #region Print checks

        private void PrintChecks()
        {
            if (checkPrinter == null || !checkPrinter.IsReady)
                return;

            if (!IsSalesContextReady || customerSalesContext.Value.InfoForCheck == null)
                return;

            bool isPrintSuccess = false;
            using (var waiter = new OperationWaiter())
            {
                var mainCheck = new MainCheck(checkPrinter)
                {
                    SalesDepartmentInfo = Config.SalesDepartmentInfo,
                    DepartmentCode = customerSalesContext.Value.InfoForCheck.DbCode,
                    CustomerId = customerSalesContext.Value.InfoForCheck.CustomerId,
                    CustomerName = customerSalesContext.Value.InfoForCheck.CustomerName,
                    PaymentReason = customerSalesContext.Value.InfoForCheck.PaymentReasonName,
                    CashierName = Properties.Settings.Default.CasherName,
                    Cost = customerSalesContext.Value.InfoForCheck.Cost
                };

                isPrintSuccess = mainCheck.Print();
            }

            if (!isPrintSuccess)
                Log.Error($"Ошибка печати чека.");
        }

        #endregion

        #region Simple events handlers

        private void On_btnFindCustomer_Click(object sender, RoutedEventArgs e)
        {
            FindCustomerInfo();
        }

        private void On_tbCustomerId_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            FindCustomerInfo();
        }

        private void On_btnClear_Click(object sender, RoutedEventArgs e)
		{
            customerSalesContext.Value = null;
        }

		private void On_miCashPrinterSettings_Click(object sender, RoutedEventArgs e)
		{
			checkPrinter.ShowProperties();
		}

        private void On_miCashPrinterCancelCheck_Click(object sender, RoutedEventArgs e)
        {
            checkPrinter.CancelCheck();
        }

        private void On_tbDayCurrentCounterValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateDayDeltaValueLbl();
        }

        private void On_tbNightCurrentCounterValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateNightDeltaValueLbl();
        }

        private void On_miSettings_Click(object sender, RoutedEventArgs e)
        {
            var settings = new SettingsDialog();
            var settingsDialogResult = settings.ShowDialog();

            if (settingsDialogResult != null && settingsDialogResult.Value)
                customerSalesContext.Value = null;
        }

        #endregion

        #region Update counters labels

        private void UpdateDayDeltaValueLbl()
        {
            int dayPreviousCounterValue = IsSalesContextReady ? customerSalesContext.Value.Customer.DayValue : 0;
            UpdateDeltaValueLbl(lblDayDeltaCounterValue, tbDayCurrentCounterValue, dayPreviousCounterValue);
        }

        private void UpdateNightDeltaValueLbl()
        {
            int nightPreviousCounterValue = IsSalesContextReady ? customerSalesContext.Value.Customer.NightValue : 0;
            UpdateDeltaValueLbl(lblNightDeltaCounterValue, tbNightCurrentCounterValue, nightPreviousCounterValue);
        }

        private void UpdateDeltaValueLbl(Label lblDeltaCounterValue, TextBox tbCurrentCounterValue, int previousCounterValue)
        {
            if (lblDeltaCounterValue == null || tbCurrentCounterValue == null)
                return;

            if (!int.TryParse(tbCurrentCounterValue.Text, out int currentValue))
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

        private void On_tbDayCurrentCounterValue_GotFocus(object sender, RoutedEventArgs e)
        {
            tbDayCurrentCounterValue.SelectAll();
        }

        private void On_tbDayCurrentCounterValue_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!tbDayCurrentCounterValue.IsKeyboardFocusWithin)
            {
                e.Handled = true;
                tbDayCurrentCounterValue.Focus();
            }
        }

        private void On_tbNightCurrentCounterValue_GotFocus(object sender, RoutedEventArgs e)
        {
            tbNightCurrentCounterValue.SelectAll();
        }

        private void On_tbNightCurrentCounterValue_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!tbNightCurrentCounterValue.IsKeyboardFocusWithin)
            {
                e.Handled = true;
                tbNightCurrentCounterValue.Focus();
            }
        }

        private void On_tbCost_GotFocus(object sender, RoutedEventArgs e)
        {
            tbCost.SelectAll();
        }

        private void On_tbCost_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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
