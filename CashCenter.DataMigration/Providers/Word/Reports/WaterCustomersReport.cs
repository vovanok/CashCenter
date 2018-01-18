using System;
using System.Collections.Generic;
using CashCenter.DataMigration.Providers.Word.Entities;
using NetOffice.WordApi;

namespace CashCenter.DataMigration.Providers.Word.Reports
{
    public class WaterCustomersReport : IWordReport
    {
        public double CommisionValue { get; private set; }
        public IEnumerable<WaterCustomersReportItem> WaterCustomersPayments { get; private set; }
        public double TotalCounterValue1 { get; private set; }
        public double TotalCounterValue2 { get; private set; }
        public double TotalCounterValue3 { get; private set; }
        public decimal TotalPaymentCost { get; private set; }
        public decimal TotalPenaltyCost { get; private set; }
        public decimal TotalPaymentAndPenaltyCost { get; private set; }
        public decimal TotalComissionCost { get; private set; }
        public decimal SummaryTotalCost { get; private set; }

        public WaterCustomersReport(double commisionValue, IEnumerable<WaterCustomersReportItem> waterCustomersPayments)
        {
            CommisionValue = commisionValue;
            WaterCustomersPayments = waterCustomersPayments ?? new List<WaterCustomersReportItem>();

            foreach (var waterCustomerPayment in WaterCustomersPayments)
            {
                TotalCounterValue1 += waterCustomerPayment.CounterValue1;
                TotalCounterValue2 += waterCustomerPayment.CounterValue2;
                TotalCounterValue3 += waterCustomerPayment.CounterValue3;
                TotalPaymentCost += waterCustomerPayment.PaymentCost;
                TotalPenaltyCost += waterCustomerPayment.PenaltyCost;
                TotalPaymentAndPenaltyCost += waterCustomerPayment.PaymentAndPenaltyCost;
                TotalComissionCost += waterCustomerPayment.ComissionCost;
                SummaryTotalCost += waterCustomerPayment.TotalCost;
            }
        }

        public void ExportToDocument(Document wordDocument)
        {
            if (wordDocument == null)
                return;

            var paymentsTable = wordDocument.Bookmarks["CustomerPayment"]?.Range?.Tables[1];
            if (paymentsTable == null)
                throw new ApplicationException("Не найдена таблица для выгрузки платежей в шаблоне отчета");

            wordDocument.Bookmarks["CommisionValue"].Range.Text = CommisionValue.ToString("0.00");

            int paymentNumber = 1;
            foreach (var customerPayment in WaterCustomersPayments)
            {
                var customerPaymentRow = paymentsTable.Rows.Add(paymentsTable.Rows[paymentNumber + 1]);
                customerPaymentRow.Cells[1].Range.Text = customerPayment.CreationDateTime.ToString("dd.MM.yyyy HH:mm");
                customerPaymentRow.Cells[2].Range.Text = customerPayment.CustomerNumber.ToString();
                customerPaymentRow.Cells[3].Range.Text = customerPayment.CounterValue1.ToString("0.00");
                customerPaymentRow.Cells[4].Range.Text = customerPayment.CounterValue2.ToString("0.00");
                customerPaymentRow.Cells[5].Range.Text = customerPayment.CounterValue3.ToString("0.00");
                customerPaymentRow.Cells[6].Range.Text = customerPayment.PaymentCost.ToString("0.00");
                customerPaymentRow.Cells[7].Range.Text = customerPayment.PenaltyCost.ToString("0.00");
                customerPaymentRow.Cells[8].Range.Text = customerPayment.PaymentAndPenaltyCost.ToString("0.00");
                customerPaymentRow.Cells[9].Range.Text = customerPayment.ComissionCost.ToString("0.00");
                customerPaymentRow.Cells[10].Range.Text = customerPayment.TotalCost.ToString("0.00");
                paymentNumber++;
            }

            paymentsTable.Rows[paymentNumber + 1].Delete();

            wordDocument.Bookmarks["TotalCounterValue1"].Range.Text = TotalCounterValue1.ToString("0.00");
            wordDocument.Bookmarks["TotalCounterValue2"].Range.Text = TotalCounterValue2.ToString("0.00");
            wordDocument.Bookmarks["TotalCounterValue3"].Range.Text = TotalCounterValue3.ToString("0.00");
            wordDocument.Bookmarks["TotalPaymentCost"].Range.Text = TotalPaymentCost.ToString("0.00");
            wordDocument.Bookmarks["TotalPenaltyCost"].Range.Text = TotalPenaltyCost.ToString("0.00");
            wordDocument.Bookmarks["TotalPaymentAndPenaltyCost"].Range.Text = TotalPaymentAndPenaltyCost.ToString("0.00");
            wordDocument.Bookmarks["TotalComissionCost"].Range.Text = TotalComissionCost.ToString("0.00");
            wordDocument.Bookmarks["SummaryTotalCost"].Range.Text = SummaryTotalCost.ToString("0.00");
        }
    }
}
