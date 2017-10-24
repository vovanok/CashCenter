using System.Collections.Generic;

namespace CashCenter.IvEnergySales.Check
{
    public class GarbageAndRepairCheck : CashCenter.Check.Check
    {
        public GarbageAndRepairCheck(int customerNumber, string cashierName,
            decimal costWithoutComission, decimal comissionValue, decimal cost, int paySection)
            : base(
                 descriptorId: "GarbageAndRepair",
                  parameters: new Dictionary<string, string>
                  {
                      { "customerNumber", customerNumber.ToString() },
                      { "cashierName", cashierName },
                      { "costWithoutComission", costWithoutComission.ToString("0.00") },
                      { "comissionValue", comissionValue.ToString("0.00") }
                  },
                  cost: cost,
                  totalCost: cost,
                  quantity: 1,
                  email: string.Empty,
                  paySection: paySection,
                  ndsPercent: CashCenter.Common.NdsPercent.NdsNone)
        {
        }
    }
}
