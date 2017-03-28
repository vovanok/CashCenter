using CashCenter.Common;
using CashCenter.Dal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CashCenter.IvEnergySales.BusinessLogic
{
    public class CustomerSalesContext
    {
        protected const string ERROR_PREFIX = "Ошибка совершения платежа.";

        public Customer Customer { get; protected set; }
        public InfoForCheck InfoForCheck { get; protected set; }
        public IEnumerable<PaymentReason> PaymentReasons { get; protected set; }

        public bool IsCustomerFinded => Customer != null;

        public CustomerSalesContext(int customerId)
        {
            var dalCustomer = DalController.Instance.GetCustomerById(customerId);
            if (dalCustomer == null)
                return;

            PaymentReasons = DalController.Instance.PaymentReasons;
        }

        public void ChangeEmail(string email)
        {
            if (Customer == null)
            {
                Log.Error("Плательщик не задан.");
                return;
            }

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
                    Log.Error($"{ERROR_PREFIX}\nОтсутствует плательщик.");
                    return false;
                }

                if (payment == null)
                {
                    Log.Error($"{ERROR_PREFIX}\nОтсутствует платеж.");
                    return false;
                }

                if (!payment.IsValid(out string validationErrorMessage))
                {
                    Log.Error($"{ERROR_PREFIX}\n{validationErrorMessage}");
                    return false;
                }

                var newDalCustomerPayment = new Dal.CustomerPayment()
                {
                    CustomerId = payment.Customer.Id,
                    NewDayValue = payment.NewDayValue,
                    NewNightValue = payment.NewNightValue,
                    Cost = payment.Cost,
                    ReasonId = payment.ReasonId,
                    CreateDate = payment.CreateDate,
                    Description = payment.Description
                };

                DalController.Instance.AddCustomerPayment(newDalCustomerPayment);

                var paymentReasonName = PaymentReasons.FirstOrDefault(item => item.Id == payment.ReasonId)?.Name ?? string.Empty;
                InfoForCheck = new InfoForCheck(payment.Cost, payment.CreateDate,
                    Customer.Department.Code, Customer.Id, Customer.Name, paymentReasonName);

                return true;
            }
            catch(Exception e)
            {
                Log.ErrorWithException("Ошибка добавления платежа в БД", e);
                return false;
            }
        }
    }
}
