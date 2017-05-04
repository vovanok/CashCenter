using System.Collections.Generic;

namespace CashCenter.Check
{
	public class Check
	{
        public List<string> CommonLines { get; private set; }

        public decimal Cost { get; private set; }

        public Check(List<string> commonLines, decimal cost)
        {
            CommonLines = commonLines ?? new List<string>();
            Cost = cost;
        }
    }
}
