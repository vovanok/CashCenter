using NetOffice.WordApi;

namespace CashCenter.DataMigration.Providers.Word
{
    public interface IWordReport
    {
        void ExportToDocument(Document wordDocument);
    }
}
