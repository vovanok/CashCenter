using System;

namespace CashCenter.DataMigration.Providers.Word.Entities
{
    public class ReportWaterAndEnergyCustomersPaymentsItemModel
    {
        public DateTime Date { get; private set; }
        public decimal EnergyCost { get; set; }
        public decimal WaterWithoutComissionCost { get; set; }
        public decimal WaterComissionCost { get; set; }

        public ReportWaterAndEnergyCustomersPaymentsItemModel(DateTime date, decimal energyCost,
            decimal waterWithoutComissionCost, decimal waterComissionCost)
        {
            Date = date;
            EnergyCost = energyCost;
            WaterWithoutComissionCost = waterWithoutComissionCost;
            WaterComissionCost = waterComissionCost;
        }
    }
}