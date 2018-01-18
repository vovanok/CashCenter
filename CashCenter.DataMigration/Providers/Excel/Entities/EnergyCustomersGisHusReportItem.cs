using System;

namespace CashCenter.DataMigration.Providers.Excel.Entities
{
    public class EnergyCustomersGisHusReportItem
    {
        public int OrderNumber { get; private set; }
        public string PaymentIdentifier { get; private set; }
        public decimal Total { get; private set; }
        public DateTime Date { get; private set; }
        public DateTime PaymentPeriod { get; private set; }
        public string PaymentDocumentIdentifier { get; private set; }
        public string HusIdentifier { get; private set; }
        public string UnifiedAccount { get; private set; }
        public string PerformerInn { get; private set; }
        public string PerformerName { get; private set; }
        public string PerformerKpp { get; private set; }
        public string ReceiverInn { get; private set; }
        public string ReceiverName { get; private set; }
        public string ReceiverBankName { get; private set; }
        public string ReceiverBankBik { get; private set; }
        public string ReceiverAccount { get; private set; }

        public EnergyCustomersGisHusReportItem(int orderNumber, string paymentIdentifier, decimal total, DateTime date,
            DateTime paymentPeriod, string paymentDocumentIdentifier, string husIdentifier, string unifiedAccount,
            string performerInn, string performerName, string performerKpp, string receiverInn, string receiverName,
            string receiverBankName, string receiverBankBik, string receiverAccount)
        {
            OrderNumber = orderNumber;
            PaymentIdentifier = paymentIdentifier ?? string.Empty;
            Total = total;
            Date = date;
            PaymentPeriod = paymentPeriod;
            PaymentDocumentIdentifier = paymentDocumentIdentifier ?? string.Empty;
            HusIdentifier = husIdentifier ?? string.Empty;
            UnifiedAccount = unifiedAccount ?? string.Empty;
            PerformerInn = performerInn ?? string.Empty;
            PerformerName = performerName ?? string.Empty;
            PerformerKpp = performerKpp ?? string.Empty;
            ReceiverInn = receiverInn ?? string.Empty;
            ReceiverName = receiverName ?? string.Empty;
            ReceiverBankName = receiverBankName ?? string.Empty;
            ReceiverBankBik = receiverBankBik ?? string.Empty;
            ReceiverAccount = receiverAccount ?? string.Empty;
        }
    }
}
