using System;
using System.Collections.Generic;
using System.Linq;
using CashCenter.Check;
using CashCenter.Common;
using CashCenter.Common.Exceptions;

namespace CashCenter.Objective.HotWater
{
    internal class HotWaterContext
    {
        public Observed<HotWaterCustomer> Customer { get; } = new Observed<HotWaterCustomer>();

        private HotWaterDb db = new HotWaterDb();

        public event Action<HotWaterCustomer> OnCustomerChanged
        {
            add { Customer.OnChange += value; }
            remove { Customer.OnChange -= value; }
        }

        public void FindAndApplyCustomer(uint number)
        {
            var operationName = $"Поиск плательщика за горячую воду {number}";
            Log.Info($"Запуск -> {operationName}");

            Customer.Value = db.HotWaterCustomers
                .FirstOrDefault(customer => customer.IsActive && customer.Number == number);

            if (Customer.Value == null)
                Log.Info($"Не найдено -> {operationName}");
            else
                Log.Info($"Найдено -> {operationName}");
        }

        public void ClearCustomer()
        {
            Customer.Value = null;
        }

        public void Pay(
            string email,
            double counter1Value, double counter2Value, double counter3Value,
            double counter4Value, double counter5Value, decimal total,
            string description, bool isWithoutCheck)
        {
            if (Customer.Value == null)
                throw new Exception("Отсутствует плательщик");

            var errors = new List<string>();

            if (counter1Value < 0)
                errors.Add("Показание счетчика 1 не должно быть меньше 0");

            if (counter2Value < 0)
                errors.Add("Показание счетчика 2 не должно быть меньше 0");

            if (counter3Value < 0)
                errors.Add("Показание счетчика 3 не должно быть меньше 0");

            if (counter4Value < 0)
                errors.Add("Показание счетчика 4 не должно быть меньше 0");

            if (counter5Value < 0)
                errors.Add("Показание счетчика 5 не должно быть меньше 0");

            if (total <= 0)
                errors.Add("Сумма должна быть больше нуля");

            var isEmailChange = !string.IsNullOrEmpty(email) && Customer.Value.Email != email;
            if (isEmailChange)
            {
                if (!StringUtils.IsValidEmail(email))
                    errors.Add("Адрес электронной почты имеет не верный формат");
            }

            if (errors.Count > 0)
                throw new IncorrectDataException(errors);

            if (isEmailChange)
            {
                Log.Info($"Изменение email плательщика за воду с {Customer.Value.Email} на {email}");
                Customer.Value.Email = email;
                db.SaveChanges();
            }

            var operationName = $"Платеж за воду: email={email}, counter1Value={counter1Value}, counter2Value={counter2Value}, counter3Value={counter3Value}, counter4Value={counter4Value}, total={total}, description={description}, isWithoutCheck={isWithoutCheck}";
            Log.Info($"Старт -> {operationName}");

            decimal totalWithCommission = GetCostWithCommission(total);
            decimal commissionValue = totalWithCommission - total;

            if (isWithoutCheck || TryPrintChecks(total, commissionValue, totalWithCommission,
                Customer.Value.Number, Customer.Value.Name, Customer.Value.Email))
            {
                var payment = new HotWaterPayment
                {
                    CreateDate = DateTime.Now,
                    CustomerId = Customer.Value.Id,
                    NewCounterValue1 = counter1Value,
                    NewCounterValue2 = counter2Value,
                    NewCounterValue3 = counter3Value,
                    NewCounterValue4 = counter4Value,
                    NewCounterValue5 = counter5Value,
                    Total = total,
                    CommisionTotal = commissionValue,
                    Description = description ?? string.Empty
                };

                db.HotWaterPayments.Add(payment);
                db.SaveChanges();

                Log.Info($"Успешно завершено -> {operationName}");
            }
            else
            {
                Log.Info($"Не произведено -> {operationName}");
                throw new Exception("Платеж не произведен");
            }
        }

        public decimal GetCostWithCommission(decimal costWithoutCommission)
        {
            return costWithoutCommission +
                Utils.GetCommission(costWithoutCommission, Settings.HotWaterСommissionPercent);
        }

        private bool TryPrintChecks(decimal costWithoutCommision, decimal comissionValue,
            decimal totalCost, int customerNumber, string customerName, string customerEmail)
        {
            try
            {
                using (var waiter = new OperationWaiter())
                {
                    var check = new HotWaterCheck(customerNumber, customerName,
                        Settings.CasherName, costWithoutCommision, comissionValue, totalCost, customerEmail);

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
