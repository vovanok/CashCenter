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
        public void Pay(int financialPeriodCode, DateTime createDate, int organizationCode,
            int filialCode, int customerNumber, decimal cost, bool isWithoutCheck)
        {
            var errors = new List<string>();

            if (customerNumber <= 0)
                errors.Add("Номер лицевого счета должен быть положителен");

            if (financialPeriodCode <= 0)
                errors.Add("Код финансового периода должен быть положителен");

            if (organizationCode <= 0)
                errors.Add("Код организации должен быть положителен");

            if (filialCode <= 0)
                errors.Add("Код филиала должен быть положителен");

            if (cost <= 0)
                errors.Add("Сумма должна быть положительна");

            if (errors.Count > 0)
                throw new IncorrectDataException(errors);

            var operationName = $"Платеж за вывоз ТКО: financialPeriodCode = {financialPeriodCode}, createDate = {createDate}, organizationCode = {organizationCode}, filialCode = {filialCode}, customerNumber = {customerNumber}, cost = {cost}";
            Log.Info($"Старт -> {operationName}");

            if (isWithoutCheck || TryPrintChecks(customerNumber, cost))
            {
                var payment = new GarbageCollectionPayment
                {
                    FinancialPeriodCode = financialPeriodCode,
                    CreateDate = createDate,
                    OrganizationCode = organizationCode,
                    FilialCode = filialCode,
                    CustomerNumber = customerNumber,
                    Cost = cost
                };

                DalController.Instance.AddGarbageCollectionPayment(payment);

                Log.Info($"Успешно завершено -> {operationName}");
            }
            else
            {
                Log.Info($"Не произведено -> {operationName}");
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
    }
}
