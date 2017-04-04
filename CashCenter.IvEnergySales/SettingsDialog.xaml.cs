using CashCenter.Common;
using CashCenter.Dal;
using System.Linq;
using System.Windows;

namespace CashCenter.IvEnergySales
{
    public partial class SettingsDialog : Window
    {
        public SettingsDialog()
        {
            InitializeComponent();

            tbCasherName.Text = Properties.Settings.Default.CasherName;
            dgDepartments.ItemsSource = DalController.Instance.Departments
                .Where(department => department.RegionId == Config.CurrentRegionId).ToList();
        }

        private void On_btnSave_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.CasherName = tbCasherName.Text;
            Properties.Settings.Default.Save();

            DalController.Instance.Save();

            DialogResult = true;
            Close();
        }

        private void On_btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
