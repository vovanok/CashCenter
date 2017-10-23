using System;
using System.Collections.Generic;
using CashCenter.Dal;
using CashCenter.Common;
using CashCenter.IvEnergySales.Common;
using CashCenter.IvEnergySales.Exceptions;
using CashCenter.IvEnergySales.Check;
using CashCenter.Check;
using System.Linq;

// TODO: переписать этот ужас
namespace CashCenter.IvEnergySales.BusinessLogic
{
    public class GarbageAndRepairPaymentContext
    {
        private class PaymentSubcontext
        {
            public int Code { get; private set; }
            public string Name { get; private set; }
            public Action<int, DateTime, int, int, int, decimal> StorePaymentToDb { get; private set; }
            public Func<int> GetFilialCode { get; private set; }
            public Func<float> GetCommissionPercent { get; private set; }

            public PaymentSubcontext(int code, string name, Action<int, DateTime, int, int, int, decimal> storePaymentToDb, Func<int> getFilialCode, Func<float> getCommissionPercent)
            {
                Code = code;
                Name = name;
                StorePaymentToDb = storePaymentToDb;
                GetFilialCode = getFilialCode;
                GetCommissionPercent = getCommissionPercent;
            }
        }

        private readonly List<PaymentSubcontext> PaymentContextInfos = new List<PaymentSubcontext>
        {
            new PaymentSubcontext(1600, "Вывоз ТКО",
                (int financialPeriodCode, DateTime createDate, int organizationCode, int filialCode, int customerNumber, decimal cost) =>
                {
                    var payment = new GarbageCollectionPayment
                    {
                        FinancialPeriodCode = financialPeriodCode,
                        CreateDate = createDate,
                        OrganizationCode = organizationCode,
                        FilialCode = filialCode,
                        CustomerNumber = customerNumber,
                        Cost = cost,
                        CommissionPercent = Settings.GarbageCollectionCommissionPercent
                    };

                    DalController.Instance.AddGarbageCollectionPayment(payment);
                },
                () => Settings.GarbageCollectionFilialCode, 
                () => Settings.GarbageCollectionCommissionPercent),
            new PaymentSubcontext(1500, "Кап. ремонт",
                (int financialPeriodCode, DateTime createDate, int organizationCode, int filialCode, int customerNumber, decimal cost) =>
                {
                    var payment = new RepairPayment
                    {
                        FinancialPeriodCode = financialPeriodCode,
                        CreateDate = createDate,
                        OrganizationCode = organizationCode,
                        FilialCode = filialCode,
                        CustomerNumber = customerNumber,
                        Cost = cost,
                        CommissionPercent = Settings.RepairCommissionPercent
                    };

                    DalController.Instance.AddRepairPayment(payment);
                },
                () => Settings.RepairFilialCode,
                () => Settings.RepairCommissionPercent)
        };

        public Observed<string> PaymentName { get; } = new Observed<string>();
        public Observed<int> CustomerNumber { get; } = new Observed<int>();
        public Observed<int> RegionCode { get; } = new Observed<int>();
        public Observed<int> FinancialPeriodCode { get; } = new Observed<int>();
        public Observed<int> OrganizationCode { get; } = new Observed<int>();
        public Observed<int> FilialCode { get; } = new Observed<int>();
        public Observed<decimal> Cost { get; } = new Observed<decimal>();
        public Observed<float> CommissionPercent { get; } = new Observed<float>();

        private PaymentSubcontext currentPaymentSubcontext;

        public decimal GetCostWithComission(decimal costWithoutCommission)
        {
            if (currentPaymentSubcontext == null)
                return 0;

            return Utils.GetCostWithComission(costWithoutCommission, currentPaymentSubcontext.GetCommissionPercent());
        }

        public void ApplyBarcode(string barcode)
        {
            Log.Info(string.Format("{0}. Apply barcode: {1}", string.Join(", ", PaymentContextInfos.Select(item => item.Name)), barcode));

            if (string.IsNullOrEmpty(barcode) || barcode.Length != 30)
            {
                string errorMessage = "Штрих код не задан или имеет неверный формат";
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
            else
            {
                currentPaymentSubcontext = PaymentContextInfos.FirstOrDefault(item => item.Code == organizationCode);
                if (currentPaymentSubcontext == null)
                {
                    string possibleOrganizationCodeValues = string.Join("; ", PaymentContextInfos.Select(item => $"{item.Code} ({item.Name})"));
                    errors.Add($"Код организации не корректен ({organizationCode}). Возможные значения: {possibleOrganizationCodeValues}");
                }
            }

            if (errors.Count > 0)
                throw new IncorrectDataException(errors);

            PaymentName.Value = currentPaymentSubcontext.Name;
            FinancialPeriodCode.Value = financialPeriod;
            RegionCode.Value = regionCode;
            CustomerNumber.Value = customerNumber;
            Cost.Value = (decimal)cost / 100; // Копейки -> рубли
            OrganizationCode.Value = organizationCode;
            FilialCode.Value = currentPaymentSubcontext.GetFilialCode();
            CommissionPercent.Value = currentPaymentSubcontext.GetCommissionPercent();
        }

        public void Pay(decimal cost, bool isWithoutCheck)
        {
            var errors = new List<string>();

            if (currentPaymentSubcontext == null)
                errors.Add("Некорректно распознан код организации");

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

            var operationInfo = $"Платеж за {currentPaymentSubcontext.Name}:\n" +
                $"\tfinancialPeriodCode = {FinancialPeriodCode.Value},\n" +
                $"\tcreateDate = {createDate},\n" +
                $"\torganizationCode = {OrganizationCode.Value},\n" +
                $"\tfilialCode = {FilialCode.Value},\n" +
                $"\tcustomerNumber = {CustomerNumber.Value},\n" +
                $"\tcost = {Cost.Value}";
            Log.Info($"Старт -> {operationInfo}");

            var costWithCommission = GetCostWithComission(cost);
            decimal comissionValue = costWithCommission - cost;

            if (isWithoutCheck || TryPrintChecks(CustomerNumber.Value, cost, comissionValue, costWithCommission))
            {
                currentPaymentSubcontext.StorePaymentToDb(FinancialPeriodCode.Value, createDate, OrganizationCode.Value, FilialCode.Value, CustomerNumber.Value, cost);
                Log.Info($"Успешно завершено -> {operationInfo}");
            }
            else
            {
                Log.Info($"Не произведено -> {operationInfo}");
                throw new Exception("Платеж не произведен");
            }
        }

        private bool TryPrintChecks(int customerNumber, decimal costWithoutCommission, decimal commissionValue, decimal cost)
        {
            try
            {
                using (var waiter = new OperationWaiter())
                {
                    var check = new GarbageAndRepairCheck(customerNumber, Settings.CasherName,
                        costWithoutCommission, commissionValue, cost);
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
            currentPaymentSubcontext = null;
            PaymentName.Value = string.Empty;
            CustomerNumber.Value = 0;
            RegionCode.Value = 0;
            FinancialPeriodCode.Value = 0;
            OrganizationCode.Value = 0;
            FilialCode.Value = 0;
            Cost.Value = 0;
            CommissionPercent.Value = 0;
        }
    }
}
