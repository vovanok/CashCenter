using CashCenter.Dal;

namespace CashCenter.DataMigration
{
    public abstract class BaseRemoteImporter
    {
        public abstract void Import(Department department);
    }
}
