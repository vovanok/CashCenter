using System.Data.Entity;

namespace CashCenter.Dal.Repositories
{
    public class PaymentReasonRepository : BaseEfRepository<PaymentReason, CashCenterContext>
    {
        public PaymentReasonRepository(CashCenterContext context)
            : base(context)
        {
        }

        protected override DbSet<PaymentReason> GetDdSet(CashCenterContext context)
        {
            return context.PaymentReasons;
        }
    }
}