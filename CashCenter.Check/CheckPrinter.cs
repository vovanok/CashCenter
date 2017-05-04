using CashCenter.Common;
using CashCenter.IvEnergySales.Check;
using System.Collections.Generic;
using System.Text;

namespace CashCenter.Check
{
    public static class CheckPrinter
    {
        private static CashMachine cashMachine = new CashMachine();

        private static readonly Dictionary<NdsPercent, int> ndsTypeToTax1Map =
            new Dictionary<NdsPercent, int>
            {
                { NdsPercent.Nds10, 2 },
                { NdsPercent.Nds18, 1 },
                { NdsPercent.Nds20, 3 }
            };

        public static bool IsReady { get { return cashMachine.IsReady; } }

        public static void ShowProperties()
        {
            cashMachine.ShowProperties();
        }

        public static void CancelCheck()
        {
            cashMachine.CancelCheck();
        }

        public static void Print(Check check)
        {
            cashMachine.ResetErrors();
            cashMachine.Connect();

            cashMachine.OpenCheck();
            foreach(var lineAfterOpen in check.CommonLines)
            {
                cashMachine.PrintLine(lineAfterOpen);
            }

            cashMachine.Driver.CheckType = 0;
            cashMachine.Driver.Quantity = 1;
            cashMachine.Driver.Price = check.Cost;
            cashMachine.Driver.Department = 1;

            cashMachine.Driver.Tax1 =
                ndsTypeToTax1Map.ContainsKey((NdsPercent)Config.NdsPercent)
                    ? ndsTypeToTax1Map[(NdsPercent)Config.NdsPercent] : 0;
            cashMachine.Driver.Tax2 = 0;
            cashMachine.Driver.Tax3 = 0;
            cashMachine.Driver.Tax4 = 0;

            cashMachine.Sale();

            cashMachine.Driver.Summ1 = check.Cost;
            cashMachine.CloseCheck();

            if (cashMachine.Errors.Count > 0)
            {
                var sbErrorMessage = new StringBuilder();
                foreach (var errorOerationResult in cashMachine.Errors)
                    sbErrorMessage.AppendLine($"{errorOerationResult.Description} ({errorOerationResult.ErrorCode})");

                Log.Error($"Ошибка печати чека.\n{sbErrorMessage.ToString()}");
            }
        }
    }
}
