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

            return TryExportItems(itemsForExport);
        }

        protected abstract List<TSource> GetSourceItems(DateTime beginDatetime, DateTime endDatetime);

        protected abstract ExportResult TryExportItems(IEnumerable<TSource> items);
    }
}
