using System;
using System.Collections.Generic;

namespace CashCenter.DataMigration.Providers.Word.Entities
{
    public class CommonPaymentsModel
    {
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public List<CommonPaymentsItemModel> Items { get; private set; }
        public decimal FinalEnergyTotal { get; private set; }
        public decimal FinalWaterWithoutComissionTotal { get; private set; }
        public decimal FinalWaterComissionTotal { get; private set; }
        public decimal FinalGarbageWithoutComissionTotal { get; private set; }
        public decimal FinalGarbageComissionTotal { get; private set; }
        public decimal FinalRepairWithoutComissionTotal { get; private set; }
        public decimal FinalRepairComissionTotal { get; private set; }

        public CommonPaymentsModel(DateTime startDate, DateTime endDate,
            List<CommonPaymentsItemModel> items, decimal finalEnergyTotal,
            decimal finalWaterWithoutComissionTotal, decimal finalWaterComissionTotal,
            decimal finalGarbageWithoutComissionTotal, decimal finalGarbageComissionTotal,
            decimal finalRepairWithoutComissionTotal, decimal finalRepairComissionTotal)
        {
            StartDate = startDate;
            EndDate = endDate;
            Items = items ?? new List<CommonPaymentsItemModel>();
            FinalEnergyTotal = finalEnergyTotal;
            FinalWaterWithoutComissionTotal = finalWaterWithoutComissionTotal;
            FinalWaterComissionTotal = finalWaterComissionTotal;
            FinalGarbageWithoutComissionTotal = finalGarbageWithoutComissionTotal;
            FinalGarbageComissionTotal = finalGarbageComissionTotal;
            FinalRepairWithoutComissionTotal = finalRepairWithoutComissionTotal;
            FinalRepairComissionTotal = finalRepairComissionTotal;
        }
    }
}
