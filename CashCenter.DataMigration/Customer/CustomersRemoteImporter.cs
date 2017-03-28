using System;
using System.Collections.Generic;
using System.Linq;
using CashCenter.Dal;
using CashCenter.ZeusDb.Entities;

namespace CashCenter.DataMigration
{
    public class CustomersRemoteImporter : BaseRemoteImporter<ZeusCustomer, Customer>
    {
        private DateTime beginDate;
        private DateTime endDate;
        private int dayEncoding;

        public CustomersRemoteImporter()
        {
            var now = DateTime.Now;
            beginDate = new DateTime(now.Year, now.Month, 1);
            endDate = beginDate.AddMonths(1).AddDays(-1);
            dayEncoding = now.Year * 12 + now.Month;
        }

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

            var counterValues = db.GetCustomerCounterValues(zeusCustomer.Id, beginDate, endDate);
            var debt = db.GetDebt(zeusCustomer.Id, dayEncoding);

            return new Customer
            {
                DepartmentId = sourceDepartment.Id,
                Number = zeusCustomer.Id,
                Name = zeusCustomer.Name,
                Address = zeusCustomer.Address,
                DayValue = counterValues.EndDayValue,
                NightValue = counterValues.EndNightValue,
                Balance = debt.Balance,
                Penalty = debt.Penalty,
                IsActive = true,
                Email = string.Empty
            };
        }

        protected override bool TryUpdateExistingItem(ZeusCustomer zeusCustomer)
        {
            var existingCustomer = DalController.Instance.Customers.FirstOrDefault(customer =>
                customer.Department.Id == sourceDepartment.Id &&
                customer.Number == zeusCustomer.Id);

            if (existingCustomer == null)
                return false;

            var counterValues = db.GetCustomerCounterValues(zeusCustomer.Id, beginDate, endDate);
            var debt = db.GetDebt(zeusCustomer.Id, dayEncoding);

            existingCustomer.Name = zeusCustomer.Name;
            existingCustomer.Address = zeusCustomer.Address;
            existingCustomer.DayValue = counterValues.EndDayValue;
            existingCustomer.NightValue = counterValues.EndNightValue;
            existingCustomer.Balance = debt.Balance;
            existingCustomer.Penalty = debt.Penalty;
            existingCustomer.IsActive = true;

            return true;
        }
    }
}
