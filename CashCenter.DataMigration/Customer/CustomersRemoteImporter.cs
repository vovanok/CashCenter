using System.Collections.Generic;
using System.Linq;
using CashCenter.Dal;
using CashCenter.ZeusDb.Entities;

namespace CashCenter.DataMigration
{
    public class CustomersRemoteImporter : BaseRemoteImporter<ZeusCustomer, Customer>
    {
        private int dayEncoding;

        private List<Customer> existingCustomers = new List<Customer>();

        public CustomersRemoteImporter()
        {
            existingCustomers = DalController.Instance.Customers.ToList();
        }

        protected override void CreateNewItems(IEnumerable<Customer> customers)
        {
            DalController.Instance.AddCustomersRange(customers);
        }

        protected override int DeleteAllTargetItems()
        {
            foreach (var customer in existingCustomers)
            {
                customer.IsActive = false;
            }

            return existingCustomers.Count;
        }

        protected override IEnumerable<ZeusCustomer> GetSourceItems()
        {
            if (db == null)
                return new List<ZeusCustomer>();

            return db.GetCustomers();
        }

        protected override Customer GetTargetItemBySource(ZeusCustomer zeusCustomer)
        {
            if (db == null)
                return null;

            return new Customer
            {
                DepartmentId = sourceDepartment.Id,
                Number = zeusCustomer.Id,
                Name = zeusCustomer.Name,
                Address = zeusCustomer.Address,
                DayValue = 0,
                NightValue = 0,
                Balance = 0,
                Penalty = 0,
                IsActive = true,
                Email = string.Empty
            };
        }

        protected override bool TryUpdateExistingItem(ZeusCustomer zeusCustomer)
        {
            if (zeusCustomer == null)
                return true;

            var existingCustomer = existingCustomers.FirstOrDefault(customer =>
                customer.Department.Id == sourceDepartment.Id &&
                customer.Number == zeusCustomer.Id);

            if (existingCustomer == null)
                return false;

            existingCustomer.Name = zeusCustomer.Name;
            existingCustomer.Address = zeusCustomer.Address;
            existingCustomer.IsActive = true;

            return true;
        }
    }
}
