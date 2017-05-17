using System;
using System.Collections.Generic;
using CashCenter.Common;

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

        public static void CloseSession()
        {
            cashMachine.CloseSession();
        }

        public static void SysAdminCancelCheck()
        {
            cashMachine.SysAdminCancelCheck();
        }

        public static void Print(Check check)
        {
            try
            {
                Logger.Info(">>> НАЧАЛО ПЕЧАТИ ЧЕКА");

                cashMachine.OpenSessionIfNot();
                cashMachine.Connect();

                cashMachine.OpenCheck();

                try
                {
                    if (!StringUtils.IsValidEmail(check.Email))
                        throw new Exception($"Передан не валидный email {check.Email}");

                    cashMachine.SendEmail(check.Email);
                }
                catch (Exception ex)
                {
                    Logger.Error($"Чек по email {check.Email} не отправлен", ex);
                }

                foreach (var commonLine in check.GetCommonLines())
                {
                    cashMachine.PrintLine(commonLine);
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

                cashMachine.Disconnect();

                Logger.Info(">>> ПЕЧАТЬ ЧЕКА ЗАВЕРШЕНА");
            }
            catch(Exception ex)
            {
                Logger.Error("Ошибка печати чека", ex);
                throw ex;
            }
        }
    }
}
