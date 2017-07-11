using System;
using System.Collections.Generic;

namespace CashCenter.DataMigration.Providers.Word.Entities
{
    public class ReportEnergyCustomersModel
    {
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public IEnumerable<ReportEnergyCustomerPaymentModel> CustomerPayments { get; private set; }

        public ReportEnergyCustomersModel(
            DateTime startDate, DateTime endDate,
            IEnumerable<ReportEnergyCustomerPaymentModel> customerPayments)
        {
            StartDate = startDate;
            EndDate = endDate;
            CustomerPayments = customerPayments ?? new List<ReportEnergyCustomerPaymentModel>();
        }
    }
}
