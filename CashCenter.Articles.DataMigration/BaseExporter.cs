using System;

namespace CashCenter.Articles.DataMigration
{
    public abstract class BaseExporter
    {
        public abstract void Export(DateTime beginDatetime, DateTime endDatetime);
    }
}
