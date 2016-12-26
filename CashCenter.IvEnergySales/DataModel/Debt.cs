namespace CashCenter.IvEnergySales.DataModel
{
    public class Debt
    {
        public decimal Balance { get; private set; }

        public decimal Penalty { get; private set; }

        public Debt(decimal balance, decimal penalty)
        {
            Balance = balance;
            Penalty = penalty;
        }
    }
}
