using System;
using System.IO;
using NetOffice.ExcelApi;

namespace CashCenter.DataMigration.Providers.Excel
{
    public class ExcelReportController
    {
        private string templateFilename;

        public ExcelReportController(string templateFilename)
        {
            this.templateFilename = templateFilename;
        }

        public void CreateReport(IExcelReport report)
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
                        report.ExportToExcel(workbook);
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
