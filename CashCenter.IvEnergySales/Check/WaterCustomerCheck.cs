using System.Collections.Generic;

namespace CashCenter.IvEnergySales.Check
{
    public class WaterCustomerCheck : CashCenter.Check.Check
    {
        public string SalesDepartmentInfo { get; private set; }
        public string DepartmentCode { get; private set; }
        public int CustomerId { get; private set; }
        public string CustomerName { get; private set; }
        public string PaymentReason { get; private set; }
        public string CashierName { get; private set; }

        public WaterCustomerCheck(int customerNumber, string customerName,
            string cashierName, decimal cost, string email)
            : base(
                  "WaterCustomer",
                  new Dictionary<string, string>
                  {
                      { "customerNumber", customerNumber.ToString() },
                      { "customerName", customerName },
                      { "cashierName", cashierName }
                  },
                  cost,
                  cost,
                  1,
                  email,
                  2)
        {
        }
    }
}
