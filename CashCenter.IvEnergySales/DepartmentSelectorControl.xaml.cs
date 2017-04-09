using CashCenter.Common;
using CashCenter.Dal;
using System;
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

        public event SelectionChangedEventHandler DepartmentChanged
        {
            add { cbDepartmentSelector.SelectionChanged += value; }
            remove { cbDepartmentSelector.SelectionChanged -= value; }
        }

        public DepartmentSelectorControl()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Region = DalController.Instance.Regions.FirstOrDefault(region => region.Id == Config.CurrentRegionId);
                if (Region != null)
                {
                    lblRegionName.Content = Region.Name ?? string.Empty;

                    cbDepartmentSelector.ItemsSource = Region.Departments
                        .Select(department => new { Department = department, DepartmentFullName = $"{department.Code} {department.Name}" });

                    if (cbDepartmentSelector.Items.Count > 0)
                        cbDepartmentSelector.SelectedIndex = 0;
                }
                else
                {
                    lblRegionName.Content = "Район не задан";
                    lblRegionName.Foreground = Brushes.Red;
                    cbDepartmentSelector.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                Log.ErrorWithException("Ошибка загрузки департаметов.", ex);
            }
        }
    }
}
