using System;

namespace CashCenter.Common
{
    public static class Utils
    {
        public static decimal GetNdsPart(decimal value, NdsPercent nds)
        {
            int ndsValue = (int)nds;
            return (value / (100 + ndsValue)) * ndsValue;
        }

        public static decimal GetCommission(decimal costWithoutComission, float commissionPercent)
        {
            return RoundMoney(costWithoutComission * (decimal)(commissionPercent / 100f));
        }

        public static decimal RubToCopeck(decimal rubValue)
        {
            return rubValue * 100;
        }

        public static object GetDefault(Type type)
        {
            return type.IsValueType
                ? Activator.CreateInstance(type)
                : null;
        }

        public static decimal RoundMoney(decimal value)
        {
            return Math.Round(value, 2);
        }
    }
}
