using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CashCenter.IvEnergySales.Common
{
    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void DispatchPropertyChanged(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                return;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool IsValid(DependencyObject depObj)
        {
            if (depObj == null)
                return false;

            return !Validation.GetHasError(depObj) &&
                LogicalTreeHelper.GetChildren(depObj)
                    .OfType<DependencyObject>()
                    .All(IsValid);
        }
    }
}
