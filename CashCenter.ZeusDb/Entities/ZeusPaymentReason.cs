namespace CashCenter.ZeusDb.Entities
{
    public class ZeusPaymentReason
    {
        public int Id { get; private set; }

        public string Name { get; private set; }

        public bool IsCanPay { get; private set; }

        public ZeusPaymentReason(int id, string name, bool isCanPay)
        {
            Id = id;
            Name = name;
            IsCanPay = isCanPay;
        }
    }
}
