using System;
using System.Collections.Generic;
using System.Globalization;
using NetOffice.ExcelApi;
using CashCenter.DataMigration.Providers.Excel.Entities;

namespace CashCenter.DataMigration.Providers.Excel.Reports
{
    public class EnergyCustomersGisHusReport : IExcelReport
    {
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public IEnumerable<EnergyCustomersGisHusReportItem> Payments { get; private set; }

        public EnergyCustomersGisHusReport(
            DateTime startDate, DateTime endDate,
            IEnumerable<EnergyCustomersGisHusReportItem> payments)
        {
            StartDate = startDate;
            EndDate = endDate;
            Payments = payments ?? new List<EnergyCustomersGisHusReportItem>();
        }

        public void ExportToExcel(Workbook excelWorkbook)
        {
            var worksheet = (Worksheet)excelWorkbook.Worksheets[1];

            string numberFormat = string.Format("0.00", CultureInfo.InvariantCulture);

            int currentRow = 4;
            foreach (var payment in Payments)
            {
                worksheet.Cells[currentRow, 1].Value = payment.OrderNumber.ToString();
                worksheet.Cells[currentRow, 2].Value = payment.PaymentIdentifier;
                worksheet.Cells[currentRow, 3].Value = payment.Total;
                worksheet.Cells[currentRow, 3].NumberFormat = numberFormat;
                worksheet.Cells[currentRow, 4].Value = payment.Date.ToString("dd.MM.yyyy");
                worksheet.Cells[currentRow, 5].Value = payment.PaymentDocumentIdentifier;
                worksheet.Cells[currentRow, 6].Value = payment.UnifiedAccount;
                worksheet.Cells[currentRow, 7].Value = payment.HusIdentifier;
                worksheet.Cells[currentRow, 14].Value = payment.PaymentPeriod.ToString("MM.yyyy");
                worksheet.Cells[currentRow, 20].Value = payment.PerformerInn;
                worksheet.Cells[currentRow, 24].Value = payment.PerformerName;
                worksheet.Cells[currentRow, 25].Value = payment.PerformerKpp;
                worksheet.Cells[currentRow, 28].Value = payment.ReceiverInn;
                worksheet.Cells[currentRow, 30].Value = payment.ReceiverName;
                worksheet.Cells[currentRow, 31].Value = payment.ReceiverBankName;
                worksheet.Cells[currentRow, 32].Value = payment.ReceiverBankBik;
                worksheet.Cells[currentRow, 34].Value = payment.ReceiverAccount;
                worksheet.Cells[currentRow, 34].Font.Bold = true;

                currentRow++;
            }
        }
    }
}
