using CashCenter.Dal;
using System.Collections.Generic;

namespace CashCenter.DataMigration.CommonPayments
{
    public class CommonPaymentsDataSource
    {
        public List<WaterCustomerPayment> WaterCustomersPayments { get; private set; }
        public List<EnergyCustomerPayment> EnergyCustomersPayments { get; private set; }
        public int AllPaymentsCount { get { return WaterCustomersPayments.Count + EnergyCustomersPayments.Count; } }

        public CommonPaymentsDataSource(List<WaterCustomerPayment> waterCustomersPayments, List<EnergyCustomerPayment> energyCustomersPayments)
        {
            WaterCustomersPayments = waterCustomersPayments;
            EnergyCustomersPayments = energyCustomersPayments;
        }
    }
}
