using System.Collections.Generic;

namespace CashCenter.DataMigration.Providers.Word.Entities
{
    public class ReportWaterCustomersModel
    {
        public double CommisionValue { get; private set; }
        public IEnumerable<ReportWaterCustomerPaymentModel> WaterCustomersPayments { get; private set; }
        public double TotalCounterValue1 { get; private set; }
        public double TotalCounterValue2 { get; private set; }
        public double TotalCounterValue3 { get; private set; }
        public decimal TotalPaymentCost { get; private set; }
        public decimal TotalPenaltyCost { get; private set; }
        public decimal TotalPaymentAndPenaltyCost { get; private set; }
        public decimal TotalComissionCost { get; private set; }
        public decimal SummaryTotalCost { get; private set; }

        public ReportWaterCustomersModel(double commisionValue, IEnumerable<ReportWaterCustomerPaymentModel> waterCustomersPayments)
        {
            CommisionValue = commisionValue;
            WaterCustomersPayments = waterCustomersPayments ?? new List<ReportWaterCustomerPaymentModel>();
            foreach (var waterCustomerPayment in WaterCustomersPayments)
            {
                TotalCounterValue1 += waterCustomerPayment.CounterValue1;
                TotalCounterValue2 += waterCustomerPayment.CounterValue2;
                TotalCounterValue3 += waterCustomerPayment.CounterValue3;
                TotalPaymentCost += waterCustomerPayment.PaymentCost;
                TotalPenaltyCost += waterCustomerPayment.PenaltyCost;
                TotalPaymentAndPenaltyCost += waterCustomerPayment.PaymentAndPenaltyCost;
                TotalComissionCost += waterCustomerPayment.ComissionCost;
                SummaryTotalCost += waterCustomerPayment.TotalCost;
            }
        }
    }
}
