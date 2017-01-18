using System.Collections.Generic;
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
using System;
using System.ComponentModel;

namespace CashCenter.IvEnergySales
{
    public partial class MainWindow : Window
	{
        private RegionDef currentDepartment;
        private CheckPrinter checkPrinter;
        private Observed<BaseEnergySalesContext> salesContext = new Observed<BaseEnergySalesContext>();

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

        private void SalesContext_OnChange(BaseEnergySalesContext newSalesContext)
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
            currentDepartment = QualifierManager.GetCurrentDepartment();
            if (currentDepartment != null)
            {
                lblDepartmentName.Content = currentDepartment.Name;
                cbDepartmentSelector.ItemsSource = (currentDepartment.Departments ?? new List<DepartmentDef>())
                    .Select(departmentDef => new { DepartmentCode = departmentDef.Code, DepartmentFullName = $"{departmentDef.Code} {departmentDef.Name}" });
                if (cbDepartmentSelector.Items.Count > 0)
                    cbDepartmentSelector.SelectedIndex = 0;
            }
            else
            {
                lblDepartmentName.Content = "Отделение не задано";
                lblDepartmentName.Foreground = Brushes.Red;
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

            salesContext.Value = null;
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
                salesContext.Value =
                    !Config.IsUseOfflineMode
                        ? (BaseEnergySalesContext)new EnergySalesRemoteContext(targetCustomerId, currentDepartment, cbDepartmentSelector.SelectedValue.ToString())
                        : (BaseEnergySalesContext)new EnergySalesOfflineContext(targetCustomerId, currentDepartment, cbDepartmentSelector.SelectedValue.ToString());
            }

            if (!salesContext.Value.IsCustomerFinded)
                Log.Info($"Плательщик с номером лицевого счета {targetCustomerId} не найден.");
        }

        private void On_btnPay_Click(object sender, RoutedEventArgs e)
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
                if (!salesContext.Value.Pay(reasonId, 19, dayValue, nightValue, paymentCost, description, DateTime.Now)) // TODO: Make combobox for paymentKind
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
            salesContext.Value = null;
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
