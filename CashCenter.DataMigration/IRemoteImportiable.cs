using CashCenter.Dal;

namespace CashCenter.DataMigration
{
    public interface IRemoteImportiable
    {
        ImportResult Import(Department department);
    }
}