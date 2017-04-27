using CashCenter.Common;
using CashCenter.Dal;
using CashCenter.IvEnergySales.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CashCenter.IvEnergySales.BusinessLogic
{
    public class WaterCustomerPaymentContext
    {
        public Observed<WaterCustomer> Customer { get; } = new Observed<WaterCustomer>();

        public event Action<WaterCustomer> OnCustomerChanged;

        public WaterCustomerPaymentContext()
        {
            Customer.OnChange += OnCustomerChanged;
        }

        public void FindAndApplyCustomer(uint number)
        {
            Customer.Value = DalController.Instance.WaterCustomers.FirstOrDefault(waterCustomer => waterCustomer.Number == number);
        }

        public void ClearCustomer()
        {
            Customer.Value = null;
        }

        public void Pay(string email, decimal counter1Cost, decimal counter2Cost, decimal counter3Cost, string description)
        {
            if (Customer == null)
                throw new CustomerNotAppliedException();

            var incorrectParameters = new List<string>();

            if (counter1Cost < 0)
                incorrectParameters.Add("Сумма по счетчику 1");

            if (counter2Cost < 0)
                incorrectParameters.Add("Сумма по счетчику 2");

            if (counter3Cost < 0)
                incorrectParameters.Add("Сумма по счетчику 3");

            var isEmailChange = !string.IsNullOrEmpty(email) && Customer.Value.Email != email;
            if (isEmailChange)
            {
                if (!StringUtils.IsValidEmail(email))
                    incorrectParameters.Add("Адрес электронной почты");
            }

            DalController.Instance.Save();

            if (incorrectParameters.Count > 0)
                throw new IncorrectDataException(incorrectParameters);

            if (isEmailChange)
                Customer.Value.Email = email;

            var newPayment = new WaterCustomerPayment
            {
                CounterCost1 = counter1Cost,
                CounterCost2 = counter2Cost,
                CounterCost3 = counter3Cost,
                CreateDate = DateTime.Now,
                CustomerId = Customer.Value.Id,
                Description = description ?? string.Empty
            };

            DalController.Instance.AddWaterCustomerPayment(newPayment);
        }
    }
}
