using System;

namespace CashCenter.WordReport
{
    public class ReportCustomerPaymentModel
    {
        public int Number { get; private set; }

        public int DayValue { get; private set; }

        public int NightValue { get; private set; }

        public decimal Cost { get; private set; }

        public DateTime CreationDate { get; private set; }

        public ReportCustomerPaymentModel(
            int number, int dayValue, int nightValue, decimal cost, DateTime creationDate)
        {
            Number = number;
            DayValue = dayValue;
            NightValue = nightValue;
            Cost = cost;
            CreationDate = creationDate;
        }
    }
}