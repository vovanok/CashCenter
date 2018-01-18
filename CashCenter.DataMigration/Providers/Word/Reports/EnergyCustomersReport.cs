using System;
using System.Linq;
using System.Collections.Generic;
using NetOffice.WordApi;
using CashCenter.DataMigration.Providers.Word.Entities;

namespace CashCenter.DataMigration.Providers.Word.Reports
{
    public class EnergyCustomersReport : IWordReport
    {
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public IEnumerable<EnergyCustomersReportItem> CustomerPayments { get; private set; }

        public EnergyCustomersReport(
            DateTime startDate, DateTime endDate,
            IEnumerable<EnergyCustomersReportItem> customerPayments)
        {
            StartDate = startDate;
            EndDate = endDate;
            CustomerPayments = customerPayments ?? new List<EnergyCustomersReportItem>();
        }

        public void ExportToDocument(Document wordDocument)
        {
            if (wordDocument == null)
                return;

            wordDocument.Bookmarks["StartDate"].Range.Text = StartDate.ToString("dd.MM.yyyy");
            wordDocument.Bookmarks["EndDate"].Range.Text = EndDate.ToString("dd.MM.yyyy");

            var paymentsTable = wordDocument.Bookmarks["CustomerPayment"]?.Range?.Tables[1];
            if (paymentsTable == null)
                throw new ApplicationException("Не найдена таблица для выгрузки платежей в шаблоне отчета");

            foreach (var customerPayment in CustomerPayments)
            {
                var customerPaymentRow = paymentsTable.Rows.Add();
                customerPaymentRow.Cells[1].Range.Text = customerPayment.CustomerNumber.ToString();
                customerPaymentRow.Cells[2].Range.Text = customerPayment.DayValue.ToString();
                customerPaymentRow.Cells[3].Range.Text = customerPayment.NightValue.ToString();
                customerPaymentRow.Cells[4].Range.Text = customerPayment.Cost.ToString("0.00");
                customerPaymentRow.Cells[5].Range.Text = customerPayment.CreationDate.ToString("dd.MM.yyyy");
            }

            wordDocument.Bookmarks["TotalCost"].Range.Text = CustomerPayments.Sum(payment => payment.Cost).ToString("0.00");
        }
    }
}
