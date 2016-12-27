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

        public InfoForCheck InfoForCheck { get; private set; }

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

        public CustomerCounters GetCustomerCounters(Customer customer)
        {
            if (customer == null)
                return null;

            return customer.Db.GetCustomerCounterValues(customer.Id);
        }

        public void Pay(Customer customer, CustomerCounters customerCounters,
            int reasonId, decimal cost, string description, int value1, int value2)
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

            //var paymentKind = GetOrAddPaymentKind(customer.Db);
            //int paymentKindId = paymentKind.Id;

            int paymentKindId = 1;
	        var createDate = DateTime.Now;
            var payJournal = AddOrUpdatePayJournal(customer.Db, paymentKindId, createDate, cost);
            var penaltyTotal = GetPenaltyTotal(customer, createDate, cost);

            var pay = customer.Db.AddPay(
		        new Pay(-1, customer.Id, reasonId, payJournal.Id, cost, penaltyTotal, description));

            if (penaltyTotal > 0)
                customer.Db.AddPenaltyFee(customer.Id, createDate, penaltyTotal, pay.Id);

            var customerCounterId = customer.Db.GetCustomerCounterId(customer.Id);

            if (customerCounters != null)
            {
                int? correctedValue2 = customerCounters.IsTwoTariff ? (int?)value2 : null;
                var counterValues = customer.Db.AddCounterValues(
                    new CounterValues(-1, customer.Id, customerCounterId, value1, correctedValue2), createDate);

                customer.Db.AddMeters(new Meter(-1, customer.Id, customerCounterId, value1, correctedValue2, counterValues.Id));
            }

            InfoForCheck = new InfoForCheck(cost, createDate);
        }

        private PaymentKind GetOrAddPaymentKind(DbController db)
        {
            const string PAYMENT_KIND_NAME = "CashCenter_ParmentKind";
            var paymentKind = db.GetPaymentKind(PAYMENT_KIND_NAME);
            if (paymentKind == null)
                paymentKind = db.AddPaymentKind(new PaymentKind(-1, PAYMENT_KIND_NAME));

            return paymentKind;
        }

        private PayJournal AddOrUpdatePayJournal(DbController db, int paymentKindId, DateTime createDate, decimal cost)
        {
            var payJournal = db.GetPayJournal(createDate, paymentKindId);
            if (payJournal != null)
                db.UpdatePayJournal(cost, payJournal.Id);
            else
                payJournal = db.AddPayJournal(
                    new PayJournal(-1, PAY_JOURNAL_NAME, createDate, paymentKindId), cost);

            return payJournal;
        }

        private decimal GetPenaltyTotal(Customer customer, DateTime date, decimal cost)
        {
            var debt = customer.Db.GetDebt(customer.Id, date.Year * 12 + date.Month);

            if (debt != null && cost > debt.Balance && debt.Penalty > 0)
                return Math.Min(cost - debt.Balance, debt.Penalty);

            return 0;
        }

        private List<DbController> GetDbControllersByDbCode(string dbCode)
        {
            return department.Dbs.Select(dbModel => new DbController(dbModel))
                .Where(dbController => dbController.Model.DbCode == dbCode).ToList();
        }
    }
}
