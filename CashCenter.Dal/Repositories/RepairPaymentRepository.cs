using System.Data.Entity;

namespace CashCenter.Dal.Repositories
{
    public class RepairPaymentRepository : BaseEfRepository<RepairPayment, CashCenterContext>
    {
        public RepairPaymentRepository(CashCenterContext context)
            : base(context)
        {
        }

        protected override DbSet<RepairPayment> GetDdSet(CashCenterContext context)
        {
            return context.RepairPayments;
        }
    }
}
