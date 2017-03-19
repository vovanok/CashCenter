using CashCenter.Common;
using CashCenter.Dal;
using CashCenter.OffRegistry;
using System;
using System.Linq;

namespace CashCenter.Articles.DataMigration
{
    public class CustomerPaymentsOffExporter : BaseExporter
    {
        private OffRegistryController outputController = new OffRegistryController();

        public override void Export(DateTime beginDatetime, DateTime endDatetime)
        {
            var customerPaymentsForExport = DalController.Instance.CustomerPayments.Where(customerPayment =>
                beginDatetime <= customerPayment.CreateDate && customerPayment.CreateDate <= endDatetime);

            var countSuccess = 0;

            foreach (var customerPayment in customerPaymentsForExport)
            {
                if (ExportCustomerPayment(customerPayment))
                    countSuccess++;
            }

            Log.Info($"Экспортировано {countSuccess} из {customerPaymentsForExport.Count()}.\nOFF файлы находятся в директории {Config.OutputDirectory}.");
        }

        private bool ExportCustomerPayment(CustomerPayment customerPayment)
        {
            if (customerPayment == null)
                return false;

            try
            {
                var offFileCustomerPayment = new OffRegistry.Entities.CustomerPayment(
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
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
