using System.Collections.Generic;
using System.Linq;
using CashCenter.IvEnergySales.DAL;
using CashCenter.IvEnergySales.DbQualification;
using CashCenter.IvEnergySales.DataModel;
using CashCenter.IvEnergySales.Logging;
using System;

namespace CashCenter.IvEnergySales.BusinessLogic
{
    public class EnergySalesManager
    {
        private const string PAY_JOURNAL_NAME = "Касса РКЦ Ивановской области";

        private DepartmentModel department;
        private List<DbController> dbControllers = new List<DbController>();

        public DateTime LastCreateDate { get; private set; }
        public decimal LastCost { get; private set; }

        public EnergySalesManager(DepartmentModel department)
        {
            this.department = department;
        }

        public void SetDbCode(string dbCode)
        {
            dbControllers = GetDbControllersByDbCode(dbCode);
        }

        public List<PaymentReason> GetPaymentReasons()
        {
            foreach (var dbController in dbControllers)
            {
                var paymentReasons = dbController.GetPaymentReasons();
                if (paymentReasons != null && paymentReasons.Count > 0)
                    return paymentReasons;
            }

            return new List<PaymentReason>();
        }

        public Customer GetCustomer(int customerId)
        {
            foreach(var dbController in dbControllers)
            {
                var customer = dbController.GetCustomer(customerId);
                if (customer != null)
                    return customer;
            }

            return null;
        }

        public void Pay(Customer customer, int reasonId, decimal cost, string description, int value1, int value2)
        {
            var errorPrefix = "Ошибка совершения платежа.";

            if (customer == null || customer.Db == null)
            {
                Log.Error($"{errorPrefix} Плательщик некорректен.");
                return;
            }

            if (reasonId < 0)
            {
                Log.Error($"{errorPrefix} Основание платежа некорректно.");
                return;
            }

            if (value1 < 0 || value2 < 0)
            {
                Log.Error($"{errorPrefix} Показания счетчиков не корректны. Они должны быть не отрицательными числами.");
                return;
            }

            //      const string PAYMENT_KIND_NAME = "CashCenter_ParmentKind";
            //      var paymentKind = customer.Db.GetPaymentKind(PAYMENT_KIND_NAME);
            //if (paymentKind == null)
            //	paymentKind = customer.Db.AddPaymentKind(new PaymentKind(-1, PAYMENT_KIND_NAME));

            int paymentKindId = 1;

	        LastCreateDate = DateTime.Now;
            LastCost = cost;
			var payJournal = customer.Db.GetPayJournal(LastCreateDate, paymentKindId);
	        if (payJournal != null)
	        {
		        customer.Db.UpdatePayJournal(LastCost, payJournal.Id);
	        }
	        else
	        {
		        payJournal = customer.Db.AddPayJournal(
			        new PayJournal(-1, PAY_JOURNAL_NAME, LastCreateDate, paymentKindId), LastCost);
	        }

	        customer.Db.AddPay(
		        new Pay(-1, customer.Id, reasonId, payJournal.Id, LastCost, description));

	        var customerCounterId = customer.Db.GetCustomerCounterId(customer.Id);

	        var counterValues = customer.Db.AddCounterValues(
				new CounterValues(-1, customer.Id, customerCounterId, value1, value2), LastCreateDate);

	        customer.Db.AddMeters(new Meter(-1, customer.Id, customerCounterId, value1, value2, counterValues.Id));
        }

        private List<DbController> GetDbControllersByDbCode(string dbCode)
        {
            return department.Dbs.Select(dbModel => new DbController(dbModel))
                .Where(dbController => dbController.Model.DbCode == dbCode).ToList();
        }
    }
}
