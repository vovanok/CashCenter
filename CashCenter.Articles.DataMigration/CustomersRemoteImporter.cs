using System;
using System.Linq;
using CashCenter.Common.DbQualification;
using CashCenter.Common;
using CashCenter.Dal;
using CashCenter.ZeusDb;

namespace CashCenter.Articles.DataMigration
{
    public class CustomersRemoteImporter : BaseRemoteImporter
    {
        public override void Import(DepartmentDef departmentDef)
        {
            if (departmentDef == null)
            {
                Log.Error("Для импорта из БД не указано отделение.");
                return;
            }

            try
            {
                DalController.Instance.ClearAllCustomersData();

                var now = DateTime.Now;
                var beginDate = new DateTime(now.Year, now.Month, 1);
                var endDate = beginDate.AddMonths(1).AddDays(-1);

                var db = new ZeusDbController(departmentDef);
                var importingCustomers = db.GetCustomers();

                var department = DalController.Instance.GetDepartmentByCode(departmentDef.Code);
                if (department == null)
                {
                    Log.Error($"Отделение с кодом {departmentDef.Code} не найдено в БД.");
                    return;
                }

                var customersForAdd = importingCustomers.Select(customer =>
                    {
                        var counterValues = db.GetCustomerCounterValues(customer.Id, beginDate, endDate);
                        var debt = db.GetDebt(customer.Id, now.Year * 12 + now.Month);

                        return new Customer
                        {
                            DepartmentId = department.Id,
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
