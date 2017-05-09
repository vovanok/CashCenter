using System;

namespace CashCenter.DataMigration
{
    public interface IExporter
    {
        ExportResult Export(DateTime beginDatetime, DateTime endDatetime);
    }
}
