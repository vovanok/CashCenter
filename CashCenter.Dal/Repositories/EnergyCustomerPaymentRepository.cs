using System.Data.Entity;

namespace CashCenter.Dal.Repositories
{
    public class EnergyCustomerPaymentRepository : BaseEfRepository<EnergyCustomerPayment, CashCenterContext>
    {
        public EnergyCustomerPaymentRepository(CashCenterContext context)
            : base(context)
        {
        }

        protected override DbSet<EnergyCustomerPayment> GetDdSet(CashCenterContext context)
        {
            return context.EnergyCustomerPayments;
        }
    }
}
