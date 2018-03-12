using CashCenter.Common;

namespace CashCenter.ViewCommon
{
    public class ViewProperty<T> : Observed<T>
    {
        public ViewProperty(string propertyName, ViewModel viewModel)
        {
            if (string.IsNullOrEmpty(propertyName) || viewModel == null)
                return;

            OnChange += (newValue) => viewModel.DispatchPropertyChanged(propertyName);
        }
    }
}
