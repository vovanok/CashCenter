using System;
using System.Collections.Generic;
using System.Linq;
using CashCenter.Common;
using CashCenter.Dal;
using CashCenter.DataMigration.Providers.Rkc;
using CashCenter.DataMigration.Providers.Rkc.Entities;

namespace CashCenter.DataMigration.AllPayments
{
    public class AllPaymentsRkcExporter : BaseExporter<AllPaymentsContainer>
    {
        private RkcController outputController = new RkcController();

        protected override List<AllPaymentsContainer> GetSourceItems(DateTime beginDatetime, DateTime endDatetime)
        {
            return new List<AllPaymentsContainer>
            {
                new AllPaymentsContainer(
                    DalController.Instance.EnergyCustomerPayments.Where(customerPayment =>
                        beginDatetime <= customerPayment.CreateDate && customerPayment.CreateDate <= endDatetime).ToList(),
                    DalController.Instance.WaterCustomerPayments.Where(customerPayment =>
                        beginDatetime <= customerPayment.CreateDate && customerPayment.CreateDate <= endDatetime).ToList(),
                    DalController.Instance.ArticleSales.Where(customerPayment =>
                        beginDatetime <= customerPayment.CreateDate && customerPayment.CreateDate <= endDatetime).ToList(),
                    DalController.Instance.GarbageCollectionPayments.Where(customerPayment =>
                        beginDatetime <= customerPayment.CreateDate && customerPayment.CreateDate <= endDatetime).ToList(),
                    DalController.Instance.RepairPayments.Where(customerPayment =>
                        beginDatetime <= customerPayment.CreateDate && customerPayment.CreateDate <= endDatetime).ToList())
            };
        }

        protected override ExportResult TryExportItems(IEnumerable<AllPaymentsContainer> items)
        {
            if (items == null)
                return new ExportResult();

            var container = items.FirstOrDefault();
            if (container == null)
                return new ExportResult();

            var itemsForStore = new List<RkcAllPaymentsItem>();

            itemsForStore.AddRange(container.EnergyPayments.Where(item => item != null)
                .Select(item => new RkcAllPaymentsItem(
                    RkcAllPaymentsItem.ItemType.Payment,
                    1,
                    string.Empty,
                    string.Empty,
                    Settings.ArticlesWarehouseCode,
                    item.CreateDate,
                    item.Cost,
                    0,
                    RkcAllPaymentsItem.PaymentTarget.Energy)));

            itemsForStore.AddRange(container.WaterPayments.Where(item => item != null)
                .Select(item => new RkcAllPaymentsItem(
                    RkcAllPaymentsItem.ItemType.Payment,
                    2,
                    string.Empty,
                    string.Empty,
                    Settings.ArticlesWarehouseCode,
                    item.CreateDate,
                    item.Cost + item.Penalty,
                    item.CommissionValue,
                    RkcAllPaymentsItem.PaymentTarget.Water)));

            itemsForStore.AddRange(container.ArticleSales.Where(item => item != null)
                .Select(item => new RkcAllPaymentsItem(
                    RkcAllPaymentsItem.ItemType.Payment,
                    1,
                    string.Empty,
                    string.Empty,
                    Settings.ArticlesWarehouseCode,
                    item.CreateDate,
                    item.ArticlePrice.Value,
                    0,
                    RkcAllPaymentsItem.PaymentTarget.Articles)));

            itemsForStore.AddRange(container.GarbagePayments.Where(item => item != null)
                .Select(item => new RkcAllPaymentsItem(
                    RkcAllPaymentsItem.ItemType.Payment,
                    3,
                    string.Empty,
                    string.Empty,
                    Settings.ArticlesWarehouseCode,
                    item.CreateDate,
                    item.Cost,
                    item.CommissionValue,
                    RkcAllPaymentsItem.PaymentTarget.Garbage)));

            itemsForStore.AddRange(container.RepairPayments.Where(item => item != null)
                .Select(item => new RkcAllPaymentsItem(
                    RkcAllPaymentsItem.ItemType.Payment,
                    3,
                    string.Empty,
                    string.Empty,
                    Settings.ArticlesWarehouseCode,
                    item.CreateDate,
                    item.Cost,
                    item.CommissionValue,
                    RkcAllPaymentsItem.PaymentTarget.Repair)));

            outputController.StoreItems(itemsForStore);
            int itemsCount = itemsForStore.Count;
            return new ExportResult(itemsCount, container.AllItemsCount - itemsCount);
        }
    }
}
