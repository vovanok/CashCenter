using CashCenter.Dal;
using CashCenter.DbfRegistry.Entities;
using System.Linq;
using System.Collections.Generic;

namespace CashCenter.DataMigration
{
    public class CustomersDbfImporter : BaseDbfImporter<DbfCustomer, Customer>
    {
        protected override void CreateNewItems(IEnumerable<Customer> customers)
        {
            DalController.Instance.AddCustomersRange(customers);
        }

        protected override void DeleteAllTargetItems()
        {
            foreach (var customer in DalController.Instance.Customers)
            {
                customer.IsActive = false;
            }
        }

        protected override IEnumerable<DbfCustomer> GetSourceItems()
        {
            if (dbfRegistry == null)
                return new List<DbfCustomer>();

            return dbfRegistry.GetCustomers();
        }

        protected override Customer GetTargetItemBySource(DbfCustomer dbfCustomer)
        {
            var department = DalController.Instance.GetDepartmentByCode(dbfCustomer.DepartmentCode);
            if (department == null)
                return null;

            return new Customer
            {
                DepartmentId = department.Id,
                Number = dbfCustomer.Id,
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

        protected override bool TryUpdateExistingItem(DbfCustomer dbfCustomer)
        {
            var existingCustomer = DalController.Instance.Customers.FirstOrDefault(customer =>
                customer.Department.Code == dbfCustomer.DepartmentCode &&
                customer.Number == dbfCustomer.Id);

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