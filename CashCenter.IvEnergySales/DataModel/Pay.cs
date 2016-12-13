namespace CashCenter.IvEnergySales.DataModel
{
    public class Pay
    {
        public Customer Customer { get; private set; }

        public int ReasonId { get; private set; }

        public PayJournal Journal { get; private set; }

        public decimal Cost { get; private set; }

        public string Description { get; private set; }

        public Pay(Customer customer, int reasonId, PayJournal journal,
            decimal cost, string description)
        {
            Customer = customer;
            ReasonId = reasonId;
            Journal = journal;
            Cost = cost;
            Description = description;
        }
    }
}
