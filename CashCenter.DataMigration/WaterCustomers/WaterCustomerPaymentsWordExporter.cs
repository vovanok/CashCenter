using System;
using System.Linq;
using System.Collections.Generic;
using CashCenter.Common;
using CashCenter.Dal;
using CashCenter.DataMigration.Providers.Word;
using CashCenter.DataMigration.Providers.Word.Reports;
using CashCenter.DataMigration.Providers.Word.Entities;

namespace CashCenter.DataMigration.WaterCustomers
{
    public class WaterCustomerPaymentsWordExporter : BaseExporter<WaterCustomerPayment>
    {
        protected override List<WaterCustomerPayment> GetSourceItems(DateTime beginDatetime, DateTime endDatetime)
        {
            return DalController.Instance.WaterCustomerPayments.Where(waterCustomerPayment =>
                beginDatetime <= waterCustomerPayment.CreateDate && waterCustomerPayment.CreateDate <= endDatetime).ToList();
        }

        protected override ExportResult TryExportItems(IEnumerable<WaterCustomerPayment> waterCustomerPayments)
        {
            if (waterCustomerPayments == null)
                return new ExportResult();

            var customerPaymentModels = waterCustomerPayments
                .Where(waterCustomerPayment => waterCustomerPayment != null)
                .Select(waterCustomerPayment =>
                    {
                        decimal paymentAndPenaltyCost = waterCustomerPayment.Cost + waterCustomerPayment.Penalty;

                        return new WaterCustomersReportItem(
                            waterCustomerPayment.CreateDate,
                            waterCustomerPayment.WaterCustomer.Number,
                            waterCustomerPayment.CounterValue1,
                            waterCustomerPayment.CounterValue2,
                            waterCustomerPayment.CounterValue3,
                            waterCustomerPayment.Cost,
                            waterCustomerPayment.Penalty,
                            paymentAndPenaltyCost,
                            waterCustomerPayment.CommissionValue,
                            paymentAndPenaltyCost + waterCustomerPayment.CommissionValue);
                        });

            int customerPaymentModelsCount = customerPaymentModels.Count();
            if (customerPaymentModelsCount == 0)
                return new ExportResult();

            var reportModel = new WaterCustomersReport(Settings.WaterСommissionPercent, customerPaymentModels); //TODO: коммисия от платежа к платежу может быть разная

            var wordReport = new WordReportController(Config.WaterCustomersReportTemplateFilename);
            wordReport.CreateReport(reportModel);

            return new ExportResult(customerPaymentModelsCount, waterCustomerPayments.Count() - customerPaymentModelsCount);
        }
    }
}
