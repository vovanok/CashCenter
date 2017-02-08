using System.Windows;

namespace CashCenter.IvEnergySales
{
    public partial class DataMigrationDialog : Window
    {
        public DataMigrationDialog()
        {
            InitializeComponent();
        }

        private void btmImportArticles_Click(object sender, RoutedEventArgs e)
        {
            // load from dbf registry entities and use dal for record
        }

        private void On_btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}