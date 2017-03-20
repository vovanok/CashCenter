using CashCenter.Common.DbQualification;

namespace CashCenter.DataMigration
{
    public abstract class BaseRemoteImporter
    {
        public abstract void Import(DepartmentDef departmentDef);
    }
}
