using CashCenter.Common;
using CashCenter.Dal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CashCenter.IvEnergySales.BusinessLogic
{
    public class CustomerSalesContext
    {
        protected const string PAY_ERROR_PREFIX = "Ошибка совершения платежа.";

        public Customer Customer { get; protected set; }
        public InfoForCheck InfoForCheck { get; protected set; }
        public List<PaymentReason> PaymentReasons { get; protected set; }

        public bool IsCustomerFinded => Customer != null;

        public CustomerSalesContext(Department department, int customerNumber)
        {
            if (department == null)
                return;

            try
            {
                var customers = DalController.Instance.Customers.Where(customer => customer.Number == customerNumber);

                Customer = customers.FirstOrDefault(customer => customer.Department.Id == department.Id)
                    ?? customers.FirstOrDefault(customer => customer.Department.Code == department.Code);

                if (Customer == null)
                    return;

                PaymentReasons = DalController.Instance.PaymentReasons.ToList();
            }
            catch(Exception ex)
            {
                Log.ErrorWithException("Ошибка поиска физ. лица", ex);
            }
        }

        public void ChangeEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return;

            if (!IsCustomerFinded)
                return;

            if (Customer.Email == email)
                return;

            if (!StringUtils.IsValidEmail(email))
            {
                Log.Error("Адрес электронной почты не задан или имеет неверный формат.");
                return;
            }

            Customer.Email = email;
            DalController.Instance.Save();
        }

        public bool Pay(CustomerPayment payment)
        {
            try
            {
                if (!IsCustomerFinded)
                {
                    Log.Error($"{PAY_ERROR_PREFIX}\nОтсутствует плательщик.");
                    return false;
                }

                if (payment == null)
                {
                    Log.Error($"{PAY_ERROR_PREFIX}\nОтсутствует платеж.");
                    return false;
                }

                if (!payment.IsValid(Customer, out string validationErrorMessage))
                {
                    Log.Error($"{PAY_ERROR_PREFIX}\n{validationErrorMessage}");
                    return false;
                }

                DalController.Instance.AddCustomerPayment(payment);

                var paymentReasonName = PaymentReasons.FirstOrDefault(item => item.Id == payment.ReasonId)?.Name ?? string.Empty;

                InfoForCheck = new InfoForCheck(payment.Cost, payment.CreateDate,
                    Customer.Department.Code, Customer.Id, Customer.Name, paymentReasonName);

                return true;
            }
            catch(Exception e)
            {
                Log.ErrorWithException("Ошибка создание платежа физ.лица.", e);
                return false;
            }
        }
    }
}
