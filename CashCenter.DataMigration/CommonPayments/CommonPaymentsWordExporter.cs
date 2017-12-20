using System;
using System.Linq;
using System.Collections.Generic;
using CashCenter.Common;
using CashCenter.Dal;
using CashCenter.DataMigration.Providers.Word;
using CashCenter.DataMigration.Providers.Word.Entities;
using CashCenter.DataMigration.CommonPayments;

namespace CashCenter.DataMigration.WaterAndEnergyCustomers
{
    public class CommonPaymentsWordExporter : BaseExporter<CommonPaymentsDataSource>
    {
        protected override List<CommonPaymentsDataSource> GetSourceItems(DateTime beginDatetime, DateTime endDatetime)
        {
            var waterCustomersPayments = DalController.Instance.WaterCustomerPayments.Where(waterCustomerPayment =>
                beginDatetime <= waterCustomerPayment.CreateDate && waterCustomerPayment.CreateDate <= endDatetime).ToList();

            var energyCustomersPayments = DalController.Instance.EnergyCustomerPayments.Where(energyCustomerPayment =>
                beginDatetime <= energyCustomerPayment.CreateDate && energyCustomerPayment.CreateDate <= endDatetime).ToList();

            var garbagePayments = DalController.Instance.GarbageCollectionPayments.Where(garbagePayment =>
                beginDatetime <= garbagePayment.CreateDate && garbagePayment.CreateDate <= endDatetime).ToList();

            var repairPayments = DalController.Instance.RepairPayments.Where(repairPayment =>
                beginDatetime <= repairPayment.CreateDate && repairPayment.CreateDate <= endDatetime).ToList();

            return new List<CommonPaymentsDataSource>
                { new CommonPaymentsDataSource(waterCustomersPayments, energyCustomersPayments, garbagePayments, repairPayments) };
        }

        protected override ExportResult TryExportItems(IEnumerable<CommonPaymentsDataSource> commonPaymentsDataSources)
        {
            if (commonPaymentsDataSources == null)
                return new ExportResult();

            var commonPayments = commonPaymentsDataSources.FirstOrDefault();
            if (commonPayments == null)
                return new ExportResult();

            var commonPaymentsItemModels = new List<CommonPaymentsItemModel>();
            for (DateTime currentDate = beginDatetime.DayBegin(); currentDate <= endDatetime; currentDate = currentDate.AddDays(1))
            {
                commonPaymentsItemModels.Add(
                    new CommonPaymentsItemModel(currentDate));
            }

            int countItems = 0;
            decimal finalEnergyTotal = 0;
            decimal finalWaterWithoutCommissionTotal = 0;
            decimal finalWaterCommissionTotal = 0;
            decimal finalGarbageWithoutComissionTotal = 0;
            decimal finalGarbageCommissionTotal = 0;
            decimal finalRepairWithoutCommissionTotal = 0;
            decimal finalRepairCommissionTotal = 0;

            foreach (var energyCustomerPayment in commonPayments.EnergyCustomersPayments)
            {
                if (energyCustomerPayment == null)
                    continue;

                var targetModelItem = commonPaymentsItemModels
                    .FirstOrDefault(itemModel => itemModel.Date == energyCustomerPayment.CreateDate.DayBegin());
                if (targetModelItem == null)
                    continue;

                targetModelItem.EnergyTotal += energyCustomerPayment.Cost;
                finalEnergyTotal += energyCustomerPayment.Cost;

                countItems++;
            }

            foreach (var waterCustomerPayment in commonPayments.WaterCustomersPayments)
            {
                if (waterCustomerPayment == null)
                    continue;

                var targetModelItem = commonPaymentsItemModels
                    .FirstOrDefault(itemModel => itemModel.Date == waterCustomerPayment.CreateDate.DayBegin());
                if (targetModelItem == null)
                    continue;

                targetModelItem.WaterWithoutCommissionTotal += waterCustomerPayment.Cost + waterCustomerPayment.Penalty;
                finalWaterWithoutCommissionTotal += waterCustomerPayment.Cost + waterCustomerPayment.Penalty;

                targetModelItem.WaterCommissionTotal += waterCustomerPayment.CommissionValue;
                finalWaterCommissionTotal += waterCustomerPayment.CommissionValue;

                countItems++;
            }
            
            foreach (var garbagePayment in commonPayments.GarbagePayments)
            {
                if (garbagePayment == null)
                    continue;

                var targetModelItem = commonPaymentsItemModels
                    .FirstOrDefault(itemModel => itemModel.Date == garbagePayment.CreateDate.DayBegin());
                if (targetModelItem == null)
                    continue;

                targetModelItem.GarbageWithoutCommissionTotal += garbagePayment.Cost;
                finalGarbageWithoutComissionTotal += garbagePayment.Cost;

                targetModelItem.GarbageCommissionTotal += garbagePayment.CommissionValue;
                finalGarbageCommissionTotal += garbagePayment.CommissionValue;

                countItems++;
            }

            foreach (var repairPayment in commonPayments.RepairPayments)
            {
                if (repairPayment == null)
                    continue;

                var targetModelItem = commonPaymentsItemModels
                    .FirstOrDefault(itemModel => itemModel.Date == repairPayment.CreateDate.DayBegin());
                if (repairPayment == null)
                    continue;

                targetModelItem.RepairWithoutCommissionTotal += repairPayment.Cost;
                finalRepairWithoutCommissionTotal += repairPayment.Cost;

                targetModelItem.RepairCommissionTotal += repairPayment.CommissionValue;
                finalRepairCommissionTotal += repairPayment.CommissionValue;

                countItems++;
            }

            if (countItems == 0)
                return new ExportResult(0, commonPayments.AllPaymentsCount);

            var reportModel = new CommonPaymentsModel(beginDatetime, endDatetime, commonPaymentsItemModels,
                finalEnergyTotal,
                finalWaterWithoutCommissionTotal, finalWaterCommissionTotal,
                finalGarbageWithoutComissionTotal, finalGarbageCommissionTotal,
                finalRepairWithoutCommissionTotal, finalRepairCommissionTotal);

            var wordReport = new WordReportController(Config.CommonPaymentsReportTemplateFilename);
            wordReport.CreateReport(reportModel);

            return new ExportResult(countItems, commonPayments.AllPaymentsCount - countItems);
        }
    }
}