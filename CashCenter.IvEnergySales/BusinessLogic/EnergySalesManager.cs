using System.Collections.Generic;
using System.Linq;
using CashCenter.IvEnergySales.DAL;
using CashCenter.IvEnergySales.DbQualification;
using CashCenter.IvEnergySales.DataModel;

namespace CashCenter.IvEnergySales.BusinessLogic
{
    public class EnergySalesManager
    {
        private DepartmentModel department;
        private List<DbController> dbControllers = new List<DbController>();

        public EnergySalesManager(DepartmentModel department)
        {
            this.department = department;
        }

        public void SetDbCode(string dbCode)
        {
            dbControllers = GetDbControllersByDbCode(dbCode);
        }

        public List<PaymentReason> GetPaymentReasons()
        {
            foreach (var dbController in dbControllers)
            {
                var paymentReasons = dbController.GetPaymentReasons();
                if (paymentReasons != null && paymentReasons.Count > 0)
                    return paymentReasons;
            }

            return new List<PaymentReason>();
        }

        public Customer GetCustomer(int customerId)
        {
            foreach(var dbController in dbControllers)
            {
                var customer = dbController.GetCustomer(customerId);
                if (customer != null)
                    return customer;
            }

            return null;
        }

        private List<DbController> GetDbControllersByDbCode(string dbCode)
        {
            return department.Dbs.Select(dbModel => new DbController(dbModel))
                .Where(dbController => dbController.Model.DbCode == dbCode).ToList();
        }
    }
}
