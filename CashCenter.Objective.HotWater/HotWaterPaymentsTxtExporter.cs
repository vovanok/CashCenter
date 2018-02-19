using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CashCenter.Common;
using CashCenter.Dal;
using CashCenter.DataMigration;
using CashCenter.DataMigration.Providers.Csv;

namespace CashCenter.Objective.HotWater
{
    public class HotWaterPaymentsTxtExporter : BaseExporter<HotWaterPayment>
    {
        private CashCenterContext db = new CashCenterContext();

        protected override List<HotWaterPayment> GetSourceItems(DateTime beginDatetime, DateTime endDatetime)
        {
            return db.HotWaterPayments.Where(customerPayment =>
                beginDatetime <= customerPayment.CreateDate && customerPayment.CreateDate <= endDatetime).ToList();
        }

        protected override ExportResult TryExportItems(IEnumerable<HotWaterPayment> hotWaterCustomerPayments)
        {
            if (hotWaterCustomerPayments == null)
                return new ExportResult();

            IEnumerable<IEnumerable<string>> rows = hotWaterCustomerPayments
                .Where(payment => payment != null)
                .Select(payment =>
                    new[]
                    {
                        payment.CreateDate.ToString("dd-MM-yyyy"),
                        payment.CreateDate.ToString("hh-mm-ss"),
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        payment.HotWaterCustomer.Number.ToString(),
                        payment.HotWaterCustomer.Name,
                        payment.HotWaterCustomer.Address,
                        payment.CreateDate.ToString("MMyy"),
                        (payment.Total + payment.CommisionTotal).ToString("0.00"),
                        payment.Total.ToString("0.00"),
                        payment.CommisionTotal.ToString("0.00")
                    });

            var countItemsForExport = rows.Count();
            if (countItemsForExport == 0)
                return new ExportResult();

            decimal sumTotalWithCommission = hotWaterCustomerPayments.Sum(payment => payment.Total + payment.CommisionTotal);
            decimal sumTotal = hotWaterCustomerPayments.Sum(payment => payment.Total);
            decimal sumCommissionTotal = hotWaterCustomerPayments.Sum(payment => payment.CommisionTotal);

            rows = rows.Concat(new[]
                {
                    new[]
                    {
                        $"={rows.Count()}",
                        sumTotalWithCommission.ToString("0.00"),
                        sumTotal.ToString("0.00"),
                        sumCommissionTotal.ToString("0.00"),
                        string.Empty,
                        DateTime.Now.ToString("dd-MM-yyyy")
                    }
                });

            var filename = Path.Combine(Config.OutputDirectory,
                string.Format(Config.HotWaterPaymentTxtOutputFileFormat, DateTime.Now));

            Exception exportException = null;
            try
            {
                var txtRegistry = new CsvController(filename);
                txtRegistry.SaveRows(rows);
            }
            finally
            {
                if (exportException != null)
                    throw exportException;
            }

            return new ExportResult(countItemsForExport, hotWaterCustomerPayments.Count() - countItemsForExport);
        }
    }
}
