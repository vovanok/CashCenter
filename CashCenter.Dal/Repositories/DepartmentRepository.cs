using System.Data.Entity;

namespace CashCenter.Dal.Repositories
{
    public class DepartmentRepository : BaseEfRepository<Department, CashCenterContext>
    {
        public DepartmentRepository(CashCenterContext context)
            : base(context)
        {
        }

        protected override DbSet<Department> GetDdSet(CashCenterContext context)
        {
            return context.Departments;
        }
    }
}
