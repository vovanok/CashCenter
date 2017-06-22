using System;

namespace CashCenter.IvEnergySales.Common
{
    public static class GlobalEvents
    {
        public static event Action OnDepartmentsChanged;
        public static event Action OnArticlesUpdated;

        public static void DispatchDepartmentsChanged()
        {
            if (OnDepartmentsChanged != null)
                OnDepartmentsChanged();
        }

        public static void DispatchArticlesUpdated()
        {
            if (OnArticlesUpdated != null)
                OnArticlesUpdated();
        }
    }
}
