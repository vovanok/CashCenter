using CashCenter.Common;
using CashCenter.Dal;
using CashCenter.DataMigration.Providers.Dbf;
using CashCenter.DataMigration.Providers.Dbf.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CashCenter.DataMigration.GarbageAndRepair
{
    public class RepairPaymentsDbfExporter : BaseExporter<RepairPayment>
    {
        protected override List<RepairPayment> GetSourceItems(DateTime beginDatetime, DateTime endDatetime)
        {
            return DalController.Instance.RepairPayments.Where(repairPayment =>
                beginDatetime <= repairPayment.CreateDate && repairPayment.CreateDate <= endDatetime).ToList();
        }

        protected override ExportResult TryExportItems(IEnumerable<RepairPayment> repairPayments)
        {
            if (repairPayments == null)
                return new ExportResult();

            var paymentsForExport = repairPayments
                .Where(repairPayment => repairPayment != null)
                .Select(repairPayment =>
                    new DbfGarbageOrRepairPayment(
                        repairPayment.FinancialPeriodCode,
                        repairPayment.CreateDate,
                        repairPayment.CreateDate,
                        repairPayment.FilialCode,
                        repairPayment.OrganizationCode,
                        repairPayment.CustomerNumber,
                        Utils.RubToCopeck(repairPayment.Cost)));

            var countItemsForExport = paymentsForExport.Count();
            if (countItemsForExport == 0)
                return new ExportResult();

            var dbfFilename = Path.Combine(Config.OutputDirectory, string.Format(Config.RepairPaymentsDbfOutputFileFormat, DateTime.Now));

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

            return new ExportResult(countItemsForExport, repairPayments.Count() - countItemsForExport);
        }
    }
}
