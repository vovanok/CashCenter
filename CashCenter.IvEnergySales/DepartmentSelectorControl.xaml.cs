using CashCenter.Common;
using CashCenter.Dal;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CashCenter.IvEnergySales
{
    public partial class DepartmentSelectorControl : UserControl
    {
        public Region Region { get; private set; }

        public Department SelectedDepartment
        {
            get
            {
                if (cbDepartmentSelector.Items.Count == 0)
                    return null;

                return cbDepartmentSelector.SelectedValue as Department;
            }
        }

        public DepartmentSelectorControl()
        {
            InitializeComponent();

            Region = DalController.Instance.Regions.FirstOrDefault(region => region.Id == Config.CurrentRegionId);
            if (Region != null)
            {
                lblErrorMessage.Visibility = Visibility.Collapsed;

                cbDepartmentSelector.ItemsSource = Region.Departments
                    .Select(department => new { Department = department, DepartmentFullName = $"{department.Code} {department.Name}" });

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
