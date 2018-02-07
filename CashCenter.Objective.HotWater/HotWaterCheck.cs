using System.Collections.Generic;
using CashCenter.Common;

namespace CashCenter.Objective.HotWater
{
    public class HotWaterCheck : CashCenter.Check.Check
    {
        public HotWaterCheck(int customerNumber, string customerName,
            string cashierName, decimal total, decimal cost, string email)
            : base(
                  descriptorId: "HotWater",
                  parameters: new Dictionary<string, string>
                  {
                      { "customerNumber", customerNumber.ToString() },
                      { "customerName", customerName },
                      { "cashierName", cashierName },
                      { "total", total.ToString("0.00") }
                  },
                  cost: cost,
                  totalCost: cost,
                  quantity: 1,
                  email: email,
                  ndsPercent: Config.HotWaterNdsPercent)
        {
        }
    }
}
