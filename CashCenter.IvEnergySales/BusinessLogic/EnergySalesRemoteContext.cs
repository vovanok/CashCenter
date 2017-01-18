using CashCenter.IvEnergySales.DataModel;
using CashCenter.IvEnergySales.DbQualification;
using CashCenter.IvEnergySales.Logging;
using System;
using System.Linq;

namespace CashCenter.IvEnergySales.BusinessLogic
{
    public class EnergySalesRemoteContext : BaseEnergySalesContext
    {
        public EnergySalesRemoteContext(int customerId, RegionDef regionDef, string dbCode)
            : base(customerId, regionDef, dbCode)
        {
        }

        public override bool Pay(int reasonId, int paymentKindId, int value1, int value2, decimal cost, string description, DateTime createDate)
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

            //if (penaltyTotal > 0)
            //    Db.AddPenaltyFee(Customer.Id, createDate, penaltyTotal, pay.Id);

            var paymentReasonName = PaymentReasons.FirstOrDefault(item => item.Id == reasonId)?.Name ?? string.Empty;
            InfoForCheck = new InfoForCheck(cost, createDate, Db.DepartamentDef.Code, Customer.Id, Customer.Name, paymentReasonName);

            return true;
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
    }
}
