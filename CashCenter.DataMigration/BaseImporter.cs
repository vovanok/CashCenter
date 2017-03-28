using CashCenter.Dal;
using System.Collections.Generic;
using System.Linq;

namespace CashCenter.DataMigration
{
    public abstract class BaseImporter<TSource, TTarget>
    {
        public void Import()
        {
            var sourceItems = GetSourceItems();

            if (sourceItems == null)
                return;

            DeleteAllTargetItems();

            var itemsForCreation = sourceItems.Select(sourceItem =>
                {
                    if (!TryUpdateExistingItem(sourceItem))
                        return GetTargetItemBySource(sourceItem);

                    return default(TTarget);
                }).Where(targetItem => !targetItem.Equals(default(TTarget)));

            CreateNewItems(itemsForCreation);

            DalController.Instance.Save();
        }

        protected abstract void CreateNewItems(IEnumerable<TTarget> itemsForCreation);

        protected abstract TTarget GetTargetItemBySource(TSource sourceItem);

        protected abstract bool TryUpdateExistingItem(TSource sourceItem);

        protected abstract IEnumerable<TSource> GetSourceItems();

        protected abstract void DeleteAllTargetItems();
    }
}
