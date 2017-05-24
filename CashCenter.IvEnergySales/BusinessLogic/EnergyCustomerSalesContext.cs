using CashCenter.Dal;
using CashCenter.Check;
using CashCenter.Common;
using CashCenter.Common.Exceptions;
using CashCenter.IvEnergySales.Check;
using CashCenter.IvEnergySales.Common;
using System;
using System.Linq;
using System.Collections.Generic;

namespace CashCenter.IvEnergySales.BusinessLogic
{
    public class EnergyCustomerSalesContext
    {
        public Observed<Customer> Customer { get; } = new Observed<Customer>();

        public event Action<Customer> OnCustomerChanged
        {
            add { Customer.OnChange += value; }
            remove { Customer.OnChange -= value; }
        }

        public void FindAndApplyCustomer(uint number, Department department)
        {
            if (department == null)
                throw new Exception("Отделение для поиска не задано");

            var operationName = $"Поиск плательщика за электроэнергию {number}, отделение \"{department.Code} {department.Name}\"";
            Log.Info($"Запуск -> {operationName}");

            var potencialCustomers = DalController.Instance.EnergyCustomers.Where(customer => customer.Number == number && customer.IsActive);

            Customer.Value = potencialCustomers.FirstOrDefault(customer => customer.Department.Id == department.Id)
                ?? potencialCustomers.FirstOrDefault(customer => customer.Department.Code == department.Code);

            if (Customer.Value == null)
                Log.Info($"Не найдено -> {operationName}");
            else
                Log.Info($"Найдено -> {operationName}");
        }

        public void ClearCustomer()
        {
            Customer.Value = null;
        }

        public void Pay(string email, int dayValue, int nightValue, decimal cost,
            PaymentReason paymentReason, string description, bool isWithoutCheck)
        {
            if (Customer.Value == null)
                throw new Exception("Отсутствует плательщик");

            var errors = new List<string>();

            if (cost <= 0)
                errors.Add($"Сумма платежа должна быть положительна ({cost})");

            if (!Customer.Value.IsNormative() && dayValue < Customer.Value.DayValue)
                errors.Add($"Новое показание дневного счетчика меньше предыдущего ({dayValue} < {Customer.Value.DayValue})");

            if (!Customer.Value.IsNormative() && Customer.Value.IsTwoTariff() && nightValue < Customer.Value.NightValue)
                errors.Add($"Новое показание ночного счетчика меньше меньше предыдущего ({nightValue} < {Customer.Value.NightValue})");

            if (paymentReason == null)
                errors.Add("Основание для оплаты не задано");

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
                Log.Info($"Изменение email плательщика за электроэнергию с {Customer.Value.Email} на {email}");
                Customer.Value.Email = email;
                DalController.Instance.Save();
            }

            var operationName = $"Платеж за электроэнергию: email={email}, dayValue={dayValue}; nightValue={nightValue}; cost={cost}; paymentReason={paymentReason.Name}; description={description}, isWithoutCheck={isWithoutCheck}";
            Log.Info($"Старт -> {operationName}");

            if (isWithoutCheck || TryPrintChecks(cost, Customer.Value.Department.Code, Customer.Value.Number, Customer.Value.Name, paymentReason.Name, Customer.Value.Email))
            {
                var payment = new CustomerPayment
                {
                    CustomerId = Customer.Value.Id,
                    NewDayValue = dayValue,
                    NewNightValue = nightValue,
                    Cost = cost,
                    ReasonId = paymentReason.Id,
                    CreateDate = DateTime.Now,
                    Description = description,
                    FiscalNumber = 0 // TODO: Fill fiscal
                };

                DalController.Instance.AddEnergyCustomerPayment(payment);

                Log.Info($"Успешно завершено -> {operationName}");
            }
            else
            {
                Log.Info($"Не произведено -> {operationName}");
                throw new Exception("Платеж не произведен");
            }
        }

        private bool TryPrintChecks(decimal cost, string departmentCode, int customerNumber,
            string customerName, string paymentReasonName, string customerEmail)
        {
            try
            {
                using (var waiter = new OperationWaiter())
                {
                    var check = new EnergyCustomerCheck(departmentCode, customerNumber, customerName,
                        paymentReasonName, Properties.Settings.Default.CasherName, cost, customerEmail);

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
