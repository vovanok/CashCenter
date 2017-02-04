namespace CashCenter.ZeusDb.Entities
{
    public class Debt
    {
        public int CustomerId { get; private set; }

        public decimal Balance { get; private set; }

        public decimal Penalty { get; private set; }

        public Debt(int customerId, decimal balance, decimal penalty)
        {
            CustomerId = customerId;
            Balance = balance;
            Penalty = penalty;
        }
    }
}
