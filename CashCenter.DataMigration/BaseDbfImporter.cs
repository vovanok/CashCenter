using CashCenter.Common;
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
            if (string.IsNullOrEmpty(dbfFilename))
            {
                Log.Error("Путь в файлу DBF для импорта не задан.");
                return new ImportResult();
            }

            if (!File.Exists(dbfFilename))
            {
                Log.Error($"Заданный DBF файл для импорта не существует ({dbfFilename}).");
                return new ImportResult();
            }

            var tempFilename = Path.Combine(Path.GetDirectoryName(dbfFilename), TEMP_DBF_FILE_NAME) + Path.GetExtension(dbfFilename);
            if (File.Exists(tempFilename))
                File.Delete(tempFilename);

            File.Copy(dbfFilename, tempFilename);

            dbfRegistry = new DbfRegistryController(tempFilename);

            var result = new ImportResult();

            try
            {
                result = Import();
            }
            catch (Exception ex)
            {
                Log.ErrorWithException("Ошибка импортирования данных из DBF файла", ex);
            }

            if (File.Exists(tempFilename))
                File.Delete(tempFilename);

            return result;
        }
    }
}
