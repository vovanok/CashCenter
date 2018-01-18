using System;
using System.Collections.Generic;
using CashCenter.DataMigration.Providers.Word.Entities;
using NetOffice.WordApi;

namespace CashCenter.DataMigration.Providers.Word.Report
{
    public class CommonPaymentsReport : IWordReport
    {
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public List<CommonPaymentsReportItem> Items { get; private set; }
        public decimal FinalEnergyTotal { get; private set; }
        public decimal FinalWaterWithoutComissionTotal { get; private set; }
        public decimal FinalWaterComissionTotal { get; private set; }
        public decimal FinalGarbageWithoutComissionTotal { get; private set; }
        public decimal FinalGarbageComissionTotal { get; private set; }
        public decimal FinalRepairWithoutComissionTotal { get; private set; }
        public decimal FinalRepairComissionTotal { get; private set; }

        public CommonPaymentsReport(DateTime startDate, DateTime endDate,
            List<CommonPaymentsReportItem> items, decimal finalEnergyTotal,
            decimal finalWaterWithoutComissionTotal, decimal finalWaterComissionTotal,
            decimal finalGarbageWithoutComissionTotal, decimal finalGarbageComissionTotal,
            decimal finalRepairWithoutComissionTotal, decimal finalRepairComissionTotal)
        {
            StartDate = startDate;
            EndDate = endDate;
            Items = items ?? new List<CommonPaymentsReportItem>();
            FinalEnergyTotal = finalEnergyTotal;
            FinalWaterWithoutComissionTotal = finalWaterWithoutComissionTotal;
            FinalWaterComissionTotal = finalWaterComissionTotal;
            FinalGarbageWithoutComissionTotal = finalGarbageWithoutComissionTotal;
            FinalGarbageComissionTotal = finalGarbageComissionTotal;
            FinalRepairWithoutComissionTotal = finalRepairWithoutComissionTotal;
            FinalRepairComissionTotal = finalRepairComissionTotal;
        }

        public void ExportToDocument(Document wordDocument)
        {
            wordDocument.Bookmarks["StartDate"].Range.Text = StartDate.ToString("dd MMMM yyyy г.");
            wordDocument.Bookmarks["EndDate"].Range.Text = EndDate.ToString("dd MMMM yyyy г.");

            var itemsTable = wordDocument.Bookmarks["Payments"]?.Range?.Tables[1];
            if (itemsTable == null)
                throw new ApplicationException("Не найдена таблица для выгрузки платежей в шаблоне отчета");

            int acticleSaleNumber = 1;
            foreach (var modelItem in Items)
            {
                var row = itemsTable.Rows.Add(itemsTable.Rows[acticleSaleNumber + 1]);
                row.Cells[1].Range.Text = modelItem.Date.ToString("dd.MM.yyyy");
                row.Cells[2].Range.Text = modelItem.EnergyTotal.ToString("0.00");
                row.Cells[3].Range.Text = modelItem.WaterWithoutCommissionTotal.ToString("0.00");
                row.Cells[4].Range.Text = modelItem.WaterCommissionTotal.ToString("0.00");
                row.Cells[5].Range.Text = modelItem.GarbageWithoutCommissionTotal.ToString("0.00");
                row.Cells[6].Range.Text = modelItem.GarbageCommissionTotal.ToString("0.00");
                row.Cells[7].Range.Text = modelItem.RepairWithoutCommissionTotal.ToString("0.00");
                row.Cells[8].Range.Text = modelItem.RepairCommissionTotal.ToString("0.00");
                acticleSaleNumber++;
            }

            itemsTable.Rows[acticleSaleNumber + 1].Delete();

            wordDocument.Bookmarks["FinalEnergyTotal"].Range.Text = FinalEnergyTotal.ToString("0.00");

            wordDocument.Bookmarks["FinalWaterWithoutComissionTotal"].Range.Text = FinalWaterWithoutComissionTotal.ToString("0.00");
            wordDocument.Bookmarks["FinalWaterComissionTotal"].Range.Text = FinalWaterComissionTotal.ToString("0.00");

            wordDocument.Bookmarks["FinalGarbageWithoutComissionTotal"].Range.Text = FinalGarbageWithoutComissionTotal.ToString("0.00");
            wordDocument.Bookmarks["FinalGarbageComissionTotal"].Range.Text = FinalGarbageComissionTotal.ToString("0.00");

            wordDocument.Bookmarks["FinalRepairWithoutComissionTotal"].Range.Text = FinalRepairWithoutComissionTotal.ToString("0.00");
            wordDocument.Bookmarks["FinalRepairComissionTotal"].Range.Text = FinalRepairComissionTotal.ToString("0.00");
        }
    }
}
