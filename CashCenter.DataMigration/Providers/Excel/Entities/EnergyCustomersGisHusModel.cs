using System;
using System.Collections.Generic;

namespace CashCenter.DataMigration.Providers.Excel.Entities
{
    public class EnergyCustomersGisHusModel
    {
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public IEnumerable<EnergyPaymentGisHusModel> Payments { get; private set; }

        public EnergyCustomersGisHusModel(
            DateTime startDate, DateTime endDate,
            IEnumerable<EnergyPaymentGisHusModel> payments)
        {
            StartDate = startDate;
            EndDate = endDate;
            Payments = payments ?? new List<EnergyPaymentGisHusModel>();
        }
    }
}
