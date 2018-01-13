using System;
using System.Linq;
using System.Collections.Generic;
using CashCenter.Dal;
using CashCenter.Common;
using CashCenter.DataMigration.Providers.Excel;
using CashCenter.DataMigration.Providers.Excel.Entities;

namespace CashCenter.DataMigration.EnergyCustomers
{
    public class EnergyPaymentsGisHusExporter : BaseExporter<EnergyCustomerPayment>
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

            int currentOrderNumber = 1;
            var customerPaymentModels = energyPayments
                .Where(item => item != null)
                .Select(item =>
                    new EnergyPaymentGisHusModel(
                        currentOrderNumber++,
                        item.Cost,
                        item.CreateDate,
                        item.CreateDate,
                        item.EnergyCustomer.PaymentDocumentIdentifier,
                        item.EnergyCustomer.HusIdentifier)).ToList();

            var customerPaymentModelsCount = customerPaymentModels.Count;
            if (customerPaymentModelsCount == 0)
                return new ExportResult();

            var reportModel = new EnergyCustomersGisHusModel(beginDatetime, endDatetime, customerPaymentModels);

            var excelReport = new ExcelReportController(Config.EnergyCustomersReportGisHusTemplateFilename);
            excelReport.CreateReport(reportModel);

            return new ExportResult(customerPaymentModelsCount, energyPayments.Count() - customerPaymentModelsCount);
        }
    }
}
