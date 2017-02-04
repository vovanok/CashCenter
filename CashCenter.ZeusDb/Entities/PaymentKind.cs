namespace CashCenter.ZeusDb.Entities
{
    public class PaymentKind
    {
        public int Id { get; private set; }

        public string Name { get; private set; }

        public PaymentKind(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
