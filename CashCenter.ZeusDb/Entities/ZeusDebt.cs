namespace CashCenter.ZeusDb.Entities
{
    public class ZeusDebt
    {
        public int CustomerNumber { get; private set; }

        public decimal Balance { get; private set; }

        public decimal Penalty { get; private set; }

        public ZeusDebt(int customerNumber, decimal balance, decimal penalty)
        {
            CustomerNumber = customerNumber;
            Balance = balance;
            Penalty = penalty;
        }
    }
}
