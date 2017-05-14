namespace CashCenter.DataMigration
{
    public class ExportResult
    {
        public int SuccessCount { get; private set; }
        public int FailCount { get; private set; }

        public ExportResult(int successCount, int failCount)
        {
            SuccessCount = successCount;
            FailCount = failCount;
        }
    }
}
