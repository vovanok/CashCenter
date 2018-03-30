using System;
using CashCenter.Common;
using CashCenter.Dal;

namespace CashCenter.Objective.GarbageAndRepair
{
    public class RepairPaymentContext : GarbageAndRepairPaymentContext
    {
        public override int OrganizationCode => 1500;
        public override string PaymentName => "Кап. ремонт";
        public override int FilialCode => Settings.RepairFilialCode;
        public override float CommissionPercent => Settings.RepairCommissionPercent;

        public override void StorePaymentToDb(int financialPeriodCode, DateTime createDate, int organizationCode, int filialCode, int customerNumber, decimal cost)
        {
            var payment = new RepairPayment
            {
                FinancialPeriodCode = financialPeriodCode,
                CreateDate = createDate,
                OrganizationCode = organizationCode,
                FilialCode = filialCode,
                CustomerNumber = customerNumber,
                Cost = cost,
                CommissionValue = Utils.GetCommission(cost, Settings.RepairCommissionPercent)
            };

            DalController.Instance.AddRepairPayment(payment);
        }

        public override CashCenter.Check.Check GetCheck(int customerNumber, decimal costWithoutCommission, decimal commissionValue, decimal cost)
        {
            return new RepairCheck(customerNumber, Settings.CasherName, costWithoutCommission, commissionValue, cost);
        }
    }
}
