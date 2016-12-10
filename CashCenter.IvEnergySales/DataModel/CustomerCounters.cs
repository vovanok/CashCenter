using CashCenter.IvEnergySales.DAL;

namespace CashCenter.IvEnergySales.DataModel
{
    public class CustomerCounters
    {
        public DbController Db { get; private set; }

        public int EndDayValue { get; private set; }

        public int EndNightValue { get; private set; }

        public bool IsTwoTariff { get; private set; }

        public CustomerCounters(DbController sourceDb, int endDayValue, int endNightValue, bool isTwoTariff)
        {
            Db = sourceDb;
            EndDayValue = endDayValue;
            EndNightValue = endNightValue;
            IsTwoTariff = isTwoTariff;
        }
    }
}
