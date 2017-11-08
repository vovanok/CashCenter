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

        public static decimal GetCostWithCommission(decimal costWithoutComission, float commissionPercent)
        {
            return costWithoutComission + GetCommission(costWithoutComission, commissionPercent);
        }

        public static decimal GetCommission(decimal costWithoutComission, float commissionPercent)
        {
            return costWithoutComission * (decimal)(commissionPercent / 100f);
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
    }
}
