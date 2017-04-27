using System;

namespace CashCenter.DataMigration.Providers.Word.Entities
{
    public class ReportCustomerPaymentModel
    {
        public int CustomerNumber { get; private set; }

        public int DayValue { get; private set; }

        public int NightValue { get; private set; }

        public decimal Cost { get; private set; }

        public DateTime CreationDate { get; private set; }

        public ReportCustomerPaymentModel(
            int customerNumber, int dayValue, int nightValue,
            decimal cost, DateTime creationDate)
        {
            CustomerNumber = customerNumber;
            DayValue = dayValue;
            NightValue = nightValue;
            Cost = cost;
            CreationDate = creationDate;
        }
    }
}