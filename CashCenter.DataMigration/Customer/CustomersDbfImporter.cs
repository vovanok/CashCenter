﻿using CashCenter.Dal;
using CashCenter.DataMigration.Providers.Dbf.Entities;
using System.Linq;
using System.Collections.Generic;

namespace CashCenter.DataMigration
{
    public class CustomersDbfImporter : BaseDbfImporter<DbfEnergyCustomer, Customer>
    {
        public Department TargetDepartment { get; set; }

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
            var existingCustomer = DalController.Instance.Customers.FirstOrDefault(customer =>
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