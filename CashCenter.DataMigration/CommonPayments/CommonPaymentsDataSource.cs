using CashCenter.Dal;
using System.Collections.Generic;

namespace CashCenter.DataMigration.CommonPayments
{
    public class CommonPaymentsDataSource
    {
        public List<WaterCustomerPayment> WaterCustomersPayments { get; private set; }
        public List<EnergyCustomerPayment> EnergyCustomersPayments { get; private set; }
        public List<GarbageCollectionPayment> GarbagePayments { get; private set; }
        public List<RepairPayment> RepairPayments { get; private set; }
        public List<HotWaterPayment> HotWaterPayments { get; private set; }

        public int AllPaymentsCount
        {
            get { return WaterCustomersPayments.Count + EnergyCustomersPayments.Count + GarbagePayments.Count + RepairPayments.Count + HotWaterPayments.Count; }
        }

        public CommonPaymentsDataSource(
            List<WaterCustomerPayment> waterCustomersPayments, List<EnergyCustomerPayment> energyCustomersPayments,
            List<GarbageCollectionPayment> garbagePayments, List<RepairPayment> repairPayments, List<HotWaterPayment> hotWaterPayments)
        {
            WaterCustomersPayments = waterCustomersPayments;
            EnergyCustomersPayments = energyCustomersPayments;
            GarbagePayments = garbagePayments;
            RepairPayments = repairPayments;
            HotWaterPayments = hotWaterPayments;
        }
    }
}
