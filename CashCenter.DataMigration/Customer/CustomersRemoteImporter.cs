using System;
using System.Linq;
using CashCenter.Common;
using CashCenter.Dal;
using CashCenter.ZeusDb;

namespace CashCenter.DataMigration
{
    public class CustomersRemoteImporter : BaseRemoteImporter
    {
        public override void Import(Department department)
        {
            if (department == null)
            {
                Log.Error("Для импорта из БД не указано отделение.");
                return;
            }

            try
            {
                var now = DateTime.Now;
                var beginDate = new DateTime(now.Year, now.Month, 1);
                var endDate = beginDate.AddMonths(1).AddDays(-1);

                var db = new ZeusDbController(department.Code, department.Url, department.Path);
                var importingCustomers = db.GetCustomers();

                var customersForAdd = importingCustomers.Select(customer =>
                    {
                        var counterValues = db.GetCustomerCounterValues(customer.Id, beginDate, endDate);
                        var debt = db.GetDebt(customer.Id, now.Year * 12 + now.Month);

                        return new Customer
                        {
                            DepartmentId = department.Id,
                            Number = customer.Id,
                            Name = customer.Name,
                            Address = customer.Address,
                            DayValue = counterValues.EndDayValue,
                            NightValue = counterValues.EndNightValue,
                            Balance = debt.Balance,
                            Penalty = debt.Penalty
                        };
                    });

                DalController.Instance.AddCustomersRange(customersForAdd);
            }
            catch (Exception ex)
            {
                Log.ErrorWithException("Ошибка импортирования данных из удаленной БД", ex);
            }
        }
    }
}
