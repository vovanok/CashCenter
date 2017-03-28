using CashCenter.Common;
using CashCenter.Dal;
using CashCenter.ZeusDb;
using System;

namespace CashCenter.DataMigration
{
    public abstract class BaseRemoteImporter<TSource, TTarget> : BaseImporter<TSource, TTarget>, IRemoteImportiable
    {
        protected Department sourceDepartment;
        protected ZeusDbController db;

        public void Import(Department department)
        {
            if (department == null)
            {
                Log.Error("Для импорта из БД не указано отделение.");
                return;
            }

            try
            {
                sourceDepartment = department;
                db = new ZeusDbController(department.Code, department.Url, department.Path);

                Import();
            }
            catch (Exception ex)
            {
                Log.ErrorWithException("Ошибка импортирования данных из удаленной БД", ex);
            }
        }
    }
}
