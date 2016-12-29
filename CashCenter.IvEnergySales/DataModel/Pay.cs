namespace CashCenter.IvEnergySales.DataModel
{
    public class Pay
    {
        public int Id { get; private set; }

        public int CustomerId { get; private set; }

        public int ReasonId { get; private set; }

        public int? MetersId { get; private set; }

        public int JournalId { get; private set; }

        public decimal Cost { get; private set; }

        public decimal PenaltyTotal { get; private set; }

        public string Description { get; private set; }

        public Pay(int id, int customerId, int reasonId, int? metersId, int journalId,
            decimal cost, decimal penaltyTotal, string description)
        {
            Id = id;
            CustomerId = customerId;
            ReasonId = reasonId;
            MetersId = metersId;
            JournalId = journalId;
            Cost = cost;
            PenaltyTotal = penaltyTotal;
            Description = description;
        }

        public Pay(int customerId, int reasonId, int? metersId, int journalId,
                decimal cost, decimal penaltyTotal, string description)
            : this(-1, customerId, reasonId, metersId, journalId, cost, penaltyTotal, description)
        {
        }
    }
}
