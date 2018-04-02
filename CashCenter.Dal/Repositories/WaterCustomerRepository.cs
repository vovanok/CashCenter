using System.Data.Entity;

namespace CashCenter.Dal.Repositories
{
    public class WaterCustomerRepository : BaseEfRepository<WaterCustomer, CashCenterContext>
    {
        public WaterCustomerRepository(CashCenterContext context) 
            : base(context)
        {
        }

        protected override DbSet<WaterCustomer> GetDdSet(CashCenterContext context)
        {
            return context.WaterCustomers;
        }
    }
}
