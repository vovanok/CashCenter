using CashCenter.Common;
using CashCenter.Dal;
using CashCenter.DataMigration.Providers.Firebird;
using System;

namespace CashCenter.DataMigration
{
    public abstract class BaseRemoteImporter<TSource, TTarget> : BaseImporter<TSource, TTarget>, IRemoteImportiable
        where TSource : class
        where TTarget : class
    {
        protected Department sourceDepartment;
        protected ZeusDbController db;

        public ImportResult Import(Department department)
        {
            if (department == null)
            {
                Log.Error("Для импорта из БД не указано отделение.");
                return new ImportResult();
            }

            try
            {
                sourceDepartment = department;
                db = new ZeusDbController(department.Code, department.Url, department.Path);

                return Import();
            }
            catch (Exception ex)
            {
                Log.ErrorWithException("Ошибка импортирования данных из удаленной БД", ex);
                return new ImportResult();
            }
        }
    }
}
