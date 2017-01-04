using CashCenter.IvEnergySales.DAL;
using CashCenter.IvEnergySales.DataModel;
using CashCenter.IvEnergySales.DbQualification;
using CashCenter.IvEnergySales.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CashCenter.IvEnergySales.BusinessLogic
{
    public abstract class BaseEnergySalesContext
    {
        protected const string PAY_JOURNAL_NAME = "Пачка квитанций 50 ЭСК";
        
        public DepartmentModel Departament { get; private set; }
        public Customer Customer { get; private set; }
        public CustomerCounters CustomerCountersValues { get; private set; }
        public Debt Debt { get; private set; }
        public List<PaymentReason> PaymentReasons { get; private set; }
        public DbController Db { get; private set; }
        public InfoForCheck InfoForCheck { get; protected set; }

        public bool IsCustomerFinded
        {
            get { return Customer != null; }
        }

        public bool IsNormative
        {
            get { return IsCustomerFinded && CustomerCountersValues == null; }
        }

        public bool IsTwoTariff
        {
            get { return CustomerCountersValues != null && CustomerCountersValues.IsTwoTariff; }
        }

        public BaseEnergySalesContext(int customerId, DepartmentModel department, string dbCode)
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

                    var now = DateTime.Now;
                    var beginDate = new DateTime(now.Year, now.Month, 1);
                    var endDate = beginDate.AddMonths(1).AddDays(-1);

                    // Получение показаний счетчиков плательщика
                    CustomerCountersValues = Db.GetCustomerCounterValues(customerId, beginDate, endDate);

                    // Получение задолженности плательщика
                    Debt = Db.GetDebt(Customer.Id, now.Year * 12 + now.Month);
                    PaymentReasons = Db.GetPaymentReasons() ?? new List<PaymentReason>();
                    break;
                }
            }
        }

        public abstract bool Pay(int reasonId, int paymentKindId, int value1, int value2, decimal cost, string description, DateTime createDate);

        protected decimal GetPenaltyTotal(DateTime date, decimal cost)
        {
            if (!ValidateCost(cost) || Debt == null)
                return 0;

            if (Debt != null && cost > Debt.Balance && Debt.Penalty > 0)
                return Math.Min(cost - Debt.Balance, Debt.Penalty);

            return 0;
        }

        private List<DbController> GetDbControllersByDbCode(string dbCode)
        {
            return Departament.Dbs.Select(dbModel => new DbController(dbModel))
                .Where(dbController => dbController.Model.DbCode == dbCode).ToList();
        }

        #region Validators

        protected bool ValidateDb()
        {
            if (Db == null)
            {
                Log.Error($"База данных не готова.");
                return false;
            }

            return true;
        }

        protected bool ValidateCost(decimal costValue)
        {
            if (costValue <= 0)
            {
                Log.Error($"Сумма платежа должна быть положительна ({costValue}).");
                return false;
            }

            return true;
        }

        protected bool ValidatePaymentKindId(int paymentKindId)
        {
            if (paymentKindId < 0)
            {
                Log.Error($"Вид платежа некорректен (id = {paymentKindId}).");
                return false;
            }

            return true;
        }

        protected bool ValidateDayCounterValue(int dayCounterValue)
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

        protected bool ValidateNightCounterValue(int nightCounterValue)
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

        protected bool ValidateReasonId(int reasonId)
        {
            if (reasonId < 0)
            {
                Log.Error($"Основание платежа некорректно (id = {reasonId}).");
                return false;
            }

            return true;
        }

        #endregion
    }
}
