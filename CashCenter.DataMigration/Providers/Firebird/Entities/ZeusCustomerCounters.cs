namespace CashCenter.DataMigration.Providers.Firebird.Entities
{
    public class ZeusCustomerCounters
    {
        public int CustomerNumber { get; private set; }

        public int EndDayValue { get; private set; }

        public int EndNightValue { get; private set; }

        public bool IsTwoTariff { get; private set; }

        public ZeusCustomerCounters(int customerNumber, int endDayValue, int endNightValue, bool isTwoTariff)
        {
            CustomerNumber = customerNumber;
            EndDayValue = endDayValue;
            EndNightValue = endNightValue;
            IsTwoTariff = isTwoTariff;
        }
    }
}