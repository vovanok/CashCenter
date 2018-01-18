using System;
using System.IO;

namespace CashCenter.DataMigration.Providers.Word
{
    public class WordReportController
    {
        private string templateFilename;

        public WordReportController(string templateFilename)
        {
            this.templateFilename = templateFilename;
        }

        public void CreateReport(IWordReport report)
        {
            var templateFullpath = Path.Combine(Environment.CurrentDirectory, templateFilename);
            if (!File.Exists(templateFullpath))
                throw new ApplicationException($"Файл-шаблон отчета не существует или не задан ({templateFullpath})");

            try
            {
                using (var application = new NetOffice.WordApi.Application())
                {
                    using (var document = application.Documents.Add(templateFullpath))
                    {
                        report.ExportToDocument(document);
                    }

                    application.Visible = true;
                    application.Activate();
                }
            }
            catch (Exception ex)
            {
                throw new SystemException("Ошибка экспорта в документ Word", ex);
            }
        }
    }
}
