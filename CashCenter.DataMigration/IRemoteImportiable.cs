using CashCenter.Dal;

namespace CashCenter.DataMigration
{
    public interface IRemoteImportiable
    {
        void Import(Department department);
    }
}