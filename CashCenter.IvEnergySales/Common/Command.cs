using System;
using System.Windows.Input;

namespace CashCenter.IvEnergySales.Common
{
    public class Command : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private Action<object> executePredicate;
        private Func<object, bool> isCanExecutePredicate;

        public Command(Action<object> executePredicate)
        {
            this.executePredicate = executePredicate;
        }

        public Command(Action<object> executePredicate, Func<object, bool> isCanExecutePredicate)
            : this(executePredicate)
        {
            this.isCanExecutePredicate = isCanExecutePredicate;
        }

        public bool CanExecute(object parameter)
        {
            if (isCanExecutePredicate != null)
                return isCanExecutePredicate(parameter);

            return true;
        }

        public void Execute(object parameter)
        {
            executePredicate?.Invoke(parameter);
        }
    }
}
