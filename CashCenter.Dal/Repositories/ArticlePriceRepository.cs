using System.Data.Entity;

namespace CashCenter.Dal.Repositories
{
    public class ArticlePriceRepository : BaseEfRepository<ArticlePrice, CashCenterContext>
    {
        public ArticlePriceRepository(CashCenterContext context)
            : base(context)
        {
        }

        protected override DbSet<ArticlePrice> GetDdSet(CashCenterContext context)
        {
            return context.ArticlePrices;
        }
    }
}
