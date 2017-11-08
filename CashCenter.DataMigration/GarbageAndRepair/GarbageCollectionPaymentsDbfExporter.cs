using CashCenter.Dal;
using System;
using System.Linq;
using System.Collections.Generic;
using CashCenter.DataMigration.Providers.Dbf.Entities;
using System.IO;
using CashCenter.Common;
using CashCenter.DataMigration.Providers.Dbf;

namespace CashCenter.DataMigration.GarbageAndRepair
{
    public class GarbageCollectionPaymentsDbfExporter : BaseExporter<GarbageCollectionPayment>
    {
        protected override List<GarbageCollectionPayment> GetSourceItems(DateTime beginDatetime, DateTime endDatetime)
        {
            return DalController.Instance.GarbageCollectionPayments.Where(garbagePayment =>
                beginDatetime <= garbagePayment.CreateDate && garbagePayment.CreateDate <= endDatetime).ToList();
        }

        protected override ExportResult TryExportItems(IEnumerable<GarbageCollectionPayment> garbageCollectionPayments)
        {
            if (garbageCollectionPayments == null)
                return new ExportResult();

            var paymentsForExport = garbageCollectionPayments
                .Where(garbageCollectionPayment => garbageCollectionPayment != null)
                .Select(garbageCollectionPayment =>
                    new DbfGarbageOrRepairPayment(
                        garbageCollectionPayment.FinancialPeriodCode,
                        garbageCollectionPayment.CreateDate,
                        garbageCollectionPayment.CreateDate,
                        garbageCollectionPayment.FilialCode,
                        garbageCollectionPayment.OrganizationCode,
                        garbageCollectionPayment.CustomerNumber,
                        Utils.RubToCopeck(garbageCollectionPayment.Cost)));

            var countItemsForExport = paymentsForExport.Count();
            if (countItemsForExport == 0)
                return new ExportResult();

            var dbfFilename = Path.Combine(Config.OutputDirectory, string.Format(Config.GarbageCollectionPaymentsDbfOutputFileFormat, DateTime.Now));

            Exception exportException = null;
            try
            {
                using (var fileBuffer = new FileBuffer(dbfFilename, FileBuffer.BufferType.Create))
                {
                    try
                    {
                        var dbfRegistry = new DbfRegistryController(fileBuffer.BufferFilename);
                        dbfRegistry.StoreGarbageCollectionPayments(paymentsForExport);
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

            return new ExportResult(countItemsForExport, garbageCollectionPayments.Count() - countItemsForExport);
        }
    }
}
