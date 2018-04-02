using System.Data.Entity;

namespace CashCenter.Dal.Repositories
{
    public class ArticleRepository : BaseEfRepository<Article, CashCenterContext>
    {
        public ArticleRepository(CashCenterContext context)
            : base(context)
        {
        }

        protected override DbSet<Article> GetDdSet(CashCenterContext context)
        {
            return context.Articles;
        }
    }
}
