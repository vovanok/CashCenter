using CashCenter.Common;
using CashCenter.Dal;
using CashCenter.WordReport;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CashCenter.DataMigration
{
    public class CustomerPaymentsWordReportExporter : BaseExporter<CustomerPayment>
    {
        protected override List<CustomerPayment> GetSourceItems(DateTime beginDatetime, DateTime endDatetime)
        {
            return DalController.Instance.CustomerPayments.Where(customerPayment =>
                beginDatetime <= customerPayment.CreateDate && customerPayment.CreateDate <= endDatetime).ToList();
        }

        protected override int TryExportItems(IEnumerable<CustomerPayment> items)
        {
            var customerPaymentModels = items.Select(item =>
                new ReportCustomerPaymentModel(item.Customer.Number, item.NewDayValue, item.NewNightValue, item.Cost, item.CreateDate));

            var customerPaymentModelsCount = customerPaymentModels.Count();
            if (customerPaymentModelsCount == 0)
                return 0;

            var reportModel = new ReportCustomersModel(beginDatetime, endDatetime, customerPaymentModels);

            var wordReport = new WordReportController(Config.CustomersReportTemplateFilename);
            wordReport.CreateReport(reportModel);

            return customerPaymentModelsCount;
        }
    }
}
