using System.Data.Entity;

namespace CashCenter.Dal.Repositories
{
    public class EnergyCustomerRepository : BaseEfRepository<EnergyCustomer, CashCenterContext>
    {
        public EnergyCustomerRepository(CashCenterContext context)
            : base(context)
        {
        }

        protected override DbSet<EnergyCustomer> GetDdSet(CashCenterContext context)
        {
            return context.EnergyCustomers;
        }
    }
}
