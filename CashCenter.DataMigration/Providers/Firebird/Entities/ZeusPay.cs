namespace CashCenter.DataMigration.Providers.Firebird.Entities
{
    public class ZeusPay
    {
        public int Id { get; private set; }
        public int CustomerNumber { get; private set; }
        public int ReasonId { get; private set; }
        public int? MetersId { get; private set; }
        public int JournalId { get; private set; }
        public decimal Cost { get; private set; }
        public decimal PenaltyTotal { get; private set; }
        public string Description { get; private set; }

        public ZeusPay(int id, int customerNumber, int reasonId, int? metersId, int journalId,
            decimal cost, decimal penaltyTotal, string description)
        {
            Id = id;
            CustomerNumber = customerNumber;
            ReasonId = reasonId;
            MetersId = metersId;
            JournalId = journalId;
            Cost = cost;
            PenaltyTotal = penaltyTotal;
            Description = description;
        }

        public ZeusPay(int customerNumber, int reasonId, int? metersId, int journalId,
                decimal cost, decimal penaltyTotal, string description)
            : this(-1, customerNumber, reasonId, metersId, journalId, cost, penaltyTotal, description)
        {
        }
    }
}
