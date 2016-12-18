using System;
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

namespace CashCenter.IvEnergySales
{
    public partial class MainWindow : Window
	{
        private EnergySalesManager energySalesManager;
        private Customer targetCustomer;
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

            targetCustomer = energySalesManager?.GetCustomer(targetCustomerId);
            if (targetCustomer == null)
            {
                Log.Info($"Плательщик с номером лицевого счета {targetCustomerId} не найден.");
                return;
            }

            lblCustomerName.Content = targetCustomer.Name;

            var addressComponents = new[] { targetCustomer.LocalityName, targetCustomer.StreetName, targetCustomer.BuildingNumber, targetCustomer.Flat }
                .Where(item => !string.IsNullOrEmpty(item));
            lblCustomerAddress.Content = string.Join(", ", addressComponents);

            lblDayPreviousCounterValue.Content = tbDayCurrentCounterValue.Text = targetCustomer.Counters.EndDayValue.ToString();
            lblNightPreviousCounterValue.Content = tbNightCurrentCounterValue.Text = targetCustomer.Counters.EndNightValue.ToString();
        }

        private void btnPay_Click(object sender, RoutedEventArgs e)
        {
            decimal paymentCost;
            if (!decimal.TryParse(tbCost.Text, out paymentCost))
            {
                Log.Error($"Некорректное значение суммы платежа. Оно должно быть числом ({paymentCost}).");
                return;
            }

            if (!(cbPaymentReasons.SelectedValue is int))
            {
                Log.Error($"Некорректное значение овнования платежа.");
                return;
            }
            int reasonId = (int)cbPaymentReasons.SelectedValue;

            string description = tbDescription.Text ?? string.Empty;

            int value1;
            if (!int.TryParse(tbDayCurrentCounterValue.Text, out value1))
            {
                Log.Error($"Некорректное показание дневного счетчика. Оно должно быть числом.");
                return;
            }

            int value2;
            if (!int.TryParse(tbNightCurrentCounterValue.Text, out value2))
            {
                Log.Error($"Некорректное показание ночного счетчика. Оно должно быть числом.");
                return;
            }

            energySalesManager?.Pay(targetCustomer, reasonId, paymentCost, description, value1, value2);

            PrintPreCheck();

            if (MessageBox.Show("Печатать ли основной чек?", "Требуется подтверждение печати", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                PrintMainCheck();
        }

        private void PrintPreCheck()
        {
            if (energySalesManager == null)
            {
                Log.Error($"Ошибка печати чека.");
                return;
            }

            var preCheck = new PreCheck(checkPrinter)
            {
                Date = energySalesManager.LastCreateDate,
                Cost = energySalesManager.LastCost,
                RecipientNameShort = Config.RecipientNameShort
            };

            if (!preCheck.Print())
                Log.Error($"Ошибка печати чека.");
        }

        private void PrintMainCheck()
        {
            if (energySalesManager == null)
            {
                Log.Error($"Ошибка печати чека.");
                return;
            }

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

                Cost = energySalesManager.LastCost
            };

            if (!mainCheck.Print())
                Log.Error($"Ошибка печати чека.");
        }

		private void btnClear_Click(object sender, RoutedEventArgs e)
		{
        }

		private void miCashPrinterSettings_Click(object sender, RoutedEventArgs e)
		{
			checkPrinter.ShowProperties();
		}

        private void miCashPrinterCancelCheck_Click(object sender, RoutedEventArgs e)
        {
            checkPrinter.CancelCheck();
        }
    }
}
