using CashCenter.DataMigration.Providers.Dbf;
using System;
using System.IO;

namespace CashCenter.DataMigration
{
    public abstract class BaseDbfImporter<TSource, TTarget> : BaseImporter<TSource, TTarget>, IDbfImporter
        where TSource : class
        where TTarget : class
    {
        private const string TEMP_DBF_FILE_NAME = "dbfimp";

        protected DbfRegistryController dbfRegistry;

        public string DbfFilename { get; set; }

        public override ImportResult Import()
        {
            if (!File.Exists(DbfFilename))
                throw new ApplicationException("DBF файл не задан или не существует");

            var tempFilename = Path.Combine(Path.GetDirectoryName(DbfFilename), TEMP_DBF_FILE_NAME) + Path.GetExtension(DbfFilename);
            if (File.Exists(tempFilename))
                File.Delete(tempFilename);

            File.Copy(DbfFilename, tempFilename);

            dbfRegistry = new DbfRegistryController(tempFilename);

            var result = new ImportResult();

            try
            {
                return base.Import();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (File.Exists(tempFilename))
                    File.Delete(tempFilename);
            }
        }
    }
}
