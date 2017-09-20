using CashCenter.Common;
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
            string cashierName, decimal costWithoutComission, decimal comissionValue, decimal cost, string email)
            : base(
                  descriptorId: "WaterCustomer",
                  parameters: new Dictionary<string, string>
                  {
                      { "customerNumber", customerNumber.ToString() },
                      { "customerName", customerName },
                      { "cashierName", cashierName },
                      { "costWithoutComission", costWithoutComission.ToString("0.00") },
                      { "comissionValue", comissionValue.ToString("0.00") }
                  },
                  cost: cost,
                  totalCost: cost,
                  quantity: 1,
                  email: email,
                  paySection: 2,
                  ndsPercent: Config.WaterNdsPercent)
        {
        }
    }
}
