using System;
using System.Collections.Generic;
using System.Linq;
using CashCenter.Common;
using CashCenter.Dal;
using CashCenter.DataMigration.Providers.Word;
using CashCenter.DataMigration.Providers.Word.Reports;
using CashCenter.DataMigration.Providers.Word.Entities;

namespace CashCenter.DataMigration.EnergyCustomers
{
    public class EnergyCustomerPaymentsWordExporter : BaseExporter<EnergyCustomerPayment>
    {
        protected override List<EnergyCustomerPayment> GetSourceItems(DateTime beginDatetime, DateTime endDatetime)
        {
            return DalController.Instance.EnergyCustomerPayments.Where(customerPayment =>
                beginDatetime <= customerPayment.CreateDate && customerPayment.CreateDate <= endDatetime).ToList();
        }

        protected override ExportResult TryExportItems(IEnumerable<EnergyCustomerPayment> energyPayments)
        {
            if (energyPayments == null)
                return new ExportResult();

            var customerPaymentModels = energyPayments
                .Where(item => item != null)
                .Select(item =>
                    new EnergyCustomersReportItem(
                        item.EnergyCustomer.Number,
                        item.NewDayValue,
                        item.NewNightValue,
                        item.Cost,
                        item.CreateDate));

            var customerPaymentModelsCount = customerPaymentModels.Count();
            if (customerPaymentModelsCount == 0)
                return new ExportResult();

            var reportModel = new EnergyCustomersReport(beginDatetime, endDatetime, customerPaymentModels);

            var wordReport = new WordReportController(Config.EnergyCustomersReportTemplateFilename);
            wordReport.CreateReport(reportModel);

            return new ExportResult(customerPaymentModelsCount, energyPayments.Count() - customerPaymentModelsCount);
        }
    }
}
