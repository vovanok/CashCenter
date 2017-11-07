using System.Collections.Generic;

namespace CashCenter.IvEnergySales.Check
{
    public class RepairCheck : CashCenter.Check.Check
    {
        public RepairCheck(int customerNumber, string cashierName,
            decimal costWithoutComission, decimal comissionValue, decimal cost)
            : base(
                 descriptorId: "Repair",
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
                  ndsPercent: CashCenter.Common.NdsPercent.NdsNone)
        {
        }
    }
}
