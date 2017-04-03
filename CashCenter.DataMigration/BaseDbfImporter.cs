using CashCenter.Common;
using CashCenter.DbfRegistry;
using System;
using System.IO;

namespace CashCenter.DataMigration
{
    public abstract class BaseDbfImporter<TSource, TTarget> : BaseImporter<TSource, TTarget>, IDbfImportiable
        where TSource : class
        where TTarget : class
    {
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

            dbfRegistry = new DbfRegistryController(dbfFilename);

            try
            {
                return Import();
            }
            catch (Exception ex)
            {
                Log.ErrorWithException("Ошибка импортирования данных из DBF файла", ex);
                return new ImportResult();
            }
        }
    }
}
