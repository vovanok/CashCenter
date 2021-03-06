﻿using System;
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
                { NdsPercent.NdsNone, 0 },
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
                Log.Info(">>> НАЧАЛО ПЕЧАТИ ЧЕКА");

                cashMachine.OpenSessionIfNot();
                cashMachine.Connect();

                try
                {
                    cashMachine.ClearPrintBuffer();
                }
                catch (Exception ex)
                {
                    Log.Error($"Очистка буфера не произведена (размер буфера: {cashMachine.BufferLineNumber})", ex);
                }

                cashMachine.OpenCheck();

                try
                {
                    if (!StringUtils.IsValidEmail(check.Email))
                        throw new Exception($"Передан не валидный email {check.Email}");

                    cashMachine.SendEmail(check.Email);
                }
                catch (Exception ex)
                {
                    Log.Error($"Чек по email {check.Email} не отправлен", ex);
                }

                foreach (var commonLine in check.GetCommonLines())
                {
                    cashMachine.PrintLine(commonLine);
                }

                cashMachine.PrintLine(" ");

                cashMachine.Driver.CheckType = 0;
                cashMachine.Driver.Quantity = check.Quantity;
                cashMachine.Driver.Price = check.Cost;

                var payDepartment =
                    (1 <= check.Descriptor.PaySection && check.Descriptor.PaySection <= 16)
                        ? check.Descriptor.PaySection
                        : 1;

                cashMachine.Driver.Department = payDepartment;
                Log.Info($"Установлена секция чека {check.Descriptor.PaySection}");

                cashMachine.Driver.Tax1 = ndsTypeToTax1Map.ContainsKey(check.NdsPercent) ? ndsTypeToTax1Map[check.NdsPercent] : 0;
                cashMachine.Driver.Tax2 = 0;
                cashMachine.Driver.Tax3 = 0;
                cashMachine.Driver.Tax4 = 0;

                cashMachine.Sale();

                cashMachine.Driver.Summ1 = check.TotalCost;
                cashMachine.CloseCheck();

                cashMachine.Disconnect();

                Log.Info(">>> ПЕЧАТЬ ЧЕКА ЗАВЕРШЕНА");
            }
            catch (Exception ex)
            {
                Log.Error($"Ошибка печати чека (буфер строк: {cashMachine.BufferLineNumber})", ex);
                throw ex;
            }
        }
    }
}
