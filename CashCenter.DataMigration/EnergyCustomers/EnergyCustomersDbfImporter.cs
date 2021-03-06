﻿using System.Linq;
using System.Collections.Generic;
using CashCenter.Dal;
using CashCenter.DataMigration.Import;
using CashCenter.DataMigration.Providers.Dbf.Entities;

namespace CashCenter.DataMigration.EnergyCustomers
{
    public class EnergyCustomersDbfImporter : BaseDbfImporter<DbfEnergyCustomer, EnergyCustomer>
    {
        public Department TargetDepartment { get; set; }

        protected override void CreateNewItems(IEnumerable<EnergyCustomer> customers)
        {
            RepositoriesFactory.Get<EnergyCustomer>().AddRange(customers);
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

        protected override EnergyCustomer GetTargetItemBySource(DbfEnergyCustomer dbfCustomer)
        {
            return new EnergyCustomer
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
                IsClosed = dbfCustomer.IsClosed,
                PaymentDocumentIdentifier = dbfCustomer.PaymentDocumentIdentifier ?? string.Empty,
                HusIdentifier = dbfCustomer.HusIdentifier ?? string.Empty
            };
        }

        protected override bool TryUpdateExistingItem(DbfEnergyCustomer dbfCustomer)
        {
            var existingCustomer = RepositoriesFactory.Get<EnergyCustomer>().Get(customer =>
                customer.Department.Id == TargetDepartment.Id && customer.Number == dbfCustomer.Number);

            if (existingCustomer == null)
                return false;

            existingCustomer.Name = dbfCustomer.Name;
            existingCustomer.Address = dbfCustomer.Address;
            existingCustomer.DayValue = dbfCustomer.DayValue;
            existingCustomer.NightValue = dbfCustomer.NightValue;
            existingCustomer.Balance = dbfCustomer.Balance;
            existingCustomer.IsActive = true;
            existingCustomer.IsClosed = dbfCustomer.IsClosed;
            existingCustomer.PaymentDocumentIdentifier = dbfCustomer.PaymentDocumentIdentifier ?? string.Empty;
            existingCustomer.HusIdentifier = dbfCustomer.HusIdentifier ?? string.Empty;

            return true;
        }
    }
}