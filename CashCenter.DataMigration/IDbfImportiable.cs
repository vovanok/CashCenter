namespace CashCenter.DataMigration
{
    public interface IDbfImportiable
    {
        ImportResult Import(string dbfFilename);
    }
}
