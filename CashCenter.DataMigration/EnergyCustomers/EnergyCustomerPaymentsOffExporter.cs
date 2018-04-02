using System;
using System.Linq;
using System.Collections.Generic;
using CashCenter.Dal;
using CashCenter.DataMigration.Providers.Off;
using CashCenter.DataMigration.Providers.Off.Entities;

namespace CashCenter.DataMigration.EnergyCustomers
{
    public class EnergyCustomerPaymentsOffExporter : BaseExporter<EnergyCustomerPayment>
    {
        private OffRegistryController outputController = new OffRegistryController();

        protected override List<EnergyCustomerPayment> GetSourceItems(DateTime beginDatetime, DateTime endDatetime)
        {
            return RepositoriesFactory.Get<EnergyCustomerPayment>().Filter(customerPayment =>
                beginDatetime <= customerPayment.CreateDate && customerPayment.CreateDate <= endDatetime).ToList();
        }
        
        protected override ExportResult TryExportItems(IEnumerable<EnergyCustomerPayment> customerPayments)
        {
            if (customerPayments == null)
                return new ExportResult();

            var paymentForStore = customerPayments
                .Where(customerPayment => customerPayment != null)
                .Select(customerPayment => 
                    new OffCustomerPayment(
                        Guid.NewGuid().ToString(),
                        customerPayment.EnergyCustomer.Number,
                        customerPayment.NewDayValue,
                        customerPayment.NewNightValue,
                        customerPayment.ReasonId,
                        customerPayment.Cost,
                        customerPayment.CreateDate,
                        customerPayment.EnergyCustomer.Department.Code));

            outputController.StorePayments(paymentForStore);
            int paymentsCount = paymentForStore.Count();
            return new ExportResult(paymentsCount, customerPayments.Count() - paymentsCount);

        }
    }
}
