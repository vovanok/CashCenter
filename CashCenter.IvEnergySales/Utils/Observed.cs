using System;

namespace CashCenter.IvEnergySales.Utils
{
    public class Observed<T>
    {
        private object locker = new object();

        private T value;

        public T Value
        {
            get
            {
                return value;
            }
            set
            {
                lock(locker)
                {
                    this.value = value;
                    
                }

                if (OnChange != null)
                    OnChange(value);
            }
        }

        public event Action<T> OnChange;
    }
}
