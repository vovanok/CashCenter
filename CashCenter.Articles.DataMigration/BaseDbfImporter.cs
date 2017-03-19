namespace CashCenter.Articles.DataMigration
{
    public abstract class BaseDbfImporter
    {
        protected const string IMPORT_ERROR_PREFIX = "Ошибка импортирования данных.";

        public abstract void Import(string dbfFilename);
    }
}
