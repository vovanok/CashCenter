using System;

namespace CashCenter.DataMigration.Providers.Excel.Entities
{
    public class EnergyPaymentGisHusModel
    {
        public int OrderNumber { get; private set; }
        public decimal Total { get; private set; }
        public DateTime Date { get; private set; }
        public DateTime PaymentPeriod { get; private set; }
        public string PaymentDocumentIdentifier { get; private set; }
        public string HusIdentifier { get; private set; }

        public EnergyPaymentGisHusModel(int orderNumber, decimal total, DateTime date,
            DateTime paymentPeriod, string paymentDocumentIdentifier, string husIdentifier)
        {
            OrderNumber = orderNumber;
            Total = total;
            Date = date;
            PaymentPeriod = paymentPeriod;
            PaymentDocumentIdentifier = paymentDocumentIdentifier;
            HusIdentifier = husIdentifier;
        }
    }
}
