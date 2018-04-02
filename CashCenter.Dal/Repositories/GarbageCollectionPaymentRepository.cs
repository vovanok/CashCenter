using System.Data.Entity;

namespace CashCenter.Dal.Repositories
{
    public class GarbageCollectionPaymentRepository : BaseEfRepository<GarbageCollectionPayment, CashCenterContext>
    {
        public GarbageCollectionPaymentRepository(CashCenterContext context)
            : base(context)
        {
        }

        protected override DbSet<GarbageCollectionPayment> GetDdSet(CashCenterContext context)
        {
            return context.GarbageCollectionPayments;
        }
    }
}
