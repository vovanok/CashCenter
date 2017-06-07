using System;

namespace CashCenter.IvEnergySales.Common
{
    public static class GlobalEvents
    {
        public static event Action OnDepartmentsChanged;

        public static void DispatchDepartmentsChanged()
        {
            if (OnDepartmentsChanged != null)
                OnDepartmentsChanged();
        }
    }
}
