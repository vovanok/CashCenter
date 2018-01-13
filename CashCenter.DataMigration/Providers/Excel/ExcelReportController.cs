using System;
using System.IO;
using NetOffice.ExcelApi;
using CashCenter.DataMigration.Providers.Excel.Entities;
using System.Globalization;

namespace CashCenter.DataMigration.Providers.Excel
{
    public class ExcelReportController
    {
        private string templateFilename;

        public ExcelReportController(string templateFilename)
        {
            this.templateFilename = templateFilename;
        }

        public void CreateReport(EnergyCustomersGisHusModel model)
        {
            var templateFullpath = Path.Combine(Environment.CurrentDirectory, templateFilename);
            if (!File.Exists(templateFullpath))
                throw new ApplicationException($"Файл-шаблон отчета не существует или не задан ({templateFullpath})");

            try
            {
                using (var application = new Application())
                {
                    using (var workbook = application.Workbooks.Add(templateFullpath))
                    {
                        var worksheet = (Worksheet)workbook.Worksheets[1];

                        string numberFormat = string.Format("0.00", CultureInfo.InvariantCulture);

                        int currentRow = 2;
                        foreach (var payment in model.Payments)
                        {
                            worksheet.Cells[currentRow, 1].Value = payment.OrderNumber.ToString();
                            worksheet.Cells[currentRow, 2].Value = payment.Total;
                            worksheet.Cells[currentRow, 2].NumberFormat = numberFormat;
                            worksheet.Cells[currentRow, 3].Value = payment.Date.ToString("dd.MM.yyyy");
                            worksheet.Cells[currentRow, 4].Value = payment.PaymentPeriod.ToString("MM.yyyy");
                            worksheet.Cells[currentRow, 5].Value = payment.PaymentDocumentIdentifier;
                            worksheet.Cells[currentRow, 6].Value = payment.HusIdentifier;
                            currentRow++;
                        }
                    }

                    application.Visible = true;
                }
            }
            catch (Exception ex)
            {
                throw new SystemException("Ошибка экспорта платежей ЭЭ в документ Excel", ex);
            }
        }
    }
}
