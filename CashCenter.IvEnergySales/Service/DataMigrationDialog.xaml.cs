using System.Windows;

namespace CashCenter.IvEnergySales
{
    public partial class DataMigrationDialog : Window
    {
        public DataMigrationDialog()
        {
            InitializeComponent();
        }

        private void On_btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}