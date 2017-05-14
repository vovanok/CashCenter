using CashCenter.Dal;

namespace CashCenter.DataMigration
{
    public interface IRemoteImporter
    {
        Department SourceDepartment { get; set; }
    }
}
