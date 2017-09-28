using System;
using System.Collections.Generic;

namespace CashCenter.DataMigration.Providers.Word.Entities
{
    public class ReportWaterAndEnergyCustomersPaymentsModel
    {
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public List<ReportWaterAndEnergyCustomersPaymentsItemModel> Items { get; private set; }
        public decimal TotalEnergyCost { get; private set; }
        public decimal TotalWaterWithoutComissionCost { get; private set; }
        public decimal TotalWaterComissionCost { get; private set; }

        public ReportWaterAndEnergyCustomersPaymentsModel(DateTime startDate, DateTime endDate,
            List<ReportWaterAndEnergyCustomersPaymentsItemModel> items, decimal totalEnergyCost,
            decimal totalWaterWithoutComissionCost, decimal totalWaterComissionCost)
        {
            StartDate = startDate;
            EndDate = endDate;
            Items = items ?? new List<ReportWaterAndEnergyCustomersPaymentsItemModel>();
            TotalEnergyCost = totalEnergyCost;
            TotalWaterWithoutComissionCost = totalWaterWithoutComissionCost;
            TotalWaterComissionCost = totalWaterComissionCost;
        }
    }
}
