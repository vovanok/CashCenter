using System;
using System.Collections.Generic;
using System.Linq;
using CashCenter.Dal;
using CashCenter.ZeusDb.Entities;
using CashCenter.Common;

namespace CashCenter.DataMigration
{
    public class CustomersRemoteImporter : BaseRemoteImporter<ZeusCustomer, Customer>
    {
        private DateTime beginDate;
        private DateTime endDate;
        private int dayEncoding;

        private List<Customer> existingCustomers = new List<Customer>();

        public CustomersRemoteImporter()
        {
            var now = DateTime.Now;
            beginDate = new DateTime(now.Year, now.Month, 1);
            endDate = beginDate.AddMonths(1).AddDays(-1);
            dayEncoding = now.Year * 12 + now.Month;

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

            var counterValues = db.GetCustomerCounterValues(zeusCustomer.Id, beginDate, endDate);
            var debt = db.GetDebt(zeusCustomer.Id, dayEncoding);

            return new Customer
            {
                DepartmentId = sourceDepartment.Id,
                Number = zeusCustomer.Id,
                Name = zeusCustomer.Name,
                Address = zeusCustomer.Address,
                DayValue = counterValues != null ? counterValues.EndDayValue : 0,
                NightValue = counterValues != null ? counterValues.EndNightValue : 0,
                Balance = debt != null ? debt.Balance : 0,
                Penalty = debt != null ? debt.Penalty : 0,
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

            var counterValues = db.GetCustomerCounterValues(zeusCustomer.Id, beginDate, endDate);
            var debt = db.GetDebt(zeusCustomer.Id, dayEncoding);

            existingCustomer.Name = zeusCustomer.Name;
            existingCustomer.Address = zeusCustomer.Address;
            existingCustomer.DayValue = counterValues != null ? counterValues.EndDayValue : 0;
            existingCustomer.NightValue = counterValues != null ? counterValues.EndNightValue : 0;
            existingCustomer.Balance = debt != null ? debt.Balance : 0;
            existingCustomer.Penalty = debt != null ? debt.Penalty : 0;
            existingCustomer.IsActive = true;

            return true;
        }
    }
}
