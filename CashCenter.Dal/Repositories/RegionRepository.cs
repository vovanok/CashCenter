using System.Data.Entity;

namespace CashCenter.Dal.Repositories
{
    public class RegionRepository : BaseEfRepository<Region, CashCenterContext>
    {
        public RegionRepository(CashCenterContext context)
            : base(context)
        {
        }

        protected override DbSet<Region> GetDdSet(CashCenterContext context)
        {
            return context.Regions;
        }
    }
}
