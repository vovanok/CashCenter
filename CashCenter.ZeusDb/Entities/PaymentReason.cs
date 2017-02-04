namespace CashCenter.ZeusDb.Entities
{
    public class PaymentReason
    {
        public int Id { get; private set; }

        public string Name { get; private set; }

        public bool IsCanPay { get; private set; }

        public PaymentReason(int id, string name, bool isCanPay)
        {
            Id = id;
            Name = name;
            IsCanPay = isCanPay;
        }
    }
}
