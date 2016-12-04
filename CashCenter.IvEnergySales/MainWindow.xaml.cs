using CashCenter.IvEnergySales.DAL;
using System.Windows;

namespace CashCenter.IvEnergySales
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
        private DbController dbController;

		public MainWindow()
		{
			InitializeComponent();
		}

        private void DbQualifierControl_OnDbModelChanged(Controls.DbQualifierControl.DbModelChangedEventArgs args)
        {
            dbController = new DbController(args.NewDbModel);
        }
    }
}
