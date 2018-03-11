using System;
using System.Collections.Generic;
using CashCenter.Check;
using CashCenter.Common;
using CashCenter.Common.Exceptions;
using CashCenter.ViewCommon;

namespace CashCenter.IvEnergySales.BusinessLogic
{
    public abstract class GarbageAndRepairPaymentContext
    {
        public abstract int OrganizationCode { get; }
        public abstract string PaymentName { get; }
        public abstract int FilialCode { get; }
        public abstract float CommissionPercent { get; }
        public abstract void StorePaymentToDb(int financialPeriodCode, DateTime createDate, int organizationCode, int filialCode, int customerNumber, decimal cost);
        public abstract CashCenter.Check.Check GetCheck(int customerNumber, decimal costWithoutCommission, decimal commissionValue, decimal cost);

        public Barcode Barcode { get; private set; }
        public decimal Cost => (decimal)(Barcode?.CostInKopeck ?? 0) / 100; // Копейки -> рубли

        public decimal GetCostWithComission(decimal costWithoutCommission)
        {
            return costWithoutCommission + Utils.GetCommission(costWithoutCommission, CommissionPercent);
        }

        public void ApplyBarcode(Barcode barcode)
        {
            Log.Info("Apply barcode");
            if (barcode == null)
            {
                Log.Info("Apply: Barcode is null");
                return;
            }

            this.Barcode = barcode;
        }

        public void Pay(decimal cost, bool isWithoutCheck)
        {
            var errors = new List<string>();

            if (Barcode == null)
            {
                Log.Info("Pay: Barcode is null");
                return;
            }

            if (Barcode.CustomerNumber <= 0)
                errors.Add("Номер лицевого счета должен быть положителен");

            if (Barcode.FinancialPeriod <= 0)
                errors.Add("Код финансового периода должен быть положителен");

            if (cost <= 0)
                errors.Add("Сумма должна быть положительна");

            if (errors.Count > 0)
                throw new IncorrectDataException(errors);

            var createDate = DateTime.Now;

            var operationInfo = $"Платеж за {PaymentName}:\n" +
                $"\tfinancialPeriodCode = {Barcode.FinancialPeriod},\n" +
                $"\tcreateDate = {createDate},\n" +
                $"\torganizationCode = {OrganizationCode},\n" +
                $"\tfilialCode = {FilialCode},\n" +
                $"\tcustomerNumber = {Barcode.CustomerNumber},\n" +
                $"\tcost = {cost}";
            Log.Info($"Старт -> {operationInfo}");

            decimal commissionValue = Utils.GetCommission(cost, CommissionPercent);
            decimal costWithCommission = cost + commissionValue;

            if (isWithoutCheck || TryPrintChecks(Barcode.CustomerNumber, cost, commissionValue, costWithCommission))
            {
                StorePaymentToDb(Barcode.FinancialPeriod, createDate, OrganizationCode, FilialCode, Barcode.CustomerNumber, cost);
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
                    var check = GetCheck(customerNumber, costWithoutCommission, commissionValue, cost);
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
    }
}
