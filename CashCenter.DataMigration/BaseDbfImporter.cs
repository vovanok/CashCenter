using CashCenter.Common;
using CashCenter.DbfRegistry;
using System;
using System.IO;

namespace CashCenter.DataMigration
{
    public abstract class BaseDbfImporter<TSource, TTarget> : BaseImporter<TSource, TTarget>, IDbfImportiable
    {
        protected DbfRegistryController dbfRegistry;

        public void Import(string dbfFilename)
        {
            if (string.IsNullOrEmpty(dbfFilename))
            {
                Log.Error("Путь в файлу DBF для импорта не задан.");
                return;
            }

            if (!File.Exists(dbfFilename))
            {
                Log.Error($"Заданный DBF файл для импорта не существует ({dbfFilename}).");
                return;
            }

            dbfRegistry = new DbfRegistryController(dbfFilename);

            try
            {
                Import();
            }
            catch (Exception ex)
            {
                Log.ErrorWithException("Ошибка импортирования данных из DBF файла", ex);
            }
        }
    }
}
