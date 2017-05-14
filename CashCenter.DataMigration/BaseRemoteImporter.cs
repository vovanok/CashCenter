using CashCenter.Dal;
using CashCenter.DataMigration.Providers.Firebird;

namespace CashCenter.DataMigration
{
    public abstract class BaseRemoteImporter<TSource, TTarget> : BaseImporter<TSource, TTarget>, IRemoteImporter
        where TSource : class
        where TTarget : class
    {
        protected ZeusDbController db;
        private Department sourceDepartment;

        public Department SourceDepartment
        {
            get { return sourceDepartment; }
            set
            {
                sourceDepartment = value;

                db = (sourceDepartment != null)
                    ? new ZeusDbController(sourceDepartment.Code, sourceDepartment.Url, sourceDepartment.Path)
                    : null;
            }
        }
    }
}
