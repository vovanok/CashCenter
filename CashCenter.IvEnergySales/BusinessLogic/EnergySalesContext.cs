using CashCenter.IvEnergySales.DAL;
using CashCenter.IvEnergySales.DataModel;
using CashCenter.IvEnergySales.DbQualification;
using CashCenter.IvEnergySales.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CashCenter.IvEnergySales.BusinessLogic
{
    public class EnergySalesContext
    {
        private const string PAY_JOURNAL_NAME = "Пачка квитанций 50 ЭСК";
        private const string PAYMENT_KIND_NAME = "CashCenter_ParmentKind";

        public DepartmentModel Departament { get; private set; }
        public Customer Customer { get; private set; }
        public CustomerCounters CustomerCountersValues { get; private set; }
        public DbController Db { get; private set; }
        public InfoForCheck InfoForCheck { get; private set; }

        public bool IsCustomerFinded
        {
            get { return Customer != null; }
        }

        public bool IsNormative
        {
            get { return CustomerCountersValues == null; }
        }

        public bool IsTwoTariff
        {
            get { return CustomerCountersValues != null && CustomerCountersValues.IsTwoTariff; }
        }

        public EnergySalesContext(int customerId, DepartmentModel department, string dbCode)
        {
            Departament = department;

            var dbControllers = GetDbControllersByDbCode(dbCode);
            foreach (var dbController in dbControllers)
            {
                // Получение плательщика
                Customer = dbController.GetCustomer(customerId);
                if (Customer != null)
                {
                    Db = dbController;

                    // Получение показаний счетчиков плательщика

                    var now = DateTime.Now;
                    var beginDate = new DateTime(now.Year, now.Month, 1);
                    var endDate = beginDate.AddMonths(1).AddDays(-1);

                    CustomerCountersValues = Db.GetCustomerCounterValues(customerId, beginDate, endDate);
                    break;
                }
            }
        }

        public List<PaymentReason> GetPaymentReasons()
        {
            if (!ValidateDb())
                return new List<PaymentReason>();

            return Db.GetPaymentReasons() ?? new List<PaymentReason>();
        }

        public bool Pay(int reasonId, int value1, int value2, decimal cost, string description)
        {
            if (!IsCustomerFinded)
            {
                Log.Error($"Ошибка совершения платежа. Отсутствует плательщик.");
                return false;
            }

            if (!ValidateReasonId(reasonId))
                return false;

            if (!ValidateDayCounterValue(value1))
                return false;

            if (!ValidateNightCounterValue(value2))
                return false;

            if (!ValidateCost(cost))
                return false;

            // TODO: tempolary solution
            //var paymentKind = GetOrAddPaymentKind(customer.Db);
            //int paymentKindId = paymentKind.Id;
            int paymentKindId = 1;

            var createDate = DateTime.Now;
            var payJournal = AddOrUpdatePayJournal(paymentKindId, createDate, cost);

            var customerCounterId = Db.GetCustomerCounterId(Customer.Id);

            int? metersId = null;
            CounterValues counterValues = null;
            if (!IsNormative)
            {
                int? correctedValue2 = IsTwoTariff ? (int?)value2 : null;

                // 1.
                counterValues = Db.AddCounterValues(new CounterValues(Customer.Id, customerCounterId, value1, correctedValue2), createDate);

                // 2.
                var meters = Db.AddMeters(new Meter(-1, Customer.Id, customerCounterId, value1, correctedValue2, counterValues.Id));
                metersId = meters.Id;
            }

            var penaltyTotal = GetPenaltyTotal(createDate, cost);

            // 3.
            var pay = Db.AddPay(new Pay(Customer.Id, reasonId, metersId, payJournal.Id, cost, penaltyTotal, description));

            // 4.
            if (counterValues != null)
                Db.UpdateCounterValuesPayId(counterValues.Id, pay.Id);

            if (penaltyTotal > 0)
                Db.AddPenaltyFee(Customer.Id, createDate, penaltyTotal, pay.Id);

            InfoForCheck = new InfoForCheck(cost, createDate);

            return true;
        }

        private PaymentKind GetOrAddPaymentKind()
        {
            if (!ValidateDb())
                return null;

            var paymentKind = Db.GetPaymentKind(PAYMENT_KIND_NAME);
            if (paymentKind == null)
                paymentKind = Db.AddPaymentKind(new PaymentKind(-1, PAYMENT_KIND_NAME));

            return paymentKind;
        }

        private PayJournal AddOrUpdatePayJournal(int paymentKindId, DateTime createDate, decimal cost)
        {
            if (!ValidateDb())
                return null;

            if (!ValidatePaymentKindId(paymentKindId))
                return null;

            if (!ValidateCost(cost))
                return null;

            var payJournal = Db.GetPayJournal(createDate, paymentKindId);
            if (payJournal != null)
                Db.AddRequireToPayJournal(payJournal, cost);
            else
                payJournal = Db.AddPayJournal(new PayJournal(PAY_JOURNAL_NAME, createDate, paymentKindId), cost);

            return payJournal;
        }

        private decimal GetPenaltyTotal(DateTime date, decimal cost)
        {
            if (!ValidateCost(cost))
                return 0;

            var debt = Db.GetDebt(Customer.Id, date.Year * 12 + date.Month);

            if (debt != null && cost > debt.Balance && debt.Penalty > 0)
                return Math.Min(cost - debt.Balance, debt.Penalty);

            return 0;
        }

        private List<DbController> GetDbControllersByDbCode(string dbCode)
        {
            return Departament.Dbs.Select(dbModel => new DbController(dbModel))
                .Where(dbController => dbController.Model.DbCode == dbCode).ToList();
        }

        private bool ValidateDb()
        {
            if (Db == null)
            {
                Log.Error($"База данных не готова.");
                return false;
            }

            return true;
        }

        private bool ValidateCost(decimal costValue)
        {
            if (costValue <= 0)
            {
                Log.Error($"Сумма платежа должна быть положительна ({costValue}).");
                return false;
            }

            return true;
        }

        private bool ValidatePaymentKindId(int paymentKindId)
        {
            if (paymentKindId < 0)
            {
                Log.Error($"Вид платежа некорректен (id = {paymentKindId}).");
                return false;
            }

            return true;
        }

        private bool ValidateDayCounterValue(int dayCounterValue)
        {
            if (IsNormative)
                return true;

            if (dayCounterValue < CustomerCountersValues.EndDayValue)
            {
                Log.Error($"Новое показание дневного счетчика меньше предыдущего ({dayCounterValue} < {CustomerCountersValues.EndDayValue}).");
                return false;
            }

            return true;
        }

        private bool ValidateNightCounterValue(int nightCounterValue)
        {
            if (IsNormative)
                return true;

            if (!IsTwoTariff)
                return true;

            if (nightCounterValue < CustomerCountersValues.EndNightValue)
            {
                Log.Error($"Новое показание ночного счетчика меньше меньше предыдущего ({nightCounterValue} < {CustomerCountersValues.EndNightValue}).");
                return false;
            }

            return true;
        }

        private bool ValidateReasonId(int reasonId)
        {
            if (reasonId < 0)
            {
                Log.Error($"Основание платежа некорректно (id = {reasonId}).");
                return false;
            }

            return true;
        }
    }
}
