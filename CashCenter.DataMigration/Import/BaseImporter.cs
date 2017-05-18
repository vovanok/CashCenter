using System.Collections.Generic;
using System.Linq;
using CashCenter.Dal;

namespace CashCenter.DataMigration.Import
{
    public abstract class BaseImporter<TSource, TTarget> : IImporter
        where TSource : class
        where TTarget : class
    {
        public virtual ImportResult Import()
        {
            var sourceItems = GetSourceItems();

            if (sourceItems == null)
                return new ImportResult();

            var addedCount = 0;
            var updatedCount = 0;
            var deletedCount = DeleteAllTargetItems();

            var itemsForCreation = sourceItems.Select(sourceItem =>
                {
                    if (TryUpdateExistingItem(sourceItem))
                    {
                        updatedCount++;
                        if (deletedCount > 0)
                            deletedCount--;
                        return null;
                    }

                    addedCount++;
                    return GetTargetItemBySource(sourceItem);
                }).Where(targetItem => targetItem != null);

            CreateNewItems(itemsForCreation);

            DalController.Instance.Save();

            return new ImportResult(addedCount, updatedCount, deletedCount);
        }

        protected abstract void CreateNewItems(IEnumerable<TTarget> itemsForCreation);

        protected abstract TTarget GetTargetItemBySource(TSource sourceItem);

        protected abstract bool TryUpdateExistingItem(TSource sourceItem);

        protected abstract IEnumerable<TSource> GetSourceItems();

        protected abstract int DeleteAllTargetItems();
    }
}
