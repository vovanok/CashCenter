using CashCenter.Dal;
using System.Collections.Generic;
using System.Linq;

namespace CashCenter.IvEnergySales.BusinessLogic
{
    public abstract class BaseOrganizationSalesContext
    {
        protected const string ERROR_PREFIX = "Ошибка совершения платежа.";

        public IEnumerable<Organization> Organizations { get; protected set; }
        public InfoForCheck InfoForCheck { get; protected set; }

        public bool IsOraganizationFinded
        {
            get { return Organizations != null && Organizations.Count() > 0; }
        }

        public BaseOrganizationSalesContext(string contractNumberPart,
            string namePart, string innPart)
        {
        }

        public abstract bool Pay(OrganizationPayment payment);
    }
}
