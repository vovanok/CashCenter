using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.ComponentModel;
using CashCenter.Dal;
using CashCenter.Common;
using CashCenter.ViewCommon;

namespace CashCenter.IvEnergySales
{
    public partial class DepartmentSelectorControl : UserControl
    {
        public Region Region { get; private set; }

        public static readonly DependencyProperty SelectedDepartmentProperty =
            DependencyProperty.Register("SelectedDepartment", typeof(Department),
            typeof(DepartmentSelectorControl), new FrameworkPropertyMetadata(null));

        public Department SelectedDepartment
        {
            get { return (Department)GetValue(SelectedDepartmentProperty); }
            set { SetValue(SelectedDepartmentProperty, value); }
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
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                return;

            try
            {
                Region = DalController.Instance.Regions.FirstOrDefault(region => region.Id == Config.CurrentRegionId);
                if (Region != null)
                {
                    lblRegionName.Content = Region.Name ?? string.Empty;

                    UpdateDepartments();
                    GlobalEvents.OnDepartmentsChanged += UpdateDepartments;
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
                var message = "Ошибка загрузки департаметов.";
                Log.Error(message, ex);
                Message.Error(message);
            }
        }

        private void On_cbDepartmentSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedDepartment = cbDepartmentSelector.SelectedValue as Department;
        }

        private void UpdateDepartments()
        {
            var lastSelectedIndex = cbDepartmentSelector.SelectedIndex;

            cbDepartmentSelector.ItemsSource = Region.Departments
                .Select(department => new { Department = department, DepartmentFullName = $"{department.Code} {department.Name}" });

            if (lastSelectedIndex > 0)
                cbDepartmentSelector.SelectedIndex = lastSelectedIndex;
            else if (cbDepartmentSelector.Items.Count > 0)
                cbDepartmentSelector.SelectedIndex = 0;
        }
    }
}
