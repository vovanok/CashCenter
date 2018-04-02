using System.Data.Entity;

namespace CashCenter.Dal.Repositories
{
    public class ArticleSaleRepository : BaseEfRepository<ArticleSale, CashCenterContext>
    {
        public ArticleSaleRepository(CashCenterContext context)
            : base(context)
        {
        }

        protected override DbSet<ArticleSale> GetDdSet(CashCenterContext context)
        {
            return context.ArticleSales;
        }
    }
}
