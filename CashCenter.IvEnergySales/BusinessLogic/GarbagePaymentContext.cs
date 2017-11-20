using System;
using CashCenter.Common;
using CashCenter.Dal;
using CashCenter.IvEnergySales.Check;

namespace CashCenter.IvEnergySales.BusinessLogic
{
    public class GarbagePaymentContext : GarbageAndRepairPaymentContext
    {
        public override int OrganizationCode => 1600;
        public override string PaymentName => "Вывоз ТКО";
        public override int FilialCode => Settings.GarbageCollectionFilialCode;
        public override float CommissionPercent => Settings.GarbageCollectionCommissionPercent;

        public override void StorePaymentToDb(int financialPeriodCode, DateTime createDate, int organizationCode, int filialCode, int customerNumber, decimal cost)
        {
            var payment = new GarbageCollectionPayment
            {
                FinancialPeriodCode = financialPeriodCode,
                CreateDate = createDate,
                OrganizationCode = organizationCode,
                FilialCode = filialCode,
                CustomerNumber = customerNumber,
                Cost = cost,
                CommissionValue = Utils.GetCommission(cost, Settings.GarbageCollectionCommissionPercent)
            };

            DalController.Instance.AddGarbageCollectionPayment(payment);
        }

        public override CashCenter.Check.Check GetCheck(int customerNumber, decimal costWithoutCommission, decimal commissionValue, decimal cost)
        {
            return new GarbageCheck(customerNumber, Settings.CasherName, costWithoutCommission, commissionValue, cost);
        }
    }
}
