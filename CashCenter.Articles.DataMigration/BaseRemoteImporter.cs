using CashCenter.Common.DbQualification;

namespace CashCenter.Articles.DataMigration
{
    public abstract class BaseRemoteImporter
    {
        public abstract void Import(DepartmentDef departmentDef);
    }
}
