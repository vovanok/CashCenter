namespace CashCenter.DataMigration
{
    public interface IDbfImportiable
    {
        void Import(string dbfFilename);
    }
}
