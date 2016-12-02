using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;
using System.ComponentModel;
using CashCenter.IvEnergySales.DbQualification;

namespace CashCenter.IvEnergySales.Controls
{
    public partial class DbQualifierControl : UserControl
    {
        private DepartmentModel currentDepartment; 

        public DbQualifierControl()
        {
            InitializeComponent();
        }

        private void UserControl_Initialized(object sender, System.EventArgs e)
        {
#if DEBUG
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                return;
#endif
            currentDepartment = DbQualifier.GetCurrentDepartment();

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
