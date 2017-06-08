using System.Windows;
using CashCenter.IvEnergySales.Common;

namespace CashCenter.IvEnergySales.Service
{
    public class DataMigrationViewModel : ViewModel
    {
        public Command Close { get; }

        public DataMigrationViewModel()
        {
            Close = new Command(CloseHandler);
        }

        private void CloseHandler(object param)
        {
            var window = param as Window;
            if (window != null)
            {
                window.DialogResult = false;
                window.Close();
            }
        }
    }
}
