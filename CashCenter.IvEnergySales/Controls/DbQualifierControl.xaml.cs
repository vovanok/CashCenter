using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using CashCenter.IvEnergySales.DbQualification;
using CashCenter.IvEnergySales.Logging;

namespace CashCenter.IvEnergySales.Controls
{
    public partial class DbQualifierControl : UserControl
    {
        private DepartmentModel currentDepartment;

        public event Action<DbModelChangedEventArgs> OnDbModelChanged;

        public DbQualifierControl()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
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
            {
                cbDbSelector.SelectedIndex = 0;
                DbModelChanged();
            }
        }

        private void cbDbSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DbModelChanged();
        }

        private void DbModelChanged()
        {
            var selectedDbCode = cbDbSelector.SelectedValue as String;
            var newSelectedDbModel = currentDepartment.Dbs.FirstOrDefault(item => item.DbCode == selectedDbCode);
            if (newSelectedDbModel == null)
            {
                Log.Error($"Выбранной модели БД с кодом {selectedDbCode} не существует.");
                return;
            }

            if (OnDbModelChanged != null)
                OnDbModelChanged(new DbModelChangedEventArgs(newSelectedDbModel));
        }

        #region EventArgs
        public class DbModelChangedEventArgs : EventArgs
        {
            public DbModel NewDbModel { get; private set; }

            public DbModelChangedEventArgs(DbModel newDbModel)
            {
                NewDbModel = newDbModel;
            }
        }
        #endregion
    }
}
