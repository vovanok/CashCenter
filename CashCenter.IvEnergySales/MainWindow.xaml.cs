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
using System;

namespace CashCenter.IvEnergySales
{
    public partial class MainWindow : Window
	{
        private EnergySalesManager energySalesManager;

        private Customer targetCustomer;
        private Customer TargetCustomer
        {
            get { return targetCustomer; }
            set
            {
                targetCustomer = value;
                ChangeTargetCustomer();
            }
        }

        private CustomerCounters TargetCustomerCountersValues { get; set; }

        CheckPrinter checkPrinter;

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

            try
            {
                checkPrinter = new CheckPrinter();
            }
            catch
            {
                Log.Error("Ошибка создания драйвера. Запустите приложение от имени администратора.");
            }

            TargetCustomer = null;
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

            TargetCustomer = energySalesManager?.GetCustomer(targetCustomerId);
            if (targetCustomer == null)
            {
                Log.Info($"Плательщик с номером лицевого счета {targetCustomerId} не найден.");
                return;
            }

            TargetCustomerCountersValues = energySalesManager?.GetCustomerCounters(TargetCustomer);

            lblCustomerName.Content = targetCustomer.Name;

            var addressComponents = new[] { targetCustomer.LocalityName, targetCustomer.StreetName, targetCustomer.BuildingNumber, targetCustomer.Flat }
                .Where(item => !string.IsNullOrEmpty(item));
            lblCustomerAddress.Content = string.Join(", ", addressComponents);

            lblDayPreviousCounterValue.Content = tbDayCurrentCounterValue.Text = TargetCustomerCountersValues?.EndDayValue.ToString() ?? 0.ToString();
            lblNightPreviousCounterValue.Content = tbNightCurrentCounterValue.Text = TargetCustomerCountersValues?.EndNightValue.ToString() ?? 0.ToString();

            UpdateDeltaValueLbl(lblDayDeltaCounterValue, tbDayCurrentCounterValue, lblDayPreviousCounterValue);
            UpdateDeltaValueLbl(lblNightDeltaCounterValue, tbNightCurrentCounterValue, lblNightPreviousCounterValue);
        }

        private void btnPay_Click(object sender, RoutedEventArgs e)
        {
            if (targetCustomer == null)
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
            if (!int.TryParse(tbDayCurrentCounterValue.Text, out dayValue) || dayValue < 0 || (TargetCustomerCountersValues != null && dayValue < TargetCustomerCountersValues.EndDayValue))
            {
                errorList.Add($"Некорректное показание дневного счетчика. Оно должно быть числом, не меньшим предыдущего показания (введенное значение: {dayValue}; предыдущее показание {TargetCustomerCountersValues.EndDayValue})");
            }

            int nightValue;
            if (!int.TryParse(tbNightCurrentCounterValue.Text, out nightValue) || nightValue < 0 || (TargetCustomerCountersValues != null && nightValue < TargetCustomerCountersValues.EndNightValue))
            {
                errorList.Add($"Некорректное показание ночного счетчика. Оно должно быть числом, не меньшим предыдущего показания (введенное значение: {nightValue}; предыдущее показание {TargetCustomerCountersValues.EndNightValue})");
            }

            decimal paymentCost;
            if (!decimal.TryParse(tbCost.Text, out paymentCost) || paymentCost < 0)
            {
                errorList.Add($"Некорректное значение суммы платежа. Оно должно быть положительным числом ({paymentCost}).");
            }

            if (!(cbPaymentReasons.SelectedValue is int))
            {
                errorList.Add($"Некорректное значение основания платежа.");
            }
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
                energySalesManager?.Pay(TargetCustomer, TargetCustomerCountersValues, reasonId, paymentCost, description, dayValue, nightValue);
            }

            PrintChecks();

            TargetCustomer = null;
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
            if (energySalesManager == null || energySalesManager.InfoForCheck == null)
                return;

            bool isPrintSuccess = false;
            using (var waiter = new OperationWaiter())
            {
                var preCheck = new PreCheck(checkPrinter)
                {
                    Date = energySalesManager.InfoForCheck.Date,
                    Cost = energySalesManager.InfoForCheck.Cost,
                    RecipientNameShort = Config.RecipientNameShort
                };

                isPrintSuccess = preCheck.Print();
            }

            if (!isPrintSuccess)
                Log.Error($"Ошибка печати чека.");
        }

        private void PrintMainCheck()
        {
            if (energySalesManager == null || energySalesManager.InfoForCheck == null)
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

                    Cost = energySalesManager.InfoForCheck.Cost
                };

                isPrintSuccess = mainCheck.Print();
            }

            if (!isPrintSuccess)
                Log.Error($"Ошибка печати чека.");
        }

		private void btnClear_Click(object sender, RoutedEventArgs e)
		{
            TargetCustomer = null;
            SetDefaultCustomerValues();
            SetDefaultPayValues();
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
            if (targetCustomer == null)
                return;

            UpdateDeltaValueLbl(lblDayDeltaCounterValue, tbDayCurrentCounterValue, lblDayPreviousCounterValue);
        }

        private void tbNightCurrentCounterValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (targetCustomer == null)
                return;

            UpdateDeltaValueLbl(lblNightDeltaCounterValue, tbNightCurrentCounterValue, lblNightPreviousCounterValue);
        }

        private void UpdateDeltaValueLbl(Label lblDeltaCounterValue, TextBox tbCurrentCounterValue, Label lblPreviousCounterValue)
        {
            int currentValue;
            if (!int.TryParse(tbCurrentCounterValue.Text, out currentValue))
            {
                lblDeltaCounterValue.Content = 0;
                return;
            }

            var prevValueContent = lblPreviousCounterValue.Content as string;
            int previousValue;
            if (prevValueContent == null || !int.TryParse(prevValueContent, out previousValue))
                previousValue = 0;

            int deltaValue = currentValue - previousValue;
            lblDeltaCounterValue.Content = deltaValue;
            lblDeltaCounterValue.Foreground = deltaValue >= 0 ? Brushes.Black : Brushes.Red;
        }

        private void ChangeTargetCustomer()
        {
            if (targetCustomer == null)
                SetDefaultCustomerValues();

            SetDefaultPayValues();
            gbPaymentInfo.IsEnabled = targetCustomer != null;
        }

        private void SetDefaultCustomerValues()
        {
            tbCustomerId.Text = string.Empty;
            lblCustomerName.Content = "-";
            lblCustomerAddress.Content = "-";
        }

        private void SetDefaultPayValues()
        {
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
    }
}
