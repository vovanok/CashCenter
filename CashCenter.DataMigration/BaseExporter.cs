using System;

namespace CashCenter.DataMigration
{
    public abstract class BaseExporter
    {
        public abstract void Export(DateTime beginDatetime, DateTime endDatetime);
    }
}
