using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using CashCenter.IvEnergySales.DbQualification;

namespace CashCenter.IvEnergySales
{
    public partial class MainWindow : Window
	{
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
            var currentDepartment = DbQualifier.GetCurrentDepartment();

            if (!string.IsNullOrEmpty(currentDepartment.Name))
            {
                lblDepartmentName.Content = currentDepartment.Name;
            }
            else
            {
                lblDepartmentName.Content = "Отделение не задано";
                lblDepartmentName.Foreground = Brushes.Red;
            }

            cbDbSelector.ItemsSource = currentDepartment.Dbs ?? new List<DbModel>();

            if (cbDbSelector.Items.Count > 0)
                cbDbSelector.SelectedIndex = 0;
        }
    }
}
