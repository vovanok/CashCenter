using System;

namespace CashCenter.IvEnergySales.BusinessLogic
{
    public class InfoForCheck
    {
        public decimal Cost { get; private set; }
        public DateTime Date { get; private set; }
        public string DbCode { get; private set; }
        public int CustomerNumber { get; private set; }
        public string CustomerName { get; private set; }
        public string PaymentReasonName { get; private set; }
        public string CustomerEmail { get; private set; }

        public InfoForCheck(decimal cost, DateTime date, string dbCode, int customerNumber,
            string customerName, string paymentReasonName, string customerEmail)
        {
            Cost = cost;
            Date = date;
            DbCode = dbCode ?? string.Empty;
            CustomerNumber = customerNumber;
            CustomerName = customerName ?? string.Empty;
            PaymentReasonName = paymentReasonName ?? string.Empty;
            CustomerEmail = customerEmail ?? string.Empty;
        }
    }
}
