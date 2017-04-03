using System;
using System.Collections.Generic;

namespace CashCenter.DataMigration
{
    public abstract class BaseExporter<TSource>
    {
        public ExportResult Export(DateTime beginDatetime, DateTime endDatetime)
        {
            var itemsForExport = GetSourceItems(beginDatetime, endDatetime);

            var successCount = 0;
            var failCount = 0;

            foreach (var customerPayment in itemsForExport)
            {
                if (TryExportOneItem(customerPayment))
                    successCount++;
                else
                    failCount++;
            }

            return new ExportResult(successCount, failCount);
        }

        protected abstract List<TSource> GetSourceItems(DateTime beginDatetime, DateTime endDatetime);

        protected abstract bool TryExportOneItem(TSource item);
    }
}
