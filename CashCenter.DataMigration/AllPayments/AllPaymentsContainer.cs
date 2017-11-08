using CashCenter.Dal;
using System.Collections.Generic;

namespace CashCenter.DataMigration.AllPayments
{
    public class AllPaymentsContainer
    {
        public List<EnergyCustomerPayment> EnergyPayments { get; private set; }
        public List<WaterCustomerPayment> WaterPayments { get; private set; }
        public List<ArticleSale> ArticleSales { get; private set; }
        public List<GarbageCollectionPayment> GarbagePayments { get; private set; }
        public List<RepairPayment> RepairPayments { get; private set; }

        public int AllItemsCount
        {
            get
            {
                return EnergyPayments.Count + WaterPayments.Count +
                    ArticleSales.Count + GarbagePayments.Count + RepairPayments.Count;
            }
        }

        public AllPaymentsContainer(
            List<EnergyCustomerPayment> energyPayments,
            List<WaterCustomerPayment> waterPayments,
            List<ArticleSale> articleSales,
            List<GarbageCollectionPayment> garbagePayments,
            List<RepairPayment> repairPayments)
        {
            EnergyPayments = energyPayments;
            WaterPayments = waterPayments;
            ArticleSales = articleSales;
            GarbagePayments = garbagePayments;
            RepairPayments = repairPayments;
        }
    }
}
