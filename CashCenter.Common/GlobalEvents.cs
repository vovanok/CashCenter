using System;

namespace CashCenter.Common
{
    public static class GlobalEvents
    {
        public static event Action OnDepartmentsChanged;
        public static event Action OnArticlesUpdated;
        public static event Action OnWaterComissionPercentChanged;
        public static event Action OnArticlesManipulatorTypeChanged;
        public static event Action OnArticlesZeusDbUrlChanged;
        public static event Action OnArticlesZeusDbPathChanged;

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

        public static void DispatchWaterComissionPercentChanged()
        {
            if (OnWaterComissionPercentChanged != null)
                OnWaterComissionPercentChanged();
        }

        public static void DispatchArticlesManipulatorTypeChanged()
        {
            if (OnArticlesManipulatorTypeChanged != null)
                OnArticlesManipulatorTypeChanged();
        }

        public static void DispatchArticlesZeusDbUrlChanged()
        {
            if (OnArticlesZeusDbUrlChanged != null)
                OnArticlesZeusDbUrlChanged();
        }

        public static void DispatchArticlesZeusDbPathChanged()
        {
            if (OnArticlesZeusDbPathChanged != null)
                OnArticlesZeusDbPathChanged();
        }
    }
}
