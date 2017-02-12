using CashCenter.Common.DbQualification;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CashCenter.IvEnergySales
{
    public partial class DepartmentSelectorControl : UserControl
    {
        public RegionDef Region { get; private set; }

        public DepartmentDef SelectedDepartment
        {
            get
            {
                if (cbDepartmentSelector.Items.Count == 0)
                    return null;

                return cbDepartmentSelector.SelectedValue as DepartmentDef;
            }
        }

        public DepartmentSelectorControl()
        {
            InitializeComponent();

            Region = QualifierManager.GetCurrentRegion();

            if (Region != null)
            {
                lblErrorMessage.Visibility = Visibility.Collapsed;

                cbDepartmentSelector.ItemsSource = (Region.Departments ?? new List<DepartmentDef>())
                    .Select(departmentDef => new { Department = departmentDef, DepartmentFullName = $"{departmentDef.Code} {departmentDef.Name}" });

                if (cbDepartmentSelector.Items.Count > 0)
                    cbDepartmentSelector.SelectedIndex = 0;
            }
            else
            {
                lblErrorMessage.Content = "Район не задан";
                lblErrorMessage.Foreground = Brushes.Red;
                cbDepartmentSelector.IsEnabled = false;

                lblErrorMessage.Visibility = Visibility.Visible;
            }
        }
    }
}
