using System;

namespace CashCenter.IvEnergySales.BusinessLogic
{
    public class InfoForCheck
    {
        public decimal Cost { get; private set; }

        public DateTime Date { get; private set; }

        public InfoForCheck(decimal cost, DateTime date)
        {
            Cost = cost;
            Date = date;
        }
    }
}
