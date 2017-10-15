namespace CashCenter.Common
{
    public static class Utils
    {
        public static decimal GetNdsPart(decimal value, NdsPercent nds)
        {
            int ndsValue = (int)nds;
            return (value / (100 + ndsValue)) * ndsValue;
        }
    }
}
