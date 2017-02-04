namespace CashCenter.ZeusDb.Entities
{
    public class CustomerCounters
    {
        public int CustomerId { get; private set; }

        public int EndDayValue { get; private set; }

        public int EndNightValue { get; private set; }

        public bool IsTwoTariff { get; private set; }

        public CustomerCounters(int customerId, int endDayValue, int endNightValue, bool isTwoTariff)
        {
            CustomerId = customerId;
            EndDayValue = endDayValue;
            EndNightValue = endNightValue;
            IsTwoTariff = isTwoTariff;
        }
    }
}