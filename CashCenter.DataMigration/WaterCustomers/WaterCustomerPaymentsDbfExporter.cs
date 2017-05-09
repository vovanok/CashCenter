using CashCenter.Dal;
using System;
using System.Collections.Generic;

namespace CashCenter.DataMigration.WaterCustomers
{
    public class WaterCustomerPaymentsDbfExporter : BaseExporter<WaterCustomerPayment>
    {
        protected override List<WaterCustomerPayment> GetSourceItems(DateTime beginDatetime, DateTime endDatetime)
        {
            throw new NotImplementedException();
        }

        protected override int TryExportItems(IEnumerable<WaterCustomerPayment> items)
        {
            throw new NotImplementedException();
        }
    }
}
