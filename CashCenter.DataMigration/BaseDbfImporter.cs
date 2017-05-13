using CashCenter.Common;
using CashCenter.DataMigration.Providers.Dbf;

namespace CashCenter.DataMigration
{
    public abstract class BaseDbfImporter<TSource, TTarget> : BaseImporter<TSource, TTarget>, IDbfImporter
        where TSource : class
        where TTarget : class
    {
        protected DbfRegistryController dbfRegistry;

        public string DbfFilename { get; set; }

        public override ImportResult Import()
        {
            using (var fileBuffer = new FileBuffer(DbfFilename, FileBuffer.BufferType.Read))
            {
                dbfRegistry = new DbfRegistryController(fileBuffer.BufferFilename);
                return base.Import();
            }
        }
    }
}
