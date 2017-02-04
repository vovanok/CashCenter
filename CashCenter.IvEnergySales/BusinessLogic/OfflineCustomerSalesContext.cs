using System;
using CashCenter.Common.DataEntities;
using CashCenter.Common;
using System.IO;
using System.Linq;
using CashCenter.OffRegistry;
using CashCenter.DbfRegistry;
using System.Collections.Generic;

namespace CashCenter.IvEnergySales.BusinessLogic
{
    public class OfflineCustomerSalesContext : BaseCustomerSalesContext
    {
        private DbfRegistryController inputController;
        private OffRegistryController outputController = new OffRegistryController();

        public OfflineCustomerSalesContext(int customerId, string inputDataDbfFileName)
            : base(customerId)
        {
            inputController = new DbfRegistryController(inputDataDbfFileName);
            var dbfCustomer = inputController.GetCustomer(customerId);
            if (dbfCustomer == null)
                return;

            Customer = new Customer(dbfCustomer.Id, dbfCustomer.DepartmentCode, string.Empty, string.Empty,
                dbfCustomer.DayValue, dbfCustomer.NightValue, dbfCustomer.Balance, 0);

            PaymentReasons = new List<PaymentReason> { new PaymentReason(1, "Электроэнергия") };
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

                var offFileCustomerPayment = new OffRegistry.Entities.CustomerPayment(
                    Guid.NewGuid().ToString(),
                    payment.Customer.Id,
                    payment.NewDayValue,
                    payment.NewNightValue,
                    payment.ReasonId,
                    payment.Cost,
                    payment.CreateDate,
                    Customer.DepartmentCode);

                outputController.AddPayment(offFileCustomerPayment);

                var paymentReasonName = PaymentReasons.FirstOrDefault(item => item.Id == payment.ReasonId)?.Name ?? string.Empty;
                InfoForCheck = new InfoForCheck(payment.Cost, payment.CreateDate,
                    Customer.DepartmentCode, Customer.Id, Customer.Name, paymentReasonName);
                return true;
            }
            catch (Exception e)
            {
                Log.ErrorWithException("Ошибка записи off файла", e);
                return false;
            }
        }

        private string GetOffFileName(string departamentCode, DateTime date)
        {
            string fileName = string.Format(Config.CustomerOutputFileFormat, departamentCode, date);
            return Path.Combine(Config.OutputDirectory, fileName);
        }
    }
}
