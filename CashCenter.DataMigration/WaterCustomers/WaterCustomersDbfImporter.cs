using System.Linq;
using System.Collections.Generic;
using CashCenter.Dal;
using CashCenter.DataMigration.Import;
using CashCenter.DataMigration.Providers.Dbf.Entities;

namespace CashCenter.DataMigration.WaterCustomers
{
    public class WaterCustomersDbfImporter : BaseDbfImporter<DbfWaterCustomer, WaterCustomer>
    {
        protected override void CreateNewItems(IEnumerable<WaterCustomer> waterCustomers)
        {
            RepositoriesFactory.Get<WaterCustomer>().AddRange(waterCustomers);
        }

        protected override int DeleteAllTargetItems()
        {
            int count = 0;
            foreach(var waterCustomer in RepositoriesFactory.Get<WaterCustomer>().GetAll())
            {
                waterCustomer.IsActive = false;
                count++;
            }

            return count;
        }

        protected override IEnumerable<DbfWaterCustomer> GetSourceItems()
        {
            if (dbfRegistry == null)
                return new List<DbfWaterCustomer>();

            return dbfRegistry.GetWaterCustomers();
        }

        protected override WaterCustomer GetTargetItemBySource(DbfWaterCustomer dbfWaterCustomer)
        {
            return new WaterCustomer
            {
                Number = dbfWaterCustomer.Number,
                Name = dbfWaterCustomer.Name,
                Address = dbfWaterCustomer.Address,
                CounterNumber1 = dbfWaterCustomer.CounterNumber1,
                CounterNumber2 = dbfWaterCustomer.CounterNumber2,
                CounterNumber3 = dbfWaterCustomer.CounterNumber3,
                CounterNumber4 = string.Empty,
                IsActive = true,
                Email = string.Empty
            };
        }

        protected override bool TryUpdateExistingItem(DbfWaterCustomer dbfWaterCustomer)
        {
            var existingWaterCustomer = RepositoriesFactory.Get<WaterCustomer>().Get(waterCustomer =>
                waterCustomer.Number == dbfWaterCustomer.Number);

            if (existingWaterCustomer == null)
                return false;

            existingWaterCustomer.Name = dbfWaterCustomer.Name;
            existingWaterCustomer.Address = dbfWaterCustomer.Address;
            existingWaterCustomer.CounterNumber1 = dbfWaterCustomer.CounterNumber1;
            existingWaterCustomer.CounterNumber2 = dbfWaterCustomer.CounterNumber2;
            existingWaterCustomer.CounterNumber3 = dbfWaterCustomer.CounterNumber3;
            existingWaterCustomer.CounterNumber4 = string.Empty;
            existingWaterCustomer.IsActive = true;

            return true;
        }
    }
}
