using CashCenter.Dal;
using CashCenter.OffRegistry;
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
        
        protected override int TryExportItems(IEnumerable<CustomerPayment> items)
        {
            var countSuccess = 0;
            foreach(var item in items)
            {
                if (TryExportOneItem(item))
                    countSuccess++;
            }

            return countSuccess;
        }

        private bool TryExportOneItem(CustomerPayment customerPayment)
        {
            if (customerPayment == null)
                return false;

            try
            {
                var offFileCustomerPayment = new OffRegistry.Entities.OffCustomerPayment(
                    Guid.NewGuid().ToString(),
                    customerPayment.Customer.Id,
                    customerPayment.NewDayValue,
                    customerPayment.NewNightValue,
                    customerPayment.ReasonId,
                    customerPayment.Cost,
                    customerPayment.CreateDate,
                    customerPayment.Customer.Department.Code);

                outputController.AddPayment(offFileCustomerPayment);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
