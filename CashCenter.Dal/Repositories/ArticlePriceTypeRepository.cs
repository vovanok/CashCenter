using System.Data.Entity;

namespace CashCenter.Dal.Repositories
{
    public class ArticlePriceTypeRepository : BaseEfRepository<ArticlePriceType, CashCenterContext>
    {
        public ArticlePriceTypeRepository(CashCenterContext context)
            : base(context)
        {
        }

        protected override DbSet<ArticlePriceType> GetDdSet(CashCenterContext context)
        {
            return context.ArticlePriceTypes;
        }
    }
}
