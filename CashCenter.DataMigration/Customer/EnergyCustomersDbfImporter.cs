using System.Linq;
using System.Collections.Generic;
using CashCenter.Dal;
using CashCenter.DataMigration.Import;
using CashCenter.DataMigration.Providers.Dbf.Entities;

namespace CashCenter.DataMigration
{
    public class EnergyCustomersDbfImporter : BaseDbfImporter<DbfEnergyCustomer, Customer>
    {
        public Department TargetDepartment { get; set; }

        protected override void CreateNewItems(IEnumerable<Customer> customers)
        {
            DalController.Instance.AddEnergyCustomersRange(customers);
        }

        protected override int DeleteAllTargetItems()
        {
            return 0;
        }

        protected override IEnumerable<DbfEnergyCustomer> GetSourceItems()
        {
            if (dbfRegistry == null)
                return new List<DbfEnergyCustomer>();

            return dbfRegistry.GetEnergyCustomers();
        }

        protected override Customer GetTargetItemBySource(DbfEnergyCustomer dbfCustomer)
        {
            return new Customer
            {
                DepartmentId = TargetDepartment.Id,
                Number = dbfCustomer.Number,
                Name = dbfCustomer.Name,
                Address = dbfCustomer.Address,
                DayValue = dbfCustomer.DayValue,
                NightValue = dbfCustomer.NightValue,
                Balance = dbfCustomer.Balance,
                Penalty = 0,
                IsActive = true,
                Email = string.Empty,
                IsClosed = dbfCustomer.IsClosed
            };
        }

        protected override bool TryUpdateExistingItem(DbfEnergyCustomer dbfCustomer)
        {
            var existingCustomer = DalController.Instance.EnergyCustomers.FirstOrDefault(customer =>
                customer.Department.Id == TargetDepartment.Id && customer.Number == dbfCustomer.Number);

            if (existingCustomer == null)
                return false;

            existingCustomer.DayValue = dbfCustomer.DayValue;
            existingCustomer.NightValue = dbfCustomer.NightValue;
            existingCustomer.Balance = dbfCustomer.Balance;
            existingCustomer.IsActive = true;
            existingCustomer.IsClosed = dbfCustomer.IsClosed;

            return true;
        }
    }
}