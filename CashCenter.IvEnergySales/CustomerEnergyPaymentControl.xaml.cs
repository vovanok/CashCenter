using CashCenter.Common;
using CashCenter.Dal;
using CashCenter.IvEnergySales.BusinessLogic;
using CashCenter.IvEnergySales.Check;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CashCenter.IvEnergySales
{
    public partial class CustomerEnergyPaymentControl : UserControl
    {
        private Observed<CustomerSalesContext> customerSalesContext = new Observed<CustomerSalesContext>();

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
            get { return IsSalesContextReady ? customerSalesContext.Value.Customer.NightValue : 0; }
        }

        private decimal DebtBalance
        {
            get { return IsSalesContextReady ? customerSalesContext.Value.Customer.Balance : 0; }
        }

        public CheckPrinter CheckPrinter { get; set; }

        public CustomerEnergyPaymentControl()
        {
            InitializeComponent();
        }

        public void SettingsWereChanged()
        {
            customerSalesContext.Value = null;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
#if DEBUG
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                return;
#endif
            
            customerSalesContext.OnChange += SalesContext_OnChange;
            customerSalesContext.Value = null;
        }

        private void SalesContext_OnChange(CustomerSalesContext newCustomerSalesContext)
        {
            using (var waiter = new OperationWaiter())
            {
                if (newCustomerSalesContext == null)
                {
                    tbCustomerNumber.Text = string.Empty;
                    tbCustomerNumber.Focus();
                }

                tbCustomerEmail.IsEnabled = newCustomerSalesContext != null && newCustomerSalesContext.IsCustomerFinded;

                tbCustomerEmail.Text =
                    newCustomerSalesContext != null &&
                    newCustomerSalesContext.Customer != null &&
                    newCustomerSalesContext.Customer.Email != null
                        ? newCustomerSalesContext.Customer.Email
                        : string.Empty;

                gbPaymentInfo.IsEnabled = newCustomerSalesContext != null && newCustomerSalesContext.IsCustomerFinded;

                // Payment reasons
                cbPaymentReasons.ItemsSource = newCustomerSalesContext?.PaymentReasons ?? new List<PaymentReason>();
                if (cbPaymentReasons.Items.Count > 0)
                    cbPaymentReasons.SelectedIndex = 0;

                lblCustomerName.Content = customerSalesContext?.Value?.Customer?.Name ?? string.Empty;
                lblCustomerAddress.Content = customerSalesContext?.Value?.Customer?.Address ?? string.Empty;
                lblDayPreviousCounterValue.Content = tbDayCurrentCounterValue.Text = (IsSalesContextReady ? PreviousDayCounterValue : 0).ToString();
                lblNightPreviousCounterValue.Content = tbNightCurrentCounterValue.Text = (IsSalesContextReady ? PreviousNightCounterValue : 0).ToString();
                tbCost.Text = IsSalesContextReady ? DebtBalance.ToString("0.00") : string.Empty;
                tbDescription.Text = string.Empty;

                lblIsNormative.Visibility =
                    newCustomerSalesContext != null &&
                    newCustomerSalesContext.Customer != null &&
                    newCustomerSalesContext.Customer.IsNormative()
                        ? Visibility.Visible
                        : Visibility.Hidden;

                tbNightCurrentCounterValue.IsEnabled =
                    newCustomerSalesContext != null &&
                    newCustomerSalesContext.Customer != null &&
                    newCustomerSalesContext.Customer.IsTwoTariff();

                UpdateDayDeltaValueLbl();
                UpdateNightDeltaValueLbl();
            }
        }

        private void FindCustomerInfo()
        {
            if (controlDeparmentSelector.SelectedDepartment == null)
            {
                Log.Error("Отделение не выбрано.");
                return;
            }

            if (string.IsNullOrEmpty(tbCustomerNumber.Text))
            {
                Log.Error("Номер лицевого счета не задан.");
                return;
            }

            if (!int.TryParse(tbCustomerNumber.Text, out int customerNumber))
            {
                Log.Error("Номер лицевого счета не является числом или имеет слишком большое значение.");
                return;
            }

            using (new OperationWaiter())
            {
                customerSalesContext.Value = new CustomerSalesContext(controlDeparmentSelector.SelectedDepartment, customerNumber);
            }

            if (!customerSalesContext.Value.IsCustomerFinded)
            {
                Log.Info($"Плательщик с номером лицевого счета {customerNumber} не найден.");
                return;
            }

            if (tbDayCurrentCounterValue.IsEnabled)
            {
                tbDayCurrentCounterValue.Focus();
                return;
            }

            if (tbNightCurrentCounterValue.IsEnabled)
            {
                tbNightCurrentCounterValue.Focus();
                return;
            }

            if (tbCost.IsEnabled)
            {
                tbCost.Focus();
                return;
            }
        }

        private void DoPay()
        {
            if (customerSalesContext.Value == null || !customerSalesContext.Value.IsCustomerFinded)
            {
                MessageBox.Show("Не задан плательщик. Произведите поиск по номеру лицевого счета");
                return;
            }

            if (CheckPrinter == null || !CheckPrinter.IsReady)
            {
                if (MessageBox.Show("Кассовый аппарат не подключен. Продолжить без печати чека?", "Требуется подтверждение продолжения", MessageBoxButton.YesNo) == MessageBoxResult.No)
                    return;
            }

            var errorList = new List<string>();

            if (!int.TryParse(tbDayCurrentCounterValue.Text, out int dayValue) && dayValue >= 0)
                errorList.Add($"Некорректное показание дневного счетчика. Оно должно быть положительным числом ({dayValue}). Возможно показание не задано или является слишком большим.");

            if (!int.TryParse(tbNightCurrentCounterValue.Text, out int nightValue) && nightValue >= 0)
                errorList.Add($"Некорректное показание ночного счетчика. Оно должно быть числом ({nightValue}). Возможно показание не задано или является слишком большим.");

            if (!decimal.TryParse(tbCost.Text, out decimal paymentCost))
                errorList.Add($"Некорректное значение суммы платежа. Оно должно быть числом ({paymentCost}). Возможно сумма не задана или является слишком большой.");

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
                if (customerSalesContext.Value.Customer.Email != tbCustomerEmail.Text)
                    customerSalesContext.Value.ChangeEmail(tbCustomerEmail.Text);

                var customerPayment = new CustomerPayment
                {
                    CustomerId = customerSalesContext.Value.Customer.Id,
                    NewDayValue = dayValue,
                    NewNightValue = nightValue,
                    Cost = paymentCost,
                    ReasonId = reasonId,
                    CreateDate = DateTime.Now,
                    Description = description
                };

                if (!customerSalesContext.Value.Pay(customerPayment))
                    return;
            }

            PrintChecks();
            customerSalesContext.Value = null;
        }

        #region Simple events handlers

        private void On_btnFindCustomer_Click(object sender, RoutedEventArgs e)
        {
            FindCustomerInfo();
        }

        private void On_tbCustomerNumber_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter || e.IsToggled || string.IsNullOrEmpty(tbCustomerNumber.Text))
                return;

            FindCustomerInfo();
        }

        private void On_btnClear_Click(object sender, RoutedEventArgs e)
        {
            customerSalesContext.Value = null;
        }

        private void On_tbDayCurrentCounterValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateDayDeltaValueLbl();
        }

        private void On_tbNightCurrentCounterValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateNightDeltaValueLbl();
        }

        private void On_btnPay_Click(object sender, RoutedEventArgs e)
        {
            DoPay();
        }

        private void On_controlDeparmentSelector_DepartmentChanged(object sender, SelectionChangedEventArgs e)
        {
            tbCustomerNumber.Focus();
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

        private void On_tbDayCurrentCounterValue_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            if (tbNightCurrentCounterValue.IsEnabled)
                tbNightCurrentCounterValue.Focus();
            else
                tbCost.Focus();
        }

        private void On_tbNightCurrentCounterValue_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            tbCost.Focus();
        }

        private void On_tbCost_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            DoPay();
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

        #region Print checks

        private void PrintChecks()
        {
            if (CheckPrinter == null || !CheckPrinter.IsReady)
                return;

            if (!IsSalesContextReady || customerSalesContext.Value.InfoForCheck == null)
                return;

            bool isPrintSuccess = false;
            using (var waiter = new OperationWaiter())
            {
                var mainCheck = new CustomerCheck(CheckPrinter)
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
    }
}
