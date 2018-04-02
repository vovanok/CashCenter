using System;

namespace CashCenter.DataMigration.Providers.Dbf.Entities
{
    public class DbfWaterCustomerPayment
    {
        public DateTime CreationDate { get; private set; }
        public int CustomerNumber { get; private set; }
        public decimal Cost { get; private set; }
        public string PeriodCode { get; private set; }
        public decimal Penalty { get; private set; }
        public string CounterNumber1 { get; private set; }
        public decimal CounterCost1 { get; private set; }
        public double CounterValue1 { get; private set; }
        public string CounterNumber2 { get; private set; }
        public decimal CounterCost2 { get; private set; }
        public double CounterValue2 { get; private set; }
        public string CounterNumber3 { get; private set; }
        public decimal CounterCost3 { get; private set; }
        public double CounterValue3 { get; private set; }
        public string CounterNumber4 { get; private set; }
        public decimal CounterCost4 { get; private set; }
        public double CounterValue4 { get; private set; }

        public DbfWaterCustomerPayment()
        {
        }

        public DbfWaterCustomerPayment(
            DateTime creationDate, int customerNumber, decimal cost,
            string periodCode, decimal penalty,
            string counterNumber1, decimal counterCost1, double counterValue1,
            string counterNumber2, decimal counterCost2, double counterValue2,
            string counterNumber3, decimal counterCost3, double counterValue3,
            string counterNumber4, decimal counterCost4, double counterValue4)
        {
            CreationDate = creationDate;
            CustomerNumber = customerNumber;
            Cost = cost;
            PeriodCode = periodCode ?? string.Empty;
            Penalty = penalty;
            CounterNumber1 = counterNumber1 ?? string.Empty;
            CounterCost1 = counterCost1;
            CounterValue1 = counterValue1;
            CounterNumber2 = counterNumber2 ?? string.Empty;
            CounterCost2 = counterCost2;
            CounterValue2 = counterValue2;
            CounterNumber3 = counterNumber3 ?? string.Empty;
            CounterCost3 = counterCost3;
            CounterValue3 = counterValue3;
            CounterNumber4 = counterNumber4 ?? string.Empty;
            CounterCost4 = counterCost4;
            CounterValue4 = counterValue4;
        }
    }
}