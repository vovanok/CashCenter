using CashCenter.Check;
using CashCenter.Common;
using CashCenter.Dal;
using CashCenter.IvEnergySales.Check;
using CashCenter.IvEnergySales.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

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
            Customer.Value = DalController.Instance.WaterCustomers.FirstOrDefault(waterCustomer => waterCustomer.Number == number);
        }

        public void ClearCustomer()
        {
            Customer.Value = null;
        }

        public void Pay(string email, double counter1Value, double counter2Value, double counter3Value,
            double counter4Value, decimal penalty, decimal cost, string description, int fiscalNumber, bool isWithoutCheck)
        {
            if (Customer == null)
                throw new CustomerNotAppliedException();

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
                Customer.Value.Email = email;

            DalController.Instance.Save();

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
                FiscalNumber = fiscalNumber
            };
            
            if (isWithoutCheck || TryPrintChecks(payment.Cost, Customer.Value.Number, Customer.Value.Name, Customer.Value.Email))
            {
                DalController.Instance.AddWaterCustomerPayment(payment);
            }
            else
            {
                throw new Exception("Платеж не произведен.");
            }
        }

        private bool TryPrintChecks(decimal cost, int customerNumber,
            string customerName, string customerEmail)
        {
            try
            {
                using (var waiter = new OperationWaiter())
                {
                    var check = new WaterCustomerCheck(customerNumber, customerName,
                        Properties.Settings.Default.CasherName, cost, customerEmail);

                    CheckPrinter.Print(check);
                    return true;
                }
            }
            catch (Exception ex)
            {
                var errorMessage = "Ошибка печати чека";
                Logger.Error(errorMessage, ex);
                Message.Error(errorMessage);
                return false;
            }
        }
    }
}
