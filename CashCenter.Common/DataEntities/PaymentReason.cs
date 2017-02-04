namespace CashCenter.Common.DataEntities
{
    public class PaymentReason
    {
        public int Id { get; private set; }
        public string Name { get; private set; }

        public PaymentReason(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
