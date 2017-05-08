using CashCenter.DataMigration.Providers.Dbf;
using System;
using System.IO;

namespace CashCenter.DataMigration
{
    public abstract class BaseDbfImporter<TSource, TTarget> : BaseImporter<TSource, TTarget>, IDbfImportiable
        where TSource : class
        where TTarget : class
    {
        private const string TEMP_DBF_FILE_NAME = "dbfimp";

        protected DbfRegistryController dbfRegistry;

        public ImportResult Import(string dbfFilename)
        {
            if (!File.Exists(dbfFilename))
                throw new ApplicationException("DBF файл не задан или не существует");

            var tempFilename = Path.Combine(Path.GetDirectoryName(dbfFilename), TEMP_DBF_FILE_NAME) + Path.GetExtension(dbfFilename);
            if (File.Exists(tempFilename))
                File.Delete(tempFilename);

            File.Copy(dbfFilename, tempFilename);

            dbfRegistry = new DbfRegistryController(tempFilename);

            var result = new ImportResult();

            try
            {
                return Import();
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
