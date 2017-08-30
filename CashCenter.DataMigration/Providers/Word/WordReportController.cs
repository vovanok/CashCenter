using System;
using System.IO;
using System.Linq;
using CashCenter.DataMigration.Providers.Word.Entities;

namespace CashCenter.DataMigration.Providers.Word
{
    public class WordReportController
    {
        private string templateFilename;

        public WordReportController(string templateFilename)
        {
            this.templateFilename = templateFilename;
        }

        public void CreateReport(ReportEnergyCustomersModel model)
        {
            var templateFullpath = Path.Combine(Environment.CurrentDirectory, templateFilename);
            if (!File.Exists(templateFullpath))
                throw new ApplicationException($"Файл-шаблон отчета не существует или не задан ({templateFullpath})");

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
                            throw new ApplicationException("Не найдена таблица для выгрузки платежей в шаблоне отчета");

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
                throw new SystemException("Ошибка экспорта платежей физ.лиц в документ Word", ex);
            }
        }

        public void CreateReport(ReportWaterCustomersModel model)
        {
            var templateFullpath = Path.Combine(Environment.CurrentDirectory, templateFilename);
            if (!File.Exists(templateFullpath))
                throw new ApplicationException($"Файл-шаблон отчета не существует или не задан ({templateFullpath})");

            try
            {
                using (var application = new NetOffice.WordApi.Application { Visible = true })
                {
                    using (var document = application.Documents.Add(templateFullpath))
                    {
                        var paymentsTable = document.Bookmarks["CustomerPayment"]?.Range?.Tables[1];
                        if (paymentsTable == null)
                            throw new ApplicationException("Не найдена таблица для выгрузки платежей в шаблоне отчета");

                        document.Bookmarks["CommisionValue"].Range.Text = model.CommisionValue.ToString("0.00");

                        int paymentNumber = 1;
                        foreach (var customerPayment in model.WaterCustomersPayments)
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

                        document.Bookmarks["TotalCounterValue1"].Range.Text = model.TotalCounterValue1.ToString("0.00");
                        document.Bookmarks["TotalCounterValue2"].Range.Text = model.TotalCounterValue2.ToString("0.00");
                        document.Bookmarks["TotalCounterValue3"].Range.Text = model.TotalCounterValue3.ToString("0.00");
                        document.Bookmarks["TotalPaymentCost"].Range.Text = model.TotalPaymentCost.ToString("0.00");
                        document.Bookmarks["TotalPenaltyCost"].Range.Text = model.TotalPenaltyCost.ToString("0.00");
                        document.Bookmarks["TotalPaymentAndPenaltyCost"].Range.Text = model.TotalPaymentAndPenaltyCost.ToString("0.00");
                        document.Bookmarks["TotalComissionCost"].Range.Text = model.TotalComissionCost.ToString("0.00");
                        document.Bookmarks["SummaryTotalCost"].Range.Text = model.SummaryTotalCost.ToString("0.00");
                    }

                    application.Activate();
                }
            }
            catch (Exception ex)
            {
                throw new SystemException("Ошибка экспорта платежей физ.лиц в документ Word", ex);
            }
        }

        public void CreateReport(ReportArticlesSalesModel model)
        {
            var templateFullpath = Path.Combine(Environment.CurrentDirectory, templateFilename);
            if (!File.Exists(templateFullpath))
                throw new ApplicationException($"Файл-шаблон отчета не существует или не задан ({templateFullpath})");

            try
            {
                using (var application = new NetOffice.WordApi.Application { Visible = true })
                {
                    using (var document = application.Documents.Add(templateFullpath))
                    {
                        document.Bookmarks["StartDate"].Range.Text = model.StartDate.ToString("dd MMMM yyyy г.");
                        document.Bookmarks["EndDate"].Range.Text = model.EndDate.ToString("dd MMMM yyyy г.");

                        var salesTable = document.Bookmarks["ArticlesSales"]?.Range?.Tables[1];
                        if (salesTable == null)
                            throw new ApplicationException("Не найдена таблица для выгрузки продаж в шаблоне отчета");

                        int acticleSaleNumber = 1;
                        foreach (var articleSale in model.ArticlesSales)
                        {
                            var articleSaleRow = salesTable.Rows.Add(salesTable.Rows[acticleSaleNumber + 1]);
                            articleSaleRow.Cells[1].Range.Text = acticleSaleNumber++.ToString();
                            articleSaleRow.Cells[2].Range.Text = articleSale.ArticleName;
                            articleSaleRow.Cells[3].Range.Text = articleSale.ArticleCode;
                            articleSaleRow.Cells[4].Range.Text = articleSale.ArticleQuantity.ToString("0.00");
                            articleSaleRow.Cells[5].Range.Text = articleSale.ArticlePrice.ToString("0.00");
                            articleSaleRow.Cells[6].Range.Text = articleSale.ArticleCost.ToString("0.00");
                        }

                        salesTable.Rows[acticleSaleNumber + 1].Delete();

                        document.Bookmarks["TotalCost"].Range.Text = model.TotalCost.ToString("0.00");
                        document.Bookmarks["NdsValue"].Range.Text = model.NdsValue.ToString("0.00");
                    }

                    application.Activate();
                }
            }
            catch (Exception ex)
            {
                throw new SystemException("Ошибка экспорта продаж товаров в документ Word", ex);
            }
        }
    }
}
