using System;

namespace CashCenter.DataMigration.Providers.Word.Entities
{
    public class CommonPaymentsReportItem
    {
        public DateTime Date { get; private set; }

        // Energy
        public decimal EnergyTotal { get; set; }

        // Water
        public decimal WaterWithoutCommissionTotal { get; set; }
        public decimal WaterCommissionTotal { get; set; }

        // Garbage
        public decimal GarbageWithoutCommissionTotal { get; set; }
        public decimal GarbageCommissionTotal { get; set; }

        // Repair
        public decimal RepairWithoutCommissionTotal { get; set; }
        public decimal RepairCommissionTotal { get; set; }

        // Hot water
        public decimal HotWaterWithoutCommissionTotal { get; set; }
        public decimal HotWaterCommissionTotal { get; set; }

        public CommonPaymentsReportItem(DateTime date)
        {
            Date = date;
        }
    }
}