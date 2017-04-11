using System;
using System.Collections.Generic;

namespace CashCenter.WordReport
{
    public class ReportCustomersModel
    {
        public DateTime StartDate { get; private set; }

        public DateTime EndDate { get; private set; }

        public IEnumerable<ReportCustomerPaymentModel> CustomerPayments { get; private set; }

        public ReportCustomersModel(
            DateTime startDate, DateTime endDate,
            IEnumerable<ReportCustomerPaymentModel> customerPayments)
        {
            StartDate = startDate;
            EndDate = endDate;
            CustomerPayments = customerPayments ?? new List<ReportCustomerPaymentModel>();
        }
    }
}
