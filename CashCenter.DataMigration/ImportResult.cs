namespace CashCenter.DataMigration
{
    public class ImportResult
    {
        public int AddedCount { get; private set; }

        public int UpdatedCount { get; private set; }

        public int DeletedCount { get; private set; }

        public ImportResult() : this(0, 0, 0)
        {
        }

        public ImportResult(int addedCount, int updatedCount, int deletedCount)
        {
            AddedCount = addedCount;
            UpdatedCount = updatedCount;
            DeletedCount = deletedCount;
        }
    }
}