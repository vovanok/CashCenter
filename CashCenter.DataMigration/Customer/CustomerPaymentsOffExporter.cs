using CashCenter.Dal;
using CashCenter.DataMigration.Providers.Off;
using CashCenter.DataMigration.Providers.Off.Entities;
using System;
using System.Linq;
using System.Collections.Generic;

namespace CashCenter.DataMigration
{
    public class CustomerPaymentsOffExporter : BaseExporter<CustomerPayment>
    {
        private OffRegistryController outputController = new OffRegistryController();

        protected override List<CustomerPayment> GetSourceItems(DateTime beginDatetime, DateTime endDatetime)
        {
            return DalController.Instance.CustomerPayments.Where(customerPayment =>
                beginDatetime <= customerPayment.CreateDate && customerPayment.CreateDate <= endDatetime).ToList();
        }
        
        protected override int TryExportItems(IEnumerable<CustomerPayment> customerPayments)
        {
            if (customerPayments == null)
                return 0;

            var paymentForStore = customerPayments
                .Where(customerPayment => customerPayment != null)
                .Select(customerPayment => 
                    new OffCustomerPayment(
                        Guid.NewGuid().ToString(),
                        customerPayment.Customer.Number,
                        customerPayment.NewDayValue,
                        customerPayment.NewNightValue,
                        customerPayment.ReasonId,
                        customerPayment.Cost,
                        customerPayment.CreateDate,
                        customerPayment.Customer.Department.Code));

            outputController.StorePayments(paymentForStore);
            return paymentForStore.Count();
        }
    }
}
