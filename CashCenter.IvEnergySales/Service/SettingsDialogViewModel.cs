using System.Linq;
using System.Windows;
using System.Collections.Generic;
using CashCenter.Common;
using CashCenter.IvEnergySales.Common;
using CashCenter.Dal;

namespace CashCenter.IvEnergySales.Service
{
    public class SettingsDialogViewModel : ViewModel
    {
        public Observed<string> CashierName { get; } = new Observed<string>();
        public List<Department> Deparments { get; }

        public Command SaveCommand { get; }
        public Command CloseCommand { get; }

        public SettingsDialogViewModel()
        {
            CashierName.OnChange += (newValue) => DispatchPropertyChanged("CashierName");

            CashierName.Value = Properties.Settings.Default.CasherName;

            Deparments = DalController.Instance.Departments
                .Where(department => department.RegionId == Config.CurrentRegionId).ToList();

            SaveCommand = new Command(SaveHandler);
            CloseCommand = new Command(CloseHandler);
        }

        private void SaveHandler(object data)
        {
            Properties.Settings.Default.CasherName = CashierName.Value;
            Properties.Settings.Default.Save();

            DalController.Instance.Save();
            GlobalEvents.DispatchDepartmentsChanged();

            var window = data as Window;
            if (window != null)
            {
                window.DialogResult = true;
                window.Close();
            }
        }

        private void CloseHandler(object data)
        {
            DalController.Instance.DetachDepartmentsChanges();

            var window = data as Window;
            if (window != null)
            {
                window.DialogResult = false;
                window.Close();
            }
        }
    }
}
