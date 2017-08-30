using CashCenter.Common;
using CashCenter.Dal;
using CashCenter.DataMigration.Providers.Word;
using CashCenter.DataMigration.Providers.Word.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CashCenter.DataMigration.WaterCustomers
{
    public class WaterCustomerPaymentsWordExporter : BaseExporter<WaterCustomerPayment>
    {
        protected override List<WaterCustomerPayment> GetSourceItems(DateTime beginDatetime, DateTime endDatetime)
        {
            return DalController.Instance.WaterCustomerPayments.Where(waterCustomerPayment =>
                beginDatetime <= waterCustomerPayment.CreateDate && waterCustomerPayment.CreateDate <= endDatetime).ToList();
        }

        protected override int TryExportItems(IEnumerable<WaterCustomerPayment> waterCustomerPayments)
        {
            if (waterCustomerPayments == null)
                return 0;

            var customerPaymentModels = waterCustomerPayments
                .Where(waterCustomerPayment => waterCustomerPayment != null)
                .Select(waterCustomerPayment =>
                    {
                        decimal paymentAndPenaltyCost = waterCustomerPayment.Cost + waterCustomerPayment.Penalty;
                        decimal comissionCost = paymentAndPenaltyCost * (decimal)(waterCustomerPayment.ComissionPercent / 100);

                        return new ReportWaterCustomerPaymentModel(
                            waterCustomerPayment.CreateDate,
                            waterCustomerPayment.WaterCustomer.Number,
                            waterCustomerPayment.CounterValue1,
                            waterCustomerPayment.CounterValue2,
                            waterCustomerPayment.CounterValue3,
                            waterCustomerPayment.Cost,
                            waterCustomerPayment.Penalty,
                            paymentAndPenaltyCost,
                            comissionCost,
                            paymentAndPenaltyCost + comissionCost);
                        });

            int customerPaymentModelsCount = customerPaymentModels.Count();
            if (customerPaymentModelsCount == 0)
                return 0;

            var reportModel = new ReportWaterCustomersModel(Settings.WaterСommissionPercent, customerPaymentModels); //TODO: коммисия от платежа к платежу может быть разная

            var wordReport = new WordReportController(Config.WaterCustomersReportTemplateFilename);
            wordReport.CreateReport(reportModel);

            return customerPaymentModelsCount;
        }
    }
}
