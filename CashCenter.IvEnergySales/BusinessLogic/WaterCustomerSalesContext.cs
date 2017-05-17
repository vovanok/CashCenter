using CashCenter.Common;
using CashCenter.Dal;
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
            double counter4Value, decimal penalty, decimal cost, string description, int fiscalNumber)
        {
            if (Customer == null)
                throw new CustomerNotAppliedException();

            var incorrectParameters = new List<string>();

            if (counter1Value < 0)
                incorrectParameters.Add("Показание счетчика 1 не должно быть меньше 0");

            if (counter2Value < 0)
                incorrectParameters.Add("Показание счетчика 2 не должно быть меньше 0");

            if (counter3Value < 0)
                incorrectParameters.Add("Показание счетчика 3 не должно быть меньше 0");

            if (counter4Value < 0)
                incorrectParameters.Add("Показание счетчика 4 не должно быть меньше 0");

            var isEmailChange = !string.IsNullOrEmpty(email) && Customer.Value.Email != email;
            if (isEmailChange)
            {
                if (!StringUtils.IsValidEmail(email))
                    incorrectParameters.Add("Адрес электронной почты имеет не верный формат");
            }

            if (incorrectParameters.Count > 0)
                throw new IncorrectDataException(incorrectParameters);

            if (isEmailChange)
                Customer.Value.Email = email;

            DalController.Instance.Save();

            var newPayment = new WaterCustomerPayment
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

            DalController.Instance.AddWaterCustomerPayment(newPayment);
        }
    }
}
