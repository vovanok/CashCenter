using System.Collections.Generic;
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

            var updatedCount = 0;
            var deletedCount = DeleteAllTargetItems();

            var itemsForCreation = new List<TTarget>();
            foreach (var sourceItem in sourceItems)
            {
                if (TryUpdateExistingItem(sourceItem))
                {
                    updatedCount++;
                    if (deletedCount > 0)
                        deletedCount--;
                    continue;
                }

                var targetItem = GetTargetItemBySource(sourceItem);
                if (targetItem != null)
                    itemsForCreation.Add(targetItem);
            }

            var addedCount = itemsForCreation.Count;

            CreateNewItems(itemsForCreation);

            return new ImportResult(addedCount, updatedCount, deletedCount);
        }

        protected abstract void CreateNewItems(IEnumerable<TTarget> itemsForCreation);

        protected abstract TTarget GetTargetItemBySource(TSource sourceItem);

        protected abstract bool TryUpdateExistingItem(TSource sourceItem);

        protected abstract IEnumerable<TSource> GetSourceItems();

        protected abstract int DeleteAllTargetItems();
    }
}
