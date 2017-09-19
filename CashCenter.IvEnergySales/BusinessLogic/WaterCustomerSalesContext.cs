using System;
using System.Linq;
using System.Collections.Generic;
using CashCenter.Dal;
using CashCenter.Check;
using CashCenter.Common;
using CashCenter.IvEnergySales.Check;
using CashCenter.IvEnergySales.Common;
using CashCenter.IvEnergySales.Exceptions;

namespace CashCenter.IvEnergySales.BusinessLogic
{
    public class WaterCustomerSalesContext
    {
        public Observed<WaterCustomer> Customer { get; } = new Observed<WaterCustomer>();

        public event Action<WaterCustomer> OnCustomerChanged
        {
            add { Customer.OnChange += value; }
            remove { Customer.OnChange -= value; }
        }

        public void FindAndApplyCustomer(uint number)
        {
            var operationName = $"Поиск плательщика за воду {number}";
            Log.Info($"Запуск -> {operationName}");

            Customer.Value = DalController.Instance.WaterCustomers.FirstOrDefault(waterCustomer => waterCustomer.Number == number);

            if (Customer.Value == null)
                Log.Info($"Не найдено -> {operationName}");
            else
                Log.Info($"Найдено -> {operationName}");
        }

        public void ClearCustomer()
        {
            Customer.Value = null;
        }

        public void Pay(string email, double counter1Value, double counter2Value, double counter3Value,
            double counter4Value, decimal penalty, decimal cost, string description, bool isWithoutCheck)
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

            if (cost <= 0)
                errors.Add("Сумма должна быть больше нуля");

            if (penalty < 0)
                errors.Add("Пени не может быть меньше нуля");

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
                DalController.Instance.Save();
            }

            var operationName = $"Платеж за воду: email={email}, counter1Value={counter1Value}, counter2Value={counter2Value}, counter3Value={counter3Value}, counter4Value={counter4Value}, penalty={penalty}, cost={cost}, description={description}, isWithoutCheck={isWithoutCheck}";
            Log.Info($"Старт -> {operationName}");

            decimal costWithoutCommission = cost + penalty;
            decimal costWithComission = GetCostWithComission(costWithoutCommission);
            decimal comissionValue = costWithComission - costWithoutCommission;

            if (isWithoutCheck || TryPrintChecks(costWithoutCommission, comissionValue, costWithComission,
                Customer.Value.Number, Customer.Value.Name, Customer.Value.Email))
            {
                var payment = new WaterCustomerPayment
                {
                    CreateDate = DateTime.Now,
                    CustomerId = Customer.Value.Id,
                    CounterValue1 = counter1Value,
                    CounterValue2 = counter2Value,
                    CounterValue3 = counter3Value,
                    CounterValue4 = counter4Value,
                    Description = description ?? string.Empty,
                    Penalty = penalty,
                    Cost = cost,
                    FiscalNumber = 0, // TODO: Fill fiscal
                    ComissionPercent = Settings.WaterСommissionPercent
                };

                DalController.Instance.AddWaterCustomerPayment(payment);

                Log.Info($"Успешно завершено -> {operationName}");
            }
            else
            {
                Log.Info($"Не произведено -> {operationName}");
                throw new Exception("Платеж не произведен");
            }
        }

        public decimal GetCostWithComission(decimal costWithoutComission)
        {
            return costWithoutComission + costWithoutComission * (decimal)(Settings.WaterСommissionPercent / 100f);
        }

        private bool TryPrintChecks(decimal costWithoutCommision, decimal comissionValue,
            decimal totalCost, int customerNumber, string customerName, string customerEmail)
        {
            try
            {
                using (var waiter = new OperationWaiter())
                {
                    var check = new WaterCustomerCheck(customerNumber, customerName,
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
