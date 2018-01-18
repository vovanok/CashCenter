using System;
using System.Linq;
using System.Collections.Generic;
using CashCenter.Dal;
using CashCenter.Common;
using CashCenter.DataMigration.Providers.Excel;
using CashCenter.DataMigration.Providers.Excel.Reports;
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
                    new EnergyCustomersGisHusReportItem(
                        currentOrderNumber++,
                        Guid.NewGuid().ToString("N"),
                        item.Cost,
                        item.CreateDate,
                        item.CreateDate,
                        item.EnergyCustomer.PaymentDocumentIdentifier,
                        item.EnergyCustomer.HusIdentifier,
                        GetUnifiedAccountByHusIdentifier(item.EnergyCustomer.HusIdentifier),
                        "performer INN",
                        "performer Name",
                        "performer KPP",
                        "receiver INN",
                        "receiver Name",
                        "receiver bank name",
                        "receiver bank bik",
                        "receiver account")).ToList();

            var customerPaymentModelsCount = customerPaymentModels.Count;
            if (customerPaymentModelsCount == 0)
                return new ExportResult();

            var reportModel = new EnergyCustomersGisHusReport(beginDatetime, endDatetime, customerPaymentModels);

            var excelReport = new ExcelReportController(Config.EnergyCustomersReportGisHusTemplateFilename);
            excelReport.CreateReport(reportModel);

            return new ExportResult(customerPaymentModelsCount, energyPayments.Count() - customerPaymentModelsCount);
        }

        private string GetUnifiedAccountByHusIdentifier(string husIdentifier)
        {
            if (string.IsNullOrEmpty(husIdentifier) || husIdentifier.Length < 3)
                return string.Empty;

            return husIdentifier.Substring(0, husIdentifier.Length - 3);
        }
    }
}
