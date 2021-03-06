using CashCenter.Common;
using System.Collections.Generic;

namespace CashCenter.Objective.Energy
{
    public class EnergyCustomerCheck : CashCenter.Check.Check
    {
        public string SalesDepartmentInfo { get; private set; }
        public string DepartmentCode { get; private set; }
        public int CustomerId { get; private set; }
        public string CustomerName { get; private set; }
        public string PaymentReason { get; private set; }
        public string CashierName { get; private set; }

        public EnergyCustomerCheck(string departmentCode, int customerNumber, string customerName,
            string paymentReason, string cashierName, decimal cost, string email)
            : base(
                  descriptorId: "EnergyCustomer",
                  parameters: new Dictionary<string, string>
                  {
                      { "departmentCode", departmentCode },
                      { "customerNumber", customerNumber.ToString() },
                      { "customerName", customerName },
                      { "paymentReason", paymentReason.ToUpper() },
                      { "cashierName", cashierName }
                  },
                  cost: cost,
                  totalCost: cost,
                  quantity: 1,
                  email: email,
                  ndsPercent: Config.EnergyNdsPercent)
        {
        }
    }
}
