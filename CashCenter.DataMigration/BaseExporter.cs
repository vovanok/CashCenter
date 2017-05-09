using System;
using System.Collections.Generic;

namespace CashCenter.DataMigration
{
    public abstract class BaseExporter<TSource> : IExporter
    {
        protected DateTime beginDatetime;
        protected DateTime endDatetime;

        public ExportResult Export(DateTime beginDatetime, DateTime endDatetime)
        {
            this.beginDatetime = beginDatetime;
            this.endDatetime = endDatetime;

            var itemsForExport = GetSourceItems(beginDatetime, endDatetime);

            var successCount = TryExportItems(itemsForExport);
            var failCount = itemsForExport.Count - successCount;

            return new ExportResult(successCount, failCount);
        }

        protected abstract List<TSource> GetSourceItems(DateTime beginDatetime, DateTime endDatetime);

        protected abstract int TryExportItems(IEnumerable<TSource> items);
    }
}
