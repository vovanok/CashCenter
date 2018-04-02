using System.Data.Entity;

namespace CashCenter.Dal.Repositories
{
    public class HotWaterCustomerRepository : BaseEfRepository<HotWaterCustomer, CashCenterContext>
    {
        public HotWaterCustomerRepository(CashCenterContext context)
            : base(context)
        {
        }

        protected override DbSet<HotWaterCustomer> GetDdSet(CashCenterContext context)
        {
            return context.HotWaterCustomers;
        }
    }
}
