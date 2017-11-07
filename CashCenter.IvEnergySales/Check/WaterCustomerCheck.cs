using System.Collections.Generic;
using CashCenter.Common;

namespace CashCenter.IvEnergySales.Check
{
    public class WaterCustomerCheck : CashCenter.Check.Check
    {
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
                  ndsPercent: Config.WaterNdsPercent)
        {
        }
    }
}
