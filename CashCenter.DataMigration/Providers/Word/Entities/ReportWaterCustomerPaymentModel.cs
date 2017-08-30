using System;

namespace CashCenter.DataMigration.Providers.Word.Entities
{
    public class ReportWaterCustomerPaymentModel
    {
        public DateTime CreationDateTime { get; private set; }
        public int CustomerNumber { get; private set; }
        public double CounterValue1 { get; private set; }
        public double CounterValue2 { get; private set; }
        public double CounterValue3 { get; private set; }
        public decimal PaymentCost { get; private set; }
        public decimal PenaltyCost { get; private set; }
        public decimal PaymentAndPenaltyCost { get; private set; }
        public decimal ComissionCost { get; private set; }
        public decimal TotalCost { get; private set; }

        public ReportWaterCustomerPaymentModel(DateTime creationDateTime, int customerNumber,
            double counterValue1, double counterValue2, double counterValue3,
            decimal paymentCost, decimal penaltyCost, decimal paymentAndPenaltyCost, decimal comissionCost, decimal totalCost)
        {
            CreationDateTime = creationDateTime;
            CustomerNumber = customerNumber;
            CounterValue1 = counterValue1;
            CounterValue2 = counterValue2;
            CounterValue3 = counterValue3;
            PaymentCost = paymentCost;
            PenaltyCost = penaltyCost;
            PaymentAndPenaltyCost = paymentAndPenaltyCost;
            ComissionCost = comissionCost;
            TotalCost = totalCost;
        }
    }
}
