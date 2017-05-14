using CashCenter.Dal;
using CashCenter.DataMigration.Providers.Dbf.Entities;
using System.Linq;
using System.Collections.Generic;

namespace CashCenter.DataMigration
{
    public class CustomersDbfImporter : BaseDbfImporter<DbfEnergyCustomer, Customer>
    {
        protected override void CreateNewItems(IEnumerable<Customer> customers)
        {
            DalController.Instance.AddCustomersRange(customers);
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
            var department = DalController.Instance.GetDepartmentByCode(dbfCustomer.DepartmentCode);
            if (department == null)
                return null;

            return new Customer
            {
                DepartmentId = department.Id,
                Number = dbfCustomer.Number,
                Name = string.Empty,
                Address = string.Empty,
                DayValue = dbfCustomer.DayValue,
                NightValue = dbfCustomer.NightValue,
                Balance = dbfCustomer.Balance,
                Penalty = 0,
                IsActive = true,
                Email = string.Empty
            };
        }

        protected override bool TryUpdateExistingItem(DbfEnergyCustomer dbfCustomer)
        {
            var existingCustomer = DalController.Instance.Customers.FirstOrDefault(customer =>
                customer.Department.Code == dbfCustomer.DepartmentCode &&
                customer.Number == dbfCustomer.Number);

            if (existingCustomer == null)
                return false;

            existingCustomer.DayValue = dbfCustomer.DayValue;
            existingCustomer.NightValue = dbfCustomer.NightValue;
            existingCustomer.Balance = dbfCustomer.Balance;
            existingCustomer.IsActive = true;

            return true;
        }
    }
}