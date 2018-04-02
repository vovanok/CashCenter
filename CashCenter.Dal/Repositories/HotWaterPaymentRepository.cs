using System.Data.Entity;

namespace CashCenter.Dal.Repositories
{
    public class HotWaterPaymentRepository : BaseEfRepository<HotWaterPayment, CashCenterContext>
    {
        public HotWaterPaymentRepository(CashCenterContext context) 
            : base(context)
        {
        }

        protected override DbSet<HotWaterPayment> GetDdSet(CashCenterContext context)
        {
            return context.HotWaterPayments;
        }
    }
}
