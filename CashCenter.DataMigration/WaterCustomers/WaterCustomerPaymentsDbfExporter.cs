using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CashCenter.Dal;
using CashCenter.Common;
using CashCenter.DataMigration.Providers.Dbf;
using CashCenter.DataMigration.Providers.Dbf.Entities;

namespace CashCenter.DataMigration.WaterCustomers
{
    public class WaterCustomerPaymentsDbfExporter : BaseExporter<WaterCustomerPayment>
    {
        protected override List<WaterCustomerPayment> GetSourceItems(DateTime beginDatetime, DateTime endDatetime)
        {
            return RepositoriesFactory.Get<WaterCustomerPayment>().Filter(waterCustomerPayment =>
                beginDatetime <= waterCustomerPayment.CreateDate && waterCustomerPayment.CreateDate <= endDatetime).ToList();
        }

        protected override ExportResult TryExportItems(IEnumerable<WaterCustomerPayment> waterCustomerPayments)
        {
            if (waterCustomerPayments == null)
                return new ExportResult();

            var paymentsForExport = waterCustomerPayments
                .Where(waterCustomerPayment => waterCustomerPayment != null)
                .Select(waterCustomerPayment =>
                    new DbfWaterCustomerPayment(
                        waterCustomerPayment.CreateDate,
                        waterCustomerPayment.WaterCustomer.Number,
                        waterCustomerPayment.Cost,
                        waterCustomerPayment.CreateDate.ToString("yyyyMM"),
                        waterCustomerPayment.Penalty,
                        waterCustomerPayment.WaterCustomer.CounterNumber1,
                        0,
                        waterCustomerPayment.CounterValue1,
                        waterCustomerPayment.WaterCustomer.CounterNumber2,
                        0,
                        waterCustomerPayment.CounterValue2,
                        waterCustomerPayment.WaterCustomer.CounterNumber3,
                        0,
                        waterCustomerPayment.CounterValue3,
                        waterCustomerPayment.WaterCustomer.CounterNumber4,
                        0,
                        waterCustomerPayment.CounterValue4));

            var countItemsForExport = paymentsForExport.Count();
            if (countItemsForExport == 0)
                return new ExportResult();

            var dbfFilename = Path.Combine(Config.OutputDirectory, string.Format(Config.WaterCustomerDbfOutputFileFormat, DateTime.Now));

            Exception exportException = null;
            try
            {
                using (var fileBuffer = new FileBuffer(dbfFilename, FileBuffer.BufferType.Create))
                {
                    try
                    {
                        var dbfRegistry = new DbfRegistryController(fileBuffer.BufferFilename);
                        dbfRegistry.StoreWaterCustomerPayments(paymentsForExport);
                    }
                    catch (Exception ex)
                    {
                        exportException = ex;
                    }
                }
            }
            finally
            {
                if (exportException != null)
                    throw exportException;
            }

            return new ExportResult(countItemsForExport, waterCustomerPayments.Count() - countItemsForExport);
        }
    }
}
