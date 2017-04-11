using CashCenter.Common;
using System;
using System.IO;
using System.Linq;

namespace CashCenter.WordReport
{
    public class WordReportController
    {
        private string templateFilename;

        public WordReportController(string templateFilename)
        {
            this.templateFilename = templateFilename;
        }

        public void CreateReport(ReportCustomersModel model)
        {
            var templateFullpath = Path.Combine(Environment.CurrentDirectory, templateFilename);
            if (!File.Exists(templateFullpath))
            {
                Log.Error($"Файл-шаблон отчета не существует или не задан ({templateFullpath}).");
                return;
            }

            try
            {
                using (var application = new NetOffice.WordApi.Application { Visible = true })
                {
                    using (var document = application.Documents.Add(templateFullpath))
                    {
                        document.Bookmarks["StartDate"].Range.Text = model.StartDate.ToString("dd.MM.yyyy");
                        document.Bookmarks["EndDate"].Range.Text = model.EndDate.ToString("dd.MM.yyyy");

                        var paymentsTable = document.Bookmarks["CustomerPayment"]?.Range?.Tables[1];
                        if (paymentsTable == null)
                        {
                            Log.Error("Не найдена таблица для выгрузки платежей в шаблоне отчета.");
                            return;
                        }

                        foreach (var customerPayment in model.CustomerPayments)
                        {
                            var customerPaymentRow = paymentsTable.Rows.Add();
                            customerPaymentRow.Cells[1].Range.Text = customerPayment.CustomerNumber.ToString();
                            customerPaymentRow.Cells[2].Range.Text = customerPayment.DayValue.ToString();
                            customerPaymentRow.Cells[3].Range.Text = customerPayment.NightValue.ToString();
                            customerPaymentRow.Cells[4].Range.Text = customerPayment.Cost.ToString("0.00");
                            customerPaymentRow.Cells[5].Range.Text = customerPayment.CreationDate.ToString("dd.MM.yyyy");
                        }

                        document.Bookmarks["TotalCost"].Range.Text = model.CustomerPayments.Sum(payment => payment.Cost).ToString("0.00");
                    }

                    application.Activate();
                }
            }
            catch (Exception ex)
            {
                Log.ErrorWithException("Ошибка экспорта платежей физ.лиц в документ Word.", ex);
            }
        }
    }
}
