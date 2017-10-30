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

            return new List<CommonPaymentsDataSource>
                { new CommonPaymentsDataSource(waterCustomersPayments, energyCustomersPayments) };
        }

        protected override ExportResult TryExportItems(IEnumerable<CommonPaymentsDataSource> waterAndEnergyCustomerPaymentsDataSources)
        {
            if (waterAndEnergyCustomerPaymentsDataSources == null)
                return new ExportResult();

            var waterAndEnergyCustomerPayments = waterAndEnergyCustomerPaymentsDataSources.FirstOrDefault();
            if (waterAndEnergyCustomerPayments == null)
                return new ExportResult();

            var waterAndEnergyCustomersPaymentsItemModels = new List<ReportWaterAndEnergyCustomersPaymentsItemModel>();
            for (DateTime currentDate = beginDatetime.DayBegin(); currentDate <= endDatetime; currentDate = currentDate.AddDays(1))
            {
                waterAndEnergyCustomersPaymentsItemModels.Add(
                    new ReportWaterAndEnergyCustomersPaymentsItemModel(currentDate, 0, 0, 0));
            }

            int countItems = 0;
            decimal totalEnergyCost = 0;
            decimal totalWaterWithoutComissionCost = 0;
            decimal totalWaterComissionCost = 0;

            foreach (var waterCustomerPayment in waterAndEnergyCustomerPayments.WaterCustomersPayments)
            {
                if (waterCustomerPayment == null)
                    continue;

                var targetModelItem = waterAndEnergyCustomersPaymentsItemModels
                    .FirstOrDefault(itemModel => itemModel.Date == waterCustomerPayment.CreateDate.DayBegin());
                if (targetModelItem == null)
                    continue;

                targetModelItem.WaterWithoutComissionCost += waterCustomerPayment.Cost;
                totalWaterWithoutComissionCost += waterCustomerPayment.Cost;

                decimal comissionCost = waterCustomerPayment.Cost * (decimal)(waterCustomerPayment.ComissionPercent / 100);
                targetModelItem.WaterComissionCost += comissionCost;
                totalWaterComissionCost += comissionCost;

                countItems++;
            }

            foreach (var energyCustomerPayment in waterAndEnergyCustomerPayments.EnergyCustomersPayments)
            {
                if (energyCustomerPayment == null)
                    continue;
                
                var targetModelItem = waterAndEnergyCustomersPaymentsItemModels
                    .FirstOrDefault(itemModel => itemModel.Date == energyCustomerPayment.CreateDate.DayBegin());
                if (targetModelItem == null)
                    continue;

                targetModelItem.EnergyCost += energyCustomerPayment.Cost;
                totalEnergyCost += energyCustomerPayment.Cost;

                countItems++;
            }

            if (countItems == 0)
                return new ExportResult(0, waterAndEnergyCustomerPayments.AllPaymentsCount);

            var reportModel = new ReportWaterAndEnergyCustomersPaymentsModel(beginDatetime, endDatetime,
                waterAndEnergyCustomersPaymentsItemModels, totalEnergyCost, totalWaterWithoutComissionCost, totalWaterComissionCost);

            var wordReport = new WordReportController(Config.CommonPaymentsReportTemplateFilename);
            wordReport.CreateReport(reportModel);

            return new ExportResult(countItems, waterAndEnergyCustomerPayments.AllPaymentsCount - countItems);
        }
    }
}