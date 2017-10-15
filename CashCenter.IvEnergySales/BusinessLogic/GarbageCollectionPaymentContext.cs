using System;
using System.Collections.Generic;
using CashCenter.Dal;
using CashCenter.Common;
using CashCenter.IvEnergySales.Common;
using CashCenter.IvEnergySales.Exceptions;
using CashCenter.IvEnergySales.Check;
using CashCenter.Check;

namespace CashCenter.IvEnergySales.BusinessLogic
{
    public class GarbageCollectionPaymentContext
    {
        private const int ORGANIZATION_CODE = 1600;

        public Observed<int> CustomerNumber { get; } = new Observed<int>();
        public Observed<int> RegionCode { get; } = new Observed<int>();
        public Observed<int> FinancialPeriodCode { get; } = new Observed<int>();
        public Observed<int> OrganizationCode { get; } = new Observed<int>();
        public Observed<int> FilialCode { get; } = new Observed<int>();
        public Observed<decimal> Cost { get; } = new Observed<decimal>();

        public decimal GetCostWithComission(decimal costWithoutComission)
        {
            return costWithoutComission + costWithoutComission * (decimal)(Settings.GarbageCollectionCommissionPercent / 100f);
        }

        public void ApplyBarcode(string barcode)
        {
            Log.Info(string.Format("Garbage collection payments. Apply barcode: {0}", barcode));

            if (string.IsNullOrEmpty(barcode) || barcode.Length != 30)
            {
                string errorMessage = "Штрих код не задан или имееет неверный формат";
                Log.Error(string.Format("{0} ({1}; is null = {2})", errorMessage, barcode, barcode == null));
                Message.Error(errorMessage);
                return;
            }

            var errors = new List<string>();

            string financialPeriodStr = barcode.Substring(0, 3);
            if (!int.TryParse(barcode.Substring(0, 3), out int financialPeriod))
                errors.Add($"Код финансового периода не корректен ({financialPeriodStr})");

            string regionCodeStr = barcode.Substring(5, 2);
            if (!int.TryParse(regionCodeStr, out int regionCode))
                errors.Add($"Код региона не корректен ({regionCodeStr})");

            string customerNumberStr = barcode.Substring(7, 10);
            if (!int.TryParse(customerNumberStr, out int customerNumber))
                errors.Add($"Лицевой счет не корректен ({customerNumberStr})");

            string costStr = barcode.Substring(17, 9);
            if (!int.TryParse(costStr, out int cost))
                errors.Add($"Сумма не корректна ({costStr})");

            string organizationCodeStr = barcode.Substring(26, 4);
            if (!int.TryParse(organizationCodeStr, out int organizationCode))
                errors.Add($"Код организации не корректен ({organizationCodeStr})");
            else if (organizationCode != ORGANIZATION_CODE)
                errors.Add($"Код организации для данного вида платежа должен быть равен {ORGANIZATION_CODE}, а не {organizationCode})");

            if (errors.Count > 0)
                throw new IncorrectDataException(errors);

            FinancialPeriodCode.Value = financialPeriod;
            RegionCode.Value = regionCode;
            CustomerNumber.Value = customerNumber;
            Cost.Value = (decimal)cost / 100; // Копейки -> рубли
            OrganizationCode.Value = organizationCode; ;
            FilialCode.Value = Settings.GarbageCollectionFilialCode;
        }

        public void Pay(decimal cost, bool isWithoutCheck)
        {
            var errors = new List<string>();

            if (CustomerNumber.Value <= 0)
                errors.Add("Номер лицевого счета должен быть положителен");

            if (FinancialPeriodCode.Value <= 0)
                errors.Add("Код финансового периода должен быть положителен");

            if (OrganizationCode.Value <= 0)
                errors.Add("Код организации должен быть положителен");

            if (cost <= 0)
                errors.Add("Сумма должна быть положительна");

            if (errors.Count > 0)
                throw new IncorrectDataException(errors);

            var createDate = DateTime.Now;

            var operationInfo = $"Платеж за вывоз ТКО:\n" +
                $"\tfinancialPeriodCode = {FinancialPeriodCode.Value},\n" +
                $"\tcreateDate = {createDate},\n" +
                $"\torganizationCode = {OrganizationCode.Value},\n" +
                $"\tfilialCode = {FilialCode.Value},\n" +
                $"\tcustomerNumber = {CustomerNumber.Value},\n" +
                $"\tcost = {Cost.Value}";
            Log.Info($"Старт -> {operationInfo}");

            if (isWithoutCheck || TryPrintChecks(CustomerNumber.Value, cost))
            {
                var payment = new GarbageCollectionPayment
                {
                    FinancialPeriodCode = FinancialPeriodCode.Value,
                    CreateDate = createDate,
                    OrganizationCode = OrganizationCode.Value,
                    FilialCode = FilialCode.Value,
                    CustomerNumber = CustomerNumber.Value,
                    Cost = cost,
                    CommissionPercent = Settings.GarbageCollectionCommissionPercent
                };

                DalController.Instance.AddGarbageCollectionPayment(payment);

                Log.Info($"Успешно завершено -> {operationInfo}");
            }
            else
            {
                Log.Info($"Не произведено -> {operationInfo}");
                throw new Exception("Платеж не произведен");
            }
        }

        private bool TryPrintChecks(int customerNumber, decimal cost)
        {
            try
            {
                using (var waiter = new OperationWaiter())
                {
                    var check = new GarbageCollectionCheck(customerNumber, Settings.CasherName, cost);
                    CheckPrinter.Print(check);
                    return true;
                }
            }
            catch (Exception ex)
            {
                var errorMessage = "Ошибка печати чека";
                Log.Error(errorMessage, ex);
                Message.Error(errorMessage);
                return false;
            }
        }

        public void Clear()
        {
            CustomerNumber.Value = 0;
            RegionCode.Value = 0;
            FinancialPeriodCode.Value = 0;
            OrganizationCode.Value = 0;
            FilialCode.Value = 0;
            Cost.Value = 0;
        }
    }
}
