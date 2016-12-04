using System.Collections.Generic;
using System.Linq;
using CashCenter.IvEnergySales.DAL;
using CashCenter.IvEnergySales.DbQualification;
using CashCenter.IvEnergySales.DataModel;

namespace CashCenter.IvEnergySales.BusinessLogic
{
    public class EnergySalesManager
    {
        private List<DbController> dbControllers;

        public EnergySalesManager(DepartmentModel department)
        {
            dbControllers = department.Dbs.Select(dbModel => new DbController(dbModel)).ToList();
        }

        public List<PaymentReason> GetPaymentReasons(string dbCode)
        {
            foreach (var dbController in GetDbControllersByDbCode(dbCode))
            {
                var paymentReasons = dbController.GetPaymentReasons();
                if (paymentReasons != null && paymentReasons.Count > 0)
                    return paymentReasons;
            }

            return new List<PaymentReason>();
        }

        public Customer GetCustomer(string dbCode, int customerId)
        {
            foreach(var dbController in GetDbControllersByDbCode(dbCode))
            {
                var customer = dbController.GetCustomer(customerId);
                if (customer != null)
                    return customer;
            }

            return null;
        }

        private List<DbController> GetDbControllersByDbCode(string dbCode)
        {
            return dbControllers.Where(dbController => dbController.Model.DbCode == dbCode).ToList();
        }
    }
}
