using CashCenter.Common;
using CashCenter.Common.DataEntities;
using System;
using System.Linq;

namespace CashCenter.IvEnergySales.BusinessLogic
{
    public class CustomerSalesContext : BaseCustomerSalesContext
    {
        public CustomerSalesContext(int customerId)
            : base(customerId)
        {
            var dalCustomer = Dal.DalController.Instance.GetCustomerById(customerId);
            if (dalCustomer == null)
                return;

            Customer = new Customer(
                id: dalCustomer.Id,
                departmentCode: dalCustomer.Department.Code,
                name: dalCustomer.Name,
                address: dalCustomer.Address,
                dayValue: dalCustomer.DayValue,
                nightValue: dalCustomer.NightValue,
                balance: dalCustomer.Balance,
                penalty: dalCustomer.Penalty);

            PaymentReasons = Dal.DalController.Instance.PaymentReasons
                .Select(dalReason => new PaymentReason(dalReason.Id, dalReason.Name))
                .ToList();
        }

        public override bool Pay(CustomerPayment payment)
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

                if (!payment.IsValid)
                {
                    Log.Error($"{ERROR_PREFIX}\n{payment.ValidationErrorMessage}");
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

                Dal.DalController.Instance.AddCustomerPayment(newDalCustomerPayment);

                var paymentReasonName = PaymentReasons.FirstOrDefault(item => item.Id == payment.ReasonId)?.Name ?? string.Empty;
                InfoForCheck = new InfoForCheck(payment.Cost, payment.CreateDate,
                    Customer.DepartmentCode, Customer.Id, Customer.Name, paymentReasonName);

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
