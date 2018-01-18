using System;

namespace CashCenter.DataMigration.Providers.Word.Entities
{
    public class CommonPaymentsReportItem
    {
        public DateTime Date { get; private set; }
        public decimal EnergyTotal { get; set; }
        public decimal WaterWithoutCommissionTotal { get; set; }
        public decimal WaterCommissionTotal { get; set; }
        public decimal GarbageWithoutCommissionTotal { get; set; }
        public decimal GarbageCommissionTotal { get; set; }
        public decimal RepairWithoutCommissionTotal { get; set; }
        public decimal RepairCommissionTotal { get; set; }

        public CommonPaymentsReportItem(DateTime date)
        {
            Date = date;
        }
    }
}