using CashCenter.Common;
using CashCenter.Dal;
using CashCenter.DbfRegistry;
using System;
using System.Linq;

namespace CashCenter.Articles.DataMigration
{
    public class CustomersDbfImporter : BaseDbfImporter
    {
        public override void Import(string dbfFilename)
        {
            if (string.IsNullOrEmpty(dbfFilename))
            {
                Log.Error($"{IMPORT_ERROR_PREFIX} Путь к файлу не задан.");
                return;
            }

            try
            {
                var dbfRegistry = new DbfRegistryController(dbfFilename);
                var importedCustomers = dbfRegistry.GetCustomers();

                var existingCustomers = DalController.Instance.Customers;

                foreach (var importedCustomer in importedCustomers)
                {
                    var existingCustomer = existingCustomers.FirstOrDefault(customer =>
                        customer.Id == importedCustomer.Id && customer.Department.Code == importedCustomer.DepartmentCode);
                    if (existingCustomer != null)
                    {
                        UpdateCustomerByImportedCustomer(existingCustomer, importedCustomer);
                    }
                    else
                    {
                        AddNewCustomerByImportedCustomer(importedCustomer);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.ErrorWithException(IMPORT_ERROR_PREFIX, ex);
            }
        }

        private void AddNewCustomerByImportedCustomer(DbfRegistry.Entities.Customer importedCustomer)
        {
            if (importedCustomer == null)
                return;

            var department = DalController.Instance.GetDepartmentByCode(importedCustomer.DepartmentCode);
            if (department == null)
                return;

            var newCustomer = new Customer
            {
                Id = importedCustomer.Id,
                DepartmentId = department.Id,
                Name = string.Empty,
                Address = string.Empty,
                DayValue = importedCustomer.DayValue,
                NightValue = importedCustomer.NightValue,
                Balance = importedCustomer.Balance,
                Penalty = 0
            };

            DalController.Instance.AddCustomer(newCustomer);
        }

        private void UpdateCustomerByImportedCustomer(Customer customer, DbfRegistry.Entities.Customer importedCustomer)
        {
            if (customer == null || importedCustomer == null)
                return;

            customer.DayValue = importedCustomer.DayValue;
            customer.NightValue = importedCustomer.NightValue;
            customer.Balance = importedCustomer.Balance;

            DalController.Instance.Save();
        }
    }
}