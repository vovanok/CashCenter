using System.Collections.Generic;

namespace CashCenter.IvEnergySales.Check
{
    public class GarbageCollectionCheck : CashCenter.Check.Check
    {
        public GarbageCollectionCheck(int customerNumber, string cashierName, decimal cost)
            : base(
                 descriptorId: "GarbageCollection",
                  parameters: new Dictionary<string, string>
                  {
                      { "customerNumber", customerNumber.ToString() },
                      { "cashierName", cashierName }
                  },
                  cost: cost,
                  totalCost: cost,
                  quantity: 1,
                  email: string.Empty,
                  paySection: 3,
                  ndsPercent: CashCenter.Common.NdsPercent.NdsNone)
        {
        }
    }
}
